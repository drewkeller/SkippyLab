﻿#define MOCK
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Scoopy.Enums;
using Scoopy.Extensions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Scoopy.ViewModels
{
    public class ScopeChannelVM : ReactiveObject, IActivatableViewModel
    {
        ScopeChannel Model { get; set; }
        ScopeChannel MockModel { get; set; }

        public ViewModelActivator Activator { get; }

        TelnetService Telnet => AppLocator.TelnetService;

        #region Scope Properties

        // is there a way to read this from the scope?
        public int ChannelNumber { get; set; }

        [Reactive] public string Name { get; set; }

        #region IsActive
        [Reactive] public bool IsActive { get; set; } = false;
        public ReactiveCommand<Unit, Unit> GetIsActiveCommand { get; }
        public ReactiveCommand<Unit, Unit> SetIsActiveCommand;
        [Reactive] public bool GetIsActiveSucceeded { get; set; }
        #endregion

        #region Coupling
        [Reactive] public string Coupling { get; set; }
        public ReactiveCommand<Unit, Unit> GetCouplingCommand { get; }
        public ReactiveCommand<Unit, Unit> SetCouplingCommand;
        [Reactive] public bool GetCouplingSucceeded { get; set; }
        #endregion

        #region Offset
        [Reactive]
        public double Offset { get; set; } = 0.0;
        public ReactiveCommand<Unit, Unit> GetOffsetCommand { get; }
        public ReactiveCommand<Unit, Unit> SetOffsetCommand;
        [Reactive] public bool GetOffsetSucceeded { get; set; }
        [Reactive] public string OffsetUnits { get; set; } = "V";
        #endregion

        #region Range
        [Reactive] public double Range { get; set; } = 0.0;
        public ReactiveCommand<Unit, Unit> GetRangeCommand { get; }
        public ReactiveCommand<Unit, Unit> SetRangeCommand;
        [Reactive] public bool GetRangeSucceeded { get; set; }
        #endregion

        #region Scale
        [Reactive] public double Scale { get; set; } = 0.0;
        public ReactiveCommand<Unit, Unit> GetScaleCommand { get; }
        public ReactiveCommand<Unit, Unit> SetScaleCommand;
        [Reactive] public bool GetScaleSucceeded { get; set; }
        #endregion

        #region Probe
        [Reactive] public string ProbeRatio { get; set; }
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
        [Reactive] public double TCal { get; set; } = 0.0;
        public ReactiveCommand<Unit, Unit> GetTCalCommand { get; }
        public ReactiveCommand<Unit, Unit> SetTCalCommand;
        [Reactive] public bool GetTCalSucceeded { get; set; }
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

            #region IsActive
            var canSetIsActive = this.WhenValueChanged(x => x.GetIsActiveSucceeded)
                .Where(x => x == true);
            GetIsActiveCommand = ReactiveCommand.CreateFromTask(SendIsActiveQueryAsync);
            SetIsActiveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await SendIsActiveCommandAsync();
            }, canSetIsActive);
            #endregion

            #region Coupling
            var canSetCoupling = this.WhenValueChanged(x => x.GetCouplingSucceeded)
                .Where(x => x == true);
            GetCouplingCommand = ReactiveCommand.CreateFromTask(SendCouplingQueryAsync);
            SetCouplingCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await SendCouplingCommandAsync();
            }, canSetCoupling);
            #endregion

            #region Offset
            var canSetOffset = this.WhenValueChanged(x => x.GetOffsetSucceeded)
                .Where(x => x == true);
            GetOffsetCommand = ReactiveCommand.CreateFromTask(SendOffsetQueryAsync);
            SetOffsetCommand = ReactiveCommand.CreateFromTask(SendOffsetCommandAsync, canSetOffset);

            // Offset units, based on Units
            this.WhenValueChanged(x => x.Units)
                .Subscribe(x =>
                    OffsetUnits = Units == "Volts" ? "V"
                        : Units == "Amps" ? "A"
                        : Units == "Watts" ? "W"
                        : "?");
            #endregion

            #region Range
            var canSetRange = this.WhenValueChanged(x => x.GetRangeSucceeded)
                .Where(x => x == true);
            GetRangeCommand = ReactiveCommand.CreateFromTask(SendRangeQueryAsync);
            SetRangeCommand = ReactiveCommand.CreateFromTask(SendRangeCommandAsync, canSetRange);
            #endregion

            #region Scale
            var canSetScale = this.WhenValueChanged(x => x.GetScaleSucceeded)
                .Where(x => x == true);
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
            var canSetTCal = this.WhenValueChanged(x => x.GetTCalSucceeded)
                .Where(x => x == true);
            GetTCalCommand = ReactiveCommand.CreateFromTask(SendTCalQueryAsync);
            SetTCalCommand = ReactiveCommand.CreateFromTask(SendTCalCommandAsync, canSetTCal);
            #endregion

            #region Units
            var canSetUnits = this.WhenValueChanged(x => x.GetUnitsSucceeded)
                .Where(x => x == true);
            GetUnitsCommand = ReactiveCommand.CreateFromTask(SendUnitsQueryAsync);
            SetUnitsCommand = ReactiveCommand.CreateFromTask(SendUnitsCommandAsync, canSetUnits);
            #endregion

            // watch our own properties and call commands that update the model

            //this.WhenPropertyChanged(x => x.IsActive)
            //    .InvokeCommand(SetIsActiveCommand);
            this.WhenActivated(disposables =>
            {
                this.HandleActivation();

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

                this.WhenValueChanged(x => x.TCal)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetTCalCommand)
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

                this.WhenValueChanged(x => x.Units)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetUnitsCommand)
                    .DisposeWith(disposables);

                this.WhenValueChanged(x => x.IsVernier)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetIsVernierCommand)
                    .DisposeWith(disposables);

            });
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
            await SendTCalQueryAsync();
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
            //this.Coupling = "GND";
            //GetCouplingSucceeded = true;
            //return;
            GetCouplingSucceeded = false;
            var response = await SendQueryAsync(ChannelSubcommand.COUPling);
            if (response == "") return;
            var result = StringOptions.Coupling.GetByParameter(response);
            if (result != null)
            {
                Model.Coupling = result.Value;
                this.Coupling = result.Value;
                GetCouplingSucceeded = true;
            }
            Log($"Coupling: {response}");
        }

        public async Task SendCouplingCommandAsync()
        {
            await SendCommandAsync(ChannelSubcommand.COUPling, this.Coupling.ToString());
        }
        #endregion

        #region Offset
        public async Task SendOffsetQueryAsync()
        {
            GetOffsetSucceeded = false;
#if MOCK
            var response = "0";
#else
            var response = await SendQueryAsync(ChannelSubcommand.OFFSet);
#endif
            if (response == "") return;
            if (double.TryParse(response, out double result))
            {
                Model.Offset = result;
                this.Offset = result;
                GetOffsetSucceeded = true;
            }
            Log($"Offset: {this.Offset}");
        }


        public async Task SendOffsetCommandAsync()
        {
            var value = this.Offset.ToString();
            await SendCommandAsync(ChannelSubcommand.OFFSet, value);
        }
#endregion

#region TCal
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
#endregion

#region Range
        public async Task SendRangeQueryAsync()
        {
            GetRangeSucceeded = false;
            var response = await SendQueryAsync(ChannelSubcommand.RANGe);
            if (response == "") return;
            if (double.TryParse(response, out double result))
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
            var response = await SendQueryAsync(ChannelSubcommand.SCALe);
            if (response == "") return;
            if (double.TryParse(response, out double result))
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
            var response = await SendQueryAsync(ChannelSubcommand.PROBe);
            if (response == "") return;
            if (double.TryParse(response, out double result))
            {
                var val = result.ToString();
                if (val.StartsWith("0")) val = val.Substring(1);
                var ratio = StringOptions.ProbeRatio.GetByValue(val + "x");
                Model.Probe = ratio.Value;
                this.ProbeRatio = ratio.Value;
                GetProbeSucceeded = true;
            }
            Log($"Probe: {this.ProbeRatio}");
        }


        public async Task SendProbeCommandAsync()
        {
            var option = StringOptions.ProbeRatio.GetByValue(this.ProbeRatio);
            await SendCommandAsync(ChannelSubcommand.PROBe, option.Parameter);
        }
#endregion

#region IsInverted
        public async Task SendIsInvertedQueryAsync()
        {
            GetIsInvertedSucceeded = false;
            var response = await SendQueryAsync(ChannelSubcommand.INVert);
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
            var response = await SendQueryAsync(ChannelSubcommand.BWLimit);
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
            var response = await SendQueryAsync(ChannelSubcommand.VERNier);
            if (response == "") return;
            var result = response == "1";
            this.IsVernier = result;
            GetIsVernierSucceeded = true;
            Log($"Vernier: {result}");
        }

        public async Task SendIsVernierCommandAsync()
        {
            var option = StringOptions.Vernier.GetByValue(this.Units);
            await SendCommandAsync(ChannelSubcommand.VERNier, option.Parameter);
        }
#endregion

#region Units
        public async Task SendUnitsQueryAsync()
        {
            GetUnitsSucceeded = false;
#if MOCK
            var response = "VOLT";
#else
            var response = await SendQueryAsync(ChannelSubcommand.UNITs);
#endif
            if (response == "") return;
            var result = StringOptions.Units.GetByParameter(response);
            if (result != null)
            {
                Model.Units = result.Value;
                this.Units = result.Value;
                GetUnitsSucceeded = true;
            }
            Log($"Units: {response}");
        }

        public async Task SendUnitsCommandAsync()
        {
            var option = StringOptions.Units.GetByValue(this.Units);
            await SendCommandAsync(ChannelSubcommand.UNITs, option.Parameter);
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


    }
}
