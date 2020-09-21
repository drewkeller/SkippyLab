﻿using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using System.Threading.Tasks;

namespace Skippy.ViewModels
{
    public class ConnectVM : ReactiveObject
    {

        [Reactive] public string Hostname { get; set; }

        [Reactive] public int Port { get; set; }

        [Reactive] public bool IsConnected { get; set; }

        public ReactiveCommand<Unit, Unit> ConnectCommand { get; }

        public ConnectVM()
        {
            var settings = AppLocator.Settings;
            Hostname = settings.HostName;
            Port = settings.Port;

            ConnectCommand = ReactiveCommand
                .CreateFromTask(ExecuteConnect);

            var telnet = AppLocator.TelnetService;
            telnet.WhenPropertyChanged(x => x.Connected)
                .ToProperty(this, nameof(IsConnected));
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