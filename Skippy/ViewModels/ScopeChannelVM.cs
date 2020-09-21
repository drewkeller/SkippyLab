//#define MOCK

using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Skippy.Extensions;
using Skippy.Models;
using Skippy.Protocols;
using Skippy.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Skippy.ViewModels
{
    public class ScopeChannelVM : ReactiveObject, IActivatableViewModel
    {
        ScopeChannel Model { get; set; }
#if MOCK
        ScopeChannel MockModel { get; set; }
#endif
        public ViewModelActivator Activator { get; }

        TelnetService Telnet => AppLocator.TelnetService;

        #region Scope Properties

        // is there a way to read this from the scope?
        public int ChannelNumber { get; set; }

        [Reactive] public string Name { get; set; }

        [Reactive] public string Color { get; set; }

        #region IsActive
        [Reactive] public bool IsActive { get; set; } = false;
        public ReactiveCommand<Unit, Unit> GetIsActiveCommand { get; }
        public ReactiveCommand<Unit, Unit> SetIsActiveCommand;
        [Reactive] public bool GetIsActiveSucceeded { get; set; }
        #endregion

        #region SelectChannel
        public ICommand SelectChannel { get; internal set; }

        public ICommand GetAll { get; internal set; }

        public ICommand SetAll { get; internal set; }

        #endregion

        #region Coupling
        [Reactive] public string Coupling { get; set; }
        public ReactiveCommand<Unit, Unit> GetCouplingCommand { get; }
        public ReactiveCommand<Unit, Unit> SetCouplingCommand;
        [Reactive] public bool GetCouplingSucceeded { get; set; }
        #endregion

        #region Offset
        [Reactive] public double Offset { get; set; } = 0.0;
        public ReactiveCommand<Unit, Unit> GetOffsetCommand { get; }
        public ReactiveCommand<Unit, Unit> SetOffsetCommand;
        [Reactive] public bool GetOffsetSucceeded { get; set; }
        [Reactive] public string OffsetUnits { get; set; }
        #endregion

        #region Range
        [Reactive] public double Range { get; set; } = 0.0;
        public ReactiveCommand<Unit, Unit> GetRangeCommand { get; }
        public ReactiveCommand<Unit, Unit> SetRangeCommand;
        [Reactive] public bool GetRangeSucceeded { get; set; }
        [Reactive] public string RangeUnits { get; set; } = "V";
        #endregion

        #region Scale
        [Reactive] public double Scale { get; set; } = 0.0;
        public ReactiveCommand<Unit, Unit> GetScaleCommand { get; }
        public ReactiveCommand<Unit, Unit> SetScaleCommand;
        [Reactive] public bool GetScaleSucceeded { get; set; }
        [Reactive] public string ScaleUnits { get; set; } = "V";
        #endregion

        #region Probe
        [Reactive] public StringOption ProbeRatio { get; set; }
        public ReactiveCommand<Unit, Unit> GetProbeCommand { get; }
        public ReactiveCommand<Unit, Unit> SetProbeCommand;
        [Reactive] public bool GetProbeSucceeded { get; set; }
        #endregion

        #region IsInverted
        [Reactive] public bool IsInverted { get; set; } = false;
        public ReactiveCommand<Unit, Unit> GetIsInvertedCommand { get; }
        public ReactiveCommand<Unit, Unit> SetIsInvertedCommand;
        [Reactive] public bool GetIsInvertedSucceeded { get; set; }
        #endregion

        #region IsBandwidthLimited
        [Reactive] public bool IsBandwidthLimited { get; set; }
        public ReactiveCommand<Unit, Unit> GetIsBandwidthLimitedCommand { get; }
        public ReactiveCommand<Unit, Unit> SetIsBandwidthLimitedCommand;
        [Reactive] public bool GetIsBandwidthLimitedSucceeded { get; set; }
        #endregion

        #region IsVernier
        [Reactive] public bool IsVernier { get; set; }
        public ReactiveCommand<Unit, Unit> GetIsVernierCommand { get; }
        public ReactiveCommand<Unit, Unit> SetIsVernierCommand;
        [Reactive] public bool GetIsVernierSucceeded { get; set; }
        #endregion

        #region TCal
#if false
        [Reactive] public double TCal { get; set; } = 0.0;
        public ReactiveCommand<Unit, Unit> GetTCalCommand { get; }
        public ReactiveCommand<Unit, Unit> SetTCalCommand;
        [Reactive] public bool GetTCalSucceeded { get; set; }
        [Reactive] public string TCalUnits { get; set; }
#endif
        #endregion

        #region Units
        [Reactive] public string Units { get; set; }
        public ReactiveCommand<Unit, Unit> GetUnitsCommand { get; }
        public ReactiveCommand<Unit, Unit> SetUnitsCommand;
        [Reactive] public bool GetUnitsSucceeded { get; set; }
        #endregion

        #endregion Scope Properties


        public ScopeChannelVM(int channelNumber)
        {
            Activator = new ViewModelActivator();

            Model = new ScopeChannel();
#if MOCK
            MockModel = new ScopeChannel()
            {

            };
#endif
            ChannelNumber = channelNumber;
            Name = $"CH{channelNumber}";
            switch (channelNumber)
            {
                case 1: Color = "#F8FC00"; break;
                case 2: Color = "#00FCF8"; break;
                case 3: Color = "#F800F8"; break;
                case 4: Color = "#007CF8"; break;
            }

            #region IsActive
            var canSetIsActive = this.WhenValueChanged(x => x.GetIsActiveSucceeded)
                .Where(x => x == true);
            GetIsActiveCommand = ReactiveCommand.CreateFromTask(SendIsActiveQueryAsync);
            SetIsActiveCommand = ReactiveCommand.CreateFromTask(SendIsActiveCommandAsync, canSetIsActive);
            #endregion

            SelectChannel = ReactiveCommand.CreateFromTask(SelectChannelExecute);

            #region Coupling
            var canSetCoupling = this.WhenValueChanged(x => x.GetCouplingSucceeded)
                .Where(x => x == true);
            GetCouplingCommand = ReactiveCommand.CreateFromTask(SendCouplingQueryAsync);
            SetCouplingCommand = ReactiveCommand.CreateFromTask(SendCouplingCommandAsync, canSetCoupling);
            #endregion

            #region Offset
            var canSetOffset = this.WhenValueChanged(x => x.GetOffsetSucceeded)
                .Where(x => x == true)
                .ThrottleMs(500);
            GetOffsetCommand = ReactiveCommand.CreateFromTask(SendOffsetQueryAsync);
            SetOffsetCommand = ReactiveCommand.CreateFromTask(SendOffsetCommandAsync, canSetOffset);

            // Offset units, based on Units
            this.WhenValueChanged(x => x.Units)
                .Subscribe(x => UpdateUnits());
            #endregion

            #region Range
            var canSetRange = this.WhenValueChanged(x => x.GetRangeSucceeded)
                .Where(x => x == true)
                .ThrottleMs(500);
            GetRangeCommand = ReactiveCommand.CreateFromTask(SendRangeQueryAsync);
            SetRangeCommand = ReactiveCommand.CreateFromTask(SendRangeCommandAsync, canSetRange);
            #endregion

            #region Scale
            var canSetScale = this.WhenValueChanged(x => x.GetScaleSucceeded)
                .Where(x => x == true)
                .ThrottleMs(500);
            GetScaleCommand = ReactiveCommand.CreateFromTask(SendScaleQueryAsync);
            SetScaleCommand = ReactiveCommand.CreateFromTask(SendScaleCommandAsync, canSetScale);
            #endregion

            #region Probe
            var canSetProbe = this.WhenValueChanged(x => x.GetProbeSucceeded)
                .Where(x => x == true);
            GetProbeCommand = ReactiveCommand.CreateFromTask(SendProbeQueryAsync);
            SetProbeCommand = ReactiveCommand.CreateFromTask(SendProbeCommandAsync, canSetProbe);
            #endregion

            #region IsBandwidthLimited
            var canSetIsBandwidthLimited = this.WhenValueChanged(x => x.GetIsBandwidthLimitedSucceeded)
                .Where(x => x == true);
            GetIsBandwidthLimitedCommand = ReactiveCommand.CreateFromTask(SendIsBandwidthLimitedQueryAsync);
            SetIsBandwidthLimitedCommand = ReactiveCommand.CreateFromTask(SendIsBandwidthLimitedCommandAsync, canSetIsBandwidthLimited);
            #endregion

            #region IsInverted
            var canSetIsInverted = this.WhenValueChanged(x => x.GetIsInvertedSucceeded)
                .Where(x => x == true);
            GetIsInvertedCommand = ReactiveCommand.CreateFromTask(SendIsInvertedQueryAsync);
            SetIsInvertedCommand = ReactiveCommand.CreateFromTask(SendIsInvertedCommandAsync, canSetIsInverted);
            #endregion

            #region IsVernier
            var canSetIsVernier = this.WhenValueChanged(x => x.GetIsVernierSucceeded)
                .Where(x => x == true);
            GetIsVernierCommand = ReactiveCommand.CreateFromTask(SendIsVernierQueryAsync);
            SetIsVernierCommand = ReactiveCommand.CreateFromTask(SendIsVernierCommandAsync, canSetIsVernier);
            #endregion

            #region TCal
#if TCAL
            var canSetTCal = this.WhenValueChanged(x => x.GetTCalSucceeded)
                .Where(x => x == true);
            GetTCalCommand = ReactiveCommand.CreateFromTask(SendTCalQueryAsync);
            SetTCalCommand = ReactiveCommand.CreateFromTask(SendTCalCommandAsync, canSetTCal);
#endif
            #endregion

            #region Units
            var canSetUnits = this.WhenValueChanged(x => x.GetUnitsSucceeded)
                .Where(x => x == true);
            GetUnitsCommand = ReactiveCommand.CreateFromTask(SendUnitsQueryAsync);
            SetUnitsCommand = ReactiveCommand.CreateFromTask(SendUnitsCommandAsync, canSetUnits);
            #endregion

            #region Get/Set All
            var GetAllMessage = ReactiveCommand.Create(() =>
                Debug.WriteLine($"------- Retrieving all CHANNEL{ChannelNumber} values from device ---------"));
            GetAll = ReactiveCommand.CreateCombined(new[] {
                GetAllMessage,
                GetIsActiveCommand,
                GetCouplingCommand,
                GetOffsetCommand,
                GetRangeCommand,
                GetScaleCommand,
                GetProbeCommand,
                GetIsBandwidthLimitedCommand,
                GetIsInvertedCommand,
                GetIsVernierCommand,
                GetUnitsCommand,
            });

            var SetAllMessage = ReactiveCommand.Create(() =>
                Debug.WriteLine($"------- Setting all CHANNEL{ChannelNumber} values on device ---------"));
            SetAll = ReactiveCommand.CreateCombined(new[] {
                SetAllMessage,
                SetIsActiveCommand,
                SetCouplingCommand,
                SetOffsetCommand,
                SetRangeCommand,
                SetScaleCommand,
                SetProbeCommand,
                SetIsBandwidthLimitedCommand,
                SetIsInvertedCommand,
                SetIsVernierCommand,
                SetUnitsCommand,
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

                this.WhenPropertyChanged(x => x.IsActive)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetIsActiveCommand)
                    .DisposeWith(disposables);

                this.WhenPropertyChanged(x => x.Coupling)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetCouplingCommand)
                    .DisposeWith(disposables);

                this.WhenValueChanged(x => x.Offset)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetOffsetCommand)
                    .DisposeWith(disposables);

                this.WhenValueChanged(x => x.Range)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetRangeCommand)
                    .DisposeWith(disposables);

                this.WhenValueChanged(x => x.Scale)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetScaleCommand)
                    .DisposeWith(disposables);

                this.WhenValueChanged(x => x.ProbeRatio)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetProbeCommand)
                    .DisposeWith(disposables);

                this.WhenPropertyChanged(x => x.IsBandwidthLimited)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetIsBandwidthLimitedCommand)
                    .DisposeWith(disposables);

                this.WhenPropertyChanged(x => x.IsInverted)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetIsInvertedCommand)
                    .DisposeWith(disposables);

                this.WhenValueChanged(x => x.IsVernier)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetIsVernierCommand)
                    .DisposeWith(disposables);

#if TCAL
                this.WhenValueChanged(x => x.TCal)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetTCalCommand)
                    .DisposeWith(disposables);
#endif

                this.WhenValueChanged(x => x.Units)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetUnitsCommand)
                    .DisposeWith(disposables);

            });

        }

        /// <summary>
        /// Sends the channel "on" command with whatever the current state is, 
        /// in order to select it on the scope.
        /// </summary>
        /// <returns></returns>
        private async Task SelectChannelExecute()
        {
            var parameter = IsActive ? "1" : "0";
            await SendCommandAsync(ChannelSubcommand.DISPlay, parameter);
        }

        private void HandleActivation()
        {
        }

        private void HandleDeactivation()
        {
        }

        #region Scope commands

        public async Task SendGetAllQuery()
        {
            await SendIsActiveQueryAsync();
            await SendOffsetQueryAsync();
            await SendCouplingQueryAsync();
            await SendRangeQueryAsync();
#if TCAL
            await SendTCalQueryAsync();
#endif
            await SendScaleQueryAsync();
            await SendProbeQueryAsync();
            await SendIsInvertedQueryAsync();
            await SendIsBandwidthLimitedQueryAsync();
            await SendUnitsQueryAsync();
            //await SendVernierQueryAsync();
            //GetIsActiveSucceeded = true;
            //GetOffsetSucceeded = true;
            //GetCouplingSucceeded = true;
        }

        #region IsActive
        public async Task SendIsActiveQueryAsync()
        {
            // mark as not succeeded until after we update our properties to avoid also sending out
            // the command to set it, directly after
            GetIsActiveSucceeded = false;
#if MOCK
            await Task.Delay(1);
            var response = "1";
#else
            var response = await SendQueryAsync(ChannelSubcommand.DISPlay);
            if (response == "") return;
#endif
            Model.IsActive = response == "1";
            this.IsActive = Model.IsActive;
            GetIsActiveSucceeded = true;
            Log($"IsActive: {Model.IsActive}");
        }

        public async Task SendIsActiveCommandAsync()
        {
            await SendCommandAsync(ChannelSubcommand.DISPlay, this.IsActive == true);
        }
        #endregion

        #region Coupling
        public async Task SendCouplingQueryAsync()
        {
            GetCouplingSucceeded = false;
#if MOCK
            await Task.Delay(1);
            var response = "AC";
#else
            var response = await SendQueryAsync(ChannelSubcommand.COUPling);
#endif
            if (response == "") return;
            var result = StringOptions.Coupling.GetByParameter(response);
            if (result != null)
            {
                Model.Coupling = result.Name;
                this.Coupling = result.Name;
                GetCouplingSucceeded = true;
            }
            Log($"Coupling: {response}");
        }

        public async Task SendCouplingCommandAsync()
        {
            var option = StringOptions.Coupling.GetByValue(Coupling);
            await SendCommandAsync(ChannelSubcommand.COUPling, option.Name);
        }
        #endregion

        #region Offset
        public async Task SendOffsetQueryAsync()
        {
            GetOffsetSucceeded = false;
#if MOCK
            await Task.Delay(1);
            var response = "0";
#else
            var response = await SendQueryAsync(ChannelSubcommand.OFFSet);
#endif
            if (response == "") return;
            if (double.TryParse(response, out var result))
            {
                Model.Offset = result;
                this.Offset = result;
                GetOffsetSucceeded = true;
            }
            Log($"Offset: {this.Offset}");
        }


        public async Task SendOffsetCommandAsync()
        {
#if MOCK
            await Task.Delay(1);
#else
            var value = this.Offset.ToString();
            await SendCommandAsync(ChannelSubcommand.OFFSet, value);
#endif
        }
        #endregion

        #region TCal
#if TCAL
        public async Task SendTCalQueryAsync()
        {
            GetTCalSucceeded = false;
            var response = await SendQueryAsync(ChannelSubcommand.TCAL);
            if (response == "") return;
            if (double.TryParse(response, out double result))
            {
                Model.TCal = result;
                this.TCal = result;
                GetTCalSucceeded = true;
            }
            Log($"TCal: {this.TCal}");
        }

        public async Task SendTCalCommandAsync()
        {
            var value = this.TCal.ToString();
            await SendCommandAsync(ChannelSubcommand.TCAL, value);
        }
#endif
        #endregion

        #region Range
        public async Task SendRangeQueryAsync()
        {
            GetRangeSucceeded = false;
#if MOCK
            await Task.Delay(1);
            var response = "8";
#else
            var response = await SendQueryAsync(ChannelSubcommand.RANGe);
#endif
            if (response == "") return;
            if (double.TryParse(response, out var result))
            {
                Model.Range = result;
                this.Range = result;
                GetRangeSucceeded = true;
            }
            Log($"Range: {this.Range}");
        }


        public async Task SendRangeCommandAsync()
        {
            var value = this.Range.ToString();
            await SendCommandAsync(ChannelSubcommand.RANGe, value);
        }
        #endregion

        #region Scale
        public async Task SendScaleQueryAsync()
        {
            GetScaleSucceeded = false;
#if MOCK
            await Task.Delay(1);
            var response = "1";
#else
            var response = await SendQueryAsync(ChannelSubcommand.SCALe);
#endif
            if (response == "") return;
            if (double.TryParse(response, out var result))
            {
                Model.Scale = result;
                this.Scale = result;
                GetScaleSucceeded = true;
            }
            Log($"Scale: {this.Scale}");
        }


        public async Task SendScaleCommandAsync()
        {
            var value = this.Scale.ToString();
            await SendCommandAsync(ChannelSubcommand.SCALe, value);
        }
        #endregion

        #region Probe
        public async Task SendProbeQueryAsync()
        {
            GetProbeSucceeded = false;
#if MOCK
            await Task.Delay(1);
            var response = "10";
#else
            var response = await SendQueryAsync(ChannelSubcommand.PROBe);
#endif
            if (response == "") return;
            if (double.TryParse(response, out var result))
            {
                var val = result.ToString();
                if (val.StartsWith("0")) val = val.Substring(1);
                var ratio = StringOptions.ProbeRatio.GetByValue(val + "x");
                Model.Probe = ratio.Name;
                this.ProbeRatio = ratio;
                GetProbeSucceeded = true;
            }
            Log($"Probe: {this.ProbeRatio}");
        }


        public async Task SendProbeCommandAsync()
        {
            //var option = StringOptions.ProbeRatio.GetByValue(this.ProbeRatio);
            var option = ProbeRatio;
            await SendCommandAsync(ChannelSubcommand.PROBe, option.Term);
        }
        #endregion

        #region IsInverted
        public async Task SendIsInvertedQueryAsync()
        {
            GetIsInvertedSucceeded = false;
#if MOCK
            await Task.Delay(1);
            var response = "0";
#else
            var response = await SendQueryAsync(ChannelSubcommand.INVert);
#endif
            if (response == "") return;
            Model.IsInverted = response == "1";
            this.IsInverted = Model.IsInverted;
            GetIsInvertedSucceeded = true;
            Log($"Inverted: {Model.IsInverted}");
        }

        public async Task SendIsInvertedCommandAsync()
        {
            await SendCommandAsync(ChannelSubcommand.INVert, this.IsInverted == true);
        }
        #endregion

        #region IsBandwidthLimit
        public async Task SendIsBandwidthLimitedQueryAsync()
        {
            GetIsBandwidthLimitedSucceeded = false;
#if MOCK
            await Task.Delay(1);
            var response = "0";
#else
            var response = await SendQueryAsync(ChannelSubcommand.BWLimit);
#endif
            if (response == "") return;
            var result = response == "20M";
            //Model. = result;
            this.IsBandwidthLimited = result; // == true ? "Fine" : "Coarse";
            GetIsBandwidthLimitedSucceeded = true;
            Log($"BandwidthLimit: {response}");
        }

        public async Task SendIsBandwidthLimitedCommandAsync()
        {
            var param = this.IsBandwidthLimited; // == "Fine";
            await SendCommandAsync(ChannelSubcommand.BWLimit, param);
        }
        #endregion

        #region IsVernier
        public async Task SendIsVernierQueryAsync()
        {
            GetIsVernierSucceeded = false;
#if MOCK
            await Task.Delay(1);
            var response = "0";
#else
            var response = await SendQueryAsync(ChannelSubcommand.VERNier);
#endif
            if (response == "") return;
            var result = response == "1";
            this.IsVernier = result;
            GetIsVernierSucceeded = true;
            Log($"Vernier: {result}");
        }

        public async Task SendIsVernierCommandAsync()
        {
            await SendCommandAsync(ChannelSubcommand.VERNier, this.IsVernier);
        }
        #endregion

        #region Units
        public async Task SendUnitsQueryAsync()
        {
            GetUnitsSucceeded = false;
#if MOCK
            await Task.Delay(1);
            var response = "VOLT";
#else
            var response = await SendQueryAsync(ChannelSubcommand.UNITs);
#endif
            if (response == "") return;
            var result = StringOptions.Units.GetByParameter(response);
            if (result != null)
            {
                Model.Units = result.Name;
                this.Units = result.Name;
                GetUnitsSucceeded = true;
            }
            Log($"Units: {response}");
        }

        public async Task SendUnitsCommandAsync()
        {
            var option = StringOptions.Units.GetByValue(this.Units);
            await SendCommandAsync(ChannelSubcommand.UNITs, option.Term);
        }
        #endregion

        #endregion // Scope commands

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
                : Units == "Volts" ? "V"
                : Units == "Amps" ? "A"
                : Units == "Watts" ? "W"
                : "";
            OffsetUnits = mainUnits;
            RangeUnits = mainUnits;
            ScaleUnits = mainUnits == "" ? "" : $"{mainUnits}/div";
        }

    }
}
