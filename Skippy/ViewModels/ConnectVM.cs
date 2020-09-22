using Acr.UserDialogs;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace Skippy.ViewModels
{
    public class ConnectVM : ReactiveObject, IActivatableViewModel
    {

        public ViewModelActivator Activator { get; }

        [Reactive] public string Hostname { get; set; }

        [Reactive] public int Port { get; set; }

        [Reactive] public bool IsConnected { get; set; }

        public ReactiveCommand<Unit, Unit> ConnectCommand { get; set; }

        public ConnectVM()
        {
            Activator = new ViewModelActivator();
            var settings = AppLocator.Settings;
            Hostname = settings.HostName;
            Port = settings.Port;

            this.WhenActivated(disposables =>
            {
                this.HandleActivation();

                Disposable
                    .Create(() => this.HandleDeactivation())
                    .DisposeWith(disposables);

                ConnectCommand = ReactiveCommand
                    .CreateFromTask(ExecuteConnect);
                ConnectCommand.ThrownExceptions.Subscribe(ex =>
                {
                    Debug.Write(ex.Message);
                });

                var telnet = AppLocator.TelnetService;
                telnet.WhenPropertyChanged(x => x.Connected)
                    .ToProperty(this, nameof(IsConnected));
            });

        }

        private void HandleActivation()
        {
        }

        private void HandleDeactivation()
        {
        }

        public async Task<Unit> ExecuteConnect()
        {
            var telnet = AppLocator.TelnetService;
            await telnet.ConnectAsync(Hostname, Port);
            IsConnected = telnet.Connected;
            return Unit.Default;
        }

    }
}
