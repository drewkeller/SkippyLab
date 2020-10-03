using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Skippy.Extensions;
using Skippy.Protocols;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Skippy.ViewModels
{

    public interface IProtocolVM
    {
        IProtocolCommand Protocol { get; }
    }

    public interface IScopeCommand
    {
        void WhenActivated(CompositeDisposable disposables);
        ReactiveCommand<Unit, Unit> GetCommand { get; }
        ReactiveCommand<Unit, Unit> SetCommand { get; }
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
        public ReactiveCommand<Unit, Unit> Increment { get; internal set; }
        public ReactiveCommand<Unit, Unit> Decrement { get; internal set; }

        /// <summary>
        /// Only needed for <see cref="App.Mock"/>
        /// </summary>
        private string DefaultResponse { get; set; }

        public ScopeCommand(IProtocolVM viewModel, IProtocolCommand protocolCommand, string defaultResponse)
        {
            //ViewModel = viewModel;
            ProtocolCommand = protocolCommand;

            if (protocolCommand.IsSettable)
            {
                var canSet = this.WhenValueChanged(x => x.GetSucceeded)
                    .Where(x => x == true);
                SetCommand = ReactiveCommand.CreateFromTask(SendCommandAsync, canSet);
                SetCommand.ThrownExceptions.SubscribeOnUI().Subscribe(async ex => await DisplayException(ex, "Couldn't set"));
                Increment = ReactiveCommand.Create(IncrementValue);
                Increment.ThrownExceptions.SubscribeOnUI().Subscribe(async ex => await DisplayException(ex, "Couldn't increment"));
                Decrement = ReactiveCommand.Create(DecrementValue);
                Decrement.ThrownExceptions.SubscribeOnUI().Subscribe(async ex => await DisplayException(ex, "Couldn't decrement"));
            } else
            {
                // set command is used to send the command without a value or response
                SetCommand = ReactiveCommand.CreateFromTask(SendCommandAsync);
            }

            if (protocolCommand.IsQueryable)
            {
                if (App.Mock)
                {
                    DefaultResponse = defaultResponse;
                }
                GetCommand = ReactiveCommand.CreateFromTask(SendQueryAsync);
                GetCommand.ThrownExceptions.SubscribeOnUI().Subscribe(async ex => await DisplayException(ex, "Couldn't query"));
            } else
            {
                if (App.Mock)
                {
                    DefaultResponse = null;
                }
                GetSucceeded = true;
            }


        }

        private async Task DisplayException(Exception ex, string semiDetails)
        {
            await AppLocator.CurrentPage.DisplayAlert($"Whooops! {semiDetails}...", ex.Message, "OK");
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

            var result = "";
            if (App.Mock)
            {
                await Task.Delay(1);
                result = DefaultResponse;
            }
            else
            {
                var response = await AppLocator.TelnetService.SendCommandAsync(command, true);
                // remove line terminator
                result = response?.TrimEnd();
            }
            if (result.Length == 0) return;

            if (result == "Command error")
            {
                throw new InvalidOperationException($"Command error: {command}");
            }

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

        public void IncrementValue()
        {
            var options = ProtocolCommand.Options;
            if (options is StringOptions soptions)
            {
                this.Value = (T)soptions.GetIncrementedValue(this.Value as string);
            } else if (options is RealOptions real)
            {
                var dbl = System.Convert.ToDouble((object)this.Value);
                var inc = real.GetIncrementedValue(dbl);
                this.Value = (T)System.Convert.ChangeType(inc, typeof(T));
            }
            else
            {
                throw new NotImplementedException($"Scope command doesn't know how to increment options {options}");
            }
        }

        public void DecrementValue()
        {
            var options = ProtocolCommand.Options;
            if (options is StringOptions soptions)
            {
                this.Value = (T)soptions.GetDecrementedValue(this.Value as string);
            } else if (options is RealOptions real)
            {
                var dbl = System.Convert.ToDouble((object)this.Value);
                var dec = real.GetDecrementedValue(dbl);
                this.Value = (T)System.Convert.ChangeType(dec, typeof(T));
            }
            else
            {
                throw new NotImplementedException($"Scope command doesn't know how to increment options {options}");
            }
        }

        public override string ToString()
        {
            if (ProtocolCommand != null)
            {
                return $"{ProtocolCommand.Name}: {Value}";
            }
            return base.ToString();
        }

    }

}
