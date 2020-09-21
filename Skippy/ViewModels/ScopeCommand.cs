//#define MOCK

using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Skippy.Extensions;
using Skippy.Protocols;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Skippy.ViewModels
{

    public interface IProtocolVM
    {
        IProtocolCommand Protocol { get; }
    }

    public interface IScopeCommand
    {
        void WhenActivated(CompositeDisposable disposables);
    }

    public interface IScopeCommand<T> : IScopeCommand
    {
        T Value { get; }
        bool GetSucceeded { get; }
    }

    /// <summary>
    /// Helper for ViewModels to create and handle protocol related commands in the view model.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScopeCommand<T> : ReactiveObject, IScopeCommand<T>
    {
        //IProtocolVM ViewModel { get; set; }
        IProtocolCommand ProtocolCommand { get; set; }
        [Reactive] public T Value { get; set; }
        public ReactiveCommand<Unit, Unit> GetCommand { get; }
        public ReactiveCommand<Unit, Unit> SetCommand { get; }
        [Reactive] public bool GetSucceeded { get; set; }
#if MOCK        
        private string DefaultResponse { get; set; }
#endif

        public ScopeCommand(IProtocolVM viewModel, IProtocolCommand protocolCommand, string defaultResponse)
        {
            //ViewModel = viewModel;
            ProtocolCommand = protocolCommand;

            if (protocolCommand.IsSettable)
            {
                var canSet = this.WhenValueChanged(x => x.GetSucceeded)
                    .Where(x => x == true);
                SetCommand = ReactiveCommand.CreateFromTask(SendCommandAsync, canSet);
            } else
            {
                // set command is used to send the command without a value or response
                SetCommand = ReactiveCommand.CreateFromTask(SendCommandAsync);
            }

            if (protocolCommand.IsQueryable)
            {
#if MOCK
                DefaultResponse = defaultResponse;
#endif
                GetCommand = ReactiveCommand.CreateFromTask(SendQueryAsync);
            } else
            {
#if MOCK
                DefaultResponse = null;
#endif
                GetSucceeded = true;
            }
        }

        public ScopeCommand(IProtocolVM viewModel, IProtocolCommand protocolCommand) 
            : this(viewModel, protocolCommand, "")
        {
        }

        public void WhenActivated(CompositeDisposable disposables)
        {
            // when the view changes the ViewModel's property,
            // send the command to set it on the scope
            if (ProtocolCommand.IsSettable)
            {
                this.WhenValueChanged(x => x.Value)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetCommand)
                    .DisposeWith(disposables);
            }
        }

        public async Task SendCommandAsync()
        {
            string value;
            if (ProtocolCommand.Options is StringOptions options)
            {
                value = options.GetTermForValue(this.Value.ToString());
            }
            else if (Value is bool b)
            {
                value = b ? "1" : "0";
            }
            else
            {
                value = $"{Value}";
            }
            var path = ProtocolCommand.FormatPath();
            if (value.Length > 0)
            {
                await AppLocator.TelnetService.SendCommandAsync($"{path} {value}", false);
            } 
            else
            {
                await AppLocator.TelnetService.SendCommandAsync(path, false);
            }
        }

        public async Task SendQueryAsync()
        {
            GetSucceeded = false;
            var path = ProtocolCommand.FormatPath();
            var command = $"{path}?";
#if MOCK
   await Task.Delay(1);
   var result = DefaultResponse;
#else
            var response = await AppLocator.TelnetService.SendCommandAsync(command, true);
            // remove line terminator
            var result = response?.TrimEnd();
#endif
            if (result.Length == 0) return;

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
            if (!ProtocolCommand.Name.Contains("Status"))
            {
                System.Diagnostics.Debug.WriteLine($"{ProtocolCommand.Name}: {Value}");
            }
        }
        private PropertyInfo _propInfo;

    }

}
