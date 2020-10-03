using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Skippy.Converters;
using Skippy.Extensions;
using Skippy.Models;
using Skippy.Protocols;
using Skippy.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms.Xaml;

namespace Skippy.ViewModels
{
    public class ScopeChannelVM : ReactiveObject, IActivatableViewModel, IProtocolVM
    {
        ScopeChannel Model { get; set; }

        ScopeChannel MockModel { get; set; }

        public ViewModelActivator Activator { get; }

        public ChannelProtocol Protocol { get; set; }
        IProtocolCommand IProtocolVM.Protocol => Protocol;

        TelnetService Telnet => AppLocator.TelnetService;

        #region Scope Properties

        // is there a way to read this from the scope?
        public int ChannelNumber { get; set; }

        [Reactive] public string Name { get; set; }

        [Reactive] public string Color { get; set; }

        private List<IScopeCommand> AllCommands { get; set; }

        public ICommand GetAll { get; internal set; }

        public ICommand SetAll { get; internal set; }

        public ICommand SelectChannel { get; internal set; }

        public ScopeCommand<bool> Display { get; set; }
        public ScopeCommand<bool> BWLimit { get; set; }
        public ScopeCommand<string> Coupling { get; set; }
        public ScopeCommand<double> Offset { get; set; }
        public ScopeCommand<bool> Invert { get; set; }
        public ScopeCommand<double> Range { get; set; }
        public ScopeCommand<double> Scale { get; set; }
        public ScopeCommand<string> Probe { get; set; }
        public ScopeCommand<string> Units { get; set; }
        public ScopeCommand<double> TCal { get; set; }
        public ScopeCommand<bool> Vernier { get; set; }

        [Reactive] public string OffsetUnits { get; set; }

        [Reactive] public string RangeUnits { get; set; } = "V";

        [Reactive] public string ScaleUnits { get; set; } = "V";

        #endregion Scope Properties


        public ScopeChannelVM(int channelNumber)
        {
            Activator = new ViewModelActivator();
            Protocol = new ChannelProtocol(null, channelNumber);
            Model = new ScopeChannel();

            if (App.Mock)
            {
                MockModel = new ScopeChannel();
            }

            ChannelNumber = channelNumber;
            Name = $"CH{channelNumber}";
            switch (channelNumber)
            {
                case 1: Color = "#F8FC00"; break;
                case 2: Color = "#00FCF8"; break;
                case 3: Color = "#F800F8"; break;
                case 4: Color = "#007CF8"; break;
            }

            Display = new ScopeCommand<bool>(this, Protocol.Display, "OFF");
            BWLimit = new ScopeCommand<bool>(this, Protocol.BWLimit, "OFF");
            Coupling = new ScopeCommand<string>(this, Protocol.Coupling, "AC");
            Invert = new ScopeCommand<bool>(this, Protocol.Invert, "OFF");
            Offset = new ScopeCommand<double>(this, Protocol.Offset);
            Range = new ScopeCommand<double>(this, Protocol.Range);
            TCal = new ScopeCommand<double>(this, Protocol.TCal);
            Scale = new ScopeCommand<double>(this, Protocol.Scale);
            Probe = new ScopeCommand<string>(this, Protocol.Probe);
            Vernier = new ScopeCommand<bool>(this, Protocol.Vernier);
            Units = new ScopeCommand<string>(this, Protocol.Units);

            AllCommands = new List<IScopeCommand>()
            {
                Display, BWLimit, Coupling, Invert, Offset, Range, TCal,
                Scale, Probe, Units, Vernier
            };

            SelectChannel = ReactiveCommand.CreateFromTask(SelectChannelExecute);

            // Offset units, based on Units
            this.WhenValueChanged(x => x.Units)
                .Subscribe(x => UpdateUnits());

            #region Get/Set All
            var GetAllMessage = ReactiveCommand.Create(() =>
                Debug.WriteLine($"------- Retrieving all CHANNEL{ChannelNumber} values from device ---------"));
            GetAll = ReactiveCommand.Create(async () =>
            {
                AppLocator.TelnetService.AutoGetScreenshotAfterCommand = false;
                try
                {
                    await GetAllMessage.Execute();
                    foreach (var command in AllCommands)
                    {
                        await command.GetCommand.Execute();
                    }
                }
                finally
                {
                    AppLocator.TelnetService.AutoGetScreenshotAfterCommand = true;
                }
            });

            var SetAllMessage = ReactiveCommand.Create(() =>
                Debug.WriteLine($"------- Setting all CHANNEL{ChannelNumber} values on device ---------"));
            SetAll = ReactiveCommand.Create(async () =>
            {
                AppLocator.TelnetService.AutoGetScreenshotAfterCommand = false;
                try
                {
                    await GetAllMessage.Execute();
                    foreach (var command in AllCommands)
                    {
                        await command.SetCommand.Execute();
                    }
                }
                finally
                {
                    AppLocator.TelnetService.AutoGetScreenshotAfterCommand = true;
                }
            });
            #endregion

            // watch our own properties and call commands that update the model

            //this.WhenPropertyChanged(x => x.IsActive)
            //    .InvokeCommand(SetIsActiveCommand);
            this.WhenActivated(disposables =>
            {
                this.HandleActivation();

                Disposable
                    .Create(() => this.HandleDeactivation())
                    .DisposeWith(disposables);

                foreach (var scopeCommand in AllCommands)
                {
                    scopeCommand.WhenActivated(disposables);
                }

                this.WhenPropertyChanged(vm => vm.Probe.Value)
                .SubscribeOnUI()
                .Subscribe((x) => {
                    var options = Protocol.Scale.Options as RealOptions;
                    var probeOptions = Protocol.Probe.Options as StringOptions;
                    var value = probeOptions.GetByValue(Probe.Value);
                    if (value != null)
                    {
                        options.GetChannelScaleOption(double.Parse(value.Term));
                    }
                });
#if TCAL
                this.WhenValueChanged(x => x.TCal)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetTCalCommand)
                    .DisposeWith(disposables);
#endif

            });

        }

        /// <summary>
        /// Sends the channel "on" command with whatever the current state is, 
        /// in order to select it on the scope.
        /// </summary>
        /// <returns></returns>
        private async Task SelectChannelExecute()
        {
            var parameter = Display.Value ? "1" : "0";
            await SendCommandAsync(ChannelSubcommand.DISPlay, parameter);
        }

        private void HandleActivation()
        {
        }

        private void HandleDeactivation()
        {
        }

        #region Command helpers
        public async Task<string> SendQueryAsync(ChannelSubcommand subCommand)
        {
            var command = $":CHAN{ChannelNumber}:{subCommand}?";
            var response = await Telnet.SendCommandAsync(command, true);
            // remove line terminator
            var result = response?.TrimEnd();
            return result;
        }

        public async Task SendCommandAsync(ChannelSubcommand subCommand, string param)
        {
            await Telnet.SendCommandAsync($"CHAN{ChannelNumber}:{subCommand} {param}", false);
        }

        public async Task SendCommandAsync(ChannelSubcommand subCommand, bool param)
        {
            var value = param ? "1" : "0";
            await Telnet.SendCommandAsync($"CHAN{ChannelNumber}:{subCommand} {value}", false);
        }

        private void Log(string message)
        {
            Debug.WriteLine($"CH{this.ChannelNumber} {message}");
        }

        #endregion // command helpers

        private void UpdateUnits()
        {
            var mainUnits =
                Units == null ? ""
                : Units.Value == "Volts" ? "V"
                : Units.Value == "Amps" ? "A"
                : Units.Value == "Watts" ? "W"
                : "";
            OffsetUnits = mainUnits;
            RangeUnits = mainUnits;
            ScaleUnits = mainUnits == "" ? "" : $"{mainUnits}/div";
        }

    }
}
