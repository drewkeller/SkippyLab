#define MOCK

using Acr.UserDialogs.Infrastructure;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Scoopy.Protocols;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Scoopy.ViewModels
{

    public class TriggerVM : ReactiveObject, IActivatableViewModel
    {

        public ViewModelActivator Activator { get; }

        public ICommand GetAll { get; internal set; }

        public ICommand SetAll { get; internal set; }


        #region Properties

        public ScopeCommand<string> Mode { get; set; }
        public ScopeCommand<string> Source { get; set; }
        public ScopeCommand<string> Slope { get; set; }
        public ScopeCommand<double> Level { get; set; }
        #endregion Properties


        public TriggerVM()
        {
            Activator = new ViewModelActivator();

            Mode = new ScopeCommand<string>(this, TriggerCommands.Mode, "EDGE");
            Source = new ScopeCommand<string>(this, TriggerEdgeCommands.Source, "CHAN1");
            Slope = new ScopeCommand<string>(this, TriggerEdgeCommands.Slope, "POS");
            Level = new ScopeCommand<double>(this, TriggerEdgeCommands.Level, "0");

            GetAll = ReactiveCommand.CreateCombined(new[]
            {
                Mode.GetCommand,
                Source.GetCommand,
                Slope.GetCommand,
                Level.GetCommand,
            });
        }

        public async Task SendCommandAsync(string subCommand, string value)
        {
            await AppLocator.TelnetService.SendCommandAsync($"TRIG:{subCommand} {value}", false);
        }

    }

    public class ScopeCommand<T> : ReactiveObject
    {
        TriggerVM ViewModel { get; set; }
        IProtocolCommand ProtocolCommand {get;set;}
        [Reactive] public T Value { get; set; }
        public ReactiveCommand<Unit, Unit> GetCommand { get; }
        public ReactiveCommand<Unit, Unit> SetCommand { get; }
        [Reactive] public bool GetSucceeded { get; set; }
        private string DefaultResponse { get; set; }

        public ScopeCommand(TriggerVM viewModel, IProtocolCommand protocolCommand, string defaultResponse)
        {
            ViewModel = viewModel;
            ProtocolCommand = protocolCommand;
            DefaultResponse = defaultResponse;
            var canSetIsActive = this.WhenValueChanged(x => x.GetSucceeded)
                .Where(x => x == true);
            GetCommand = ReactiveCommand.CreateFromTask(SendQueryAsync);
            SetCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await SendCommandAsync();
            }, canSetIsActive);
        }

        public async Task SendCommandAsync()
        {
            var value = "";
            if (Value is bool b)
            {
                value = b ? "1" : "0";
            } 
            else
            {
                value = $"{Value}";
            }
            await ViewModel.SendCommandAsync(ProtocolCommand.Term, value);
        }

        public async Task SendQueryAsync()
        {
            GetSucceeded = false;
            var command = $":TRIG:{ProtocolCommand.Term}?";
#if MOCK
   await Task.Delay(1);
   var result = DefaultResponse;
#else
            var response = await AppLocator.TelnetService.SendCommandAsync(command, true);
            // remove line terminator
            var result = response?.TrimEnd();
#endif
            if (result == "") return;

            var propInfo = _propInfo ?? this.GetType().GetProperty(nameof(Value));
            if (_propInfo == null) _propInfo = propInfo;
            var type = typeof(T);
            if (type == typeof(string) || type == typeof(StringOption))
            {
                var value = StringOptions.GetByTerm(ProtocolCommand.Options, result);
                propInfo.SetValue(this, $"{value}");
            }
            else if (type == typeof(bool))
            {
                propInfo.SetValue(this, result == "1");
            }
            else if (type == typeof(int) || type == typeof(IntegerOption))
            {
                if (int.TryParse(result, out var val))
                {
                    propInfo.SetValue(this, val);
                }
            }
            else if (type == typeof(double) || type == typeof(RealOption))
            {
                if (double.TryParse(result, out var val))
                {
                    propInfo.SetValue(this, val);
                }
            }
            GetSucceeded = true;
            System.Diagnostics.Debug.WriteLine($"{ProtocolCommand.Name}: {Value}");
        }
        private PropertyInfo _propInfo;

    }

}
