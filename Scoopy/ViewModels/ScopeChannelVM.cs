using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Scoopy.Enums;
using Scoopy.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Scoopy.ViewModels
{
    public class ScopeChannelVM : ReactiveObject, IActivatableViewModel
    {
        ScopeChannel Model { get; set; }

        public ViewModelActivator Activator { get; }

        TelnetService Telnet => AppLocator.TelnetService;

        #region Scope Properties

        // is there a way to read this from the scope?
        public int ChannelNumber { get; set; }

        [Reactive] public string ChannelName { get; set; }

        [Reactive] public bool IsActive { get; set; } = false;
        public ReactiveCommand<Unit, Unit> GetIsActiveCommand { get; }
        public ReactiveCommand<Unit, Unit> SetIsActiveCommand;
        [Reactive] public bool GetIsActiveSucceeded { get; set; }

        [Reactive] public string Coupling { get; set; }
        public ReactiveCommand<Unit, Unit> GetCouplingCommand {get;}
        public ReactiveCommand<Unit, Unit> SetCouplingCommand;
        [Reactive] public bool GetCouplingSucceeded { get; set; }

        [Reactive] public bool IsInverted { get; set; } = false;
        public ReactiveCommand<Unit, Unit> GetIsInvertedCommand { get; }
        public ReactiveCommand<Unit, Unit> SetIsInvertedCommand;
        [Reactive] public bool GetIsInvertedSucceeded { get; set; }

        [Reactive] public double Offset { get; set; } = 0.0;
        public ReactiveCommand<Unit, Unit> GetOffsetCommand { get; }
        public ReactiveCommand<Unit, Unit> SetOffsetCommand;
        [Reactive] public bool GetOffsetSucceeded { get; set; }

        [Reactive] public double Range { get; set; } = 0.0;
        public ReactiveCommand<Unit, Unit> GetRangeCommand { get; }
        public ReactiveCommand<Unit, Unit> SetRangeCommand;
        [Reactive] public bool GetRangeSucceeded { get; set; }

        [Reactive] public double TCal { get; set; } = 0.0;
        public ReactiveCommand<Unit, Unit> GetTCalCommand { get; }
        public ReactiveCommand<Unit, Unit> SetTCalCommand;
        [Reactive] public bool GetTCalSucceeded { get; set; }

        [Reactive] public double Scale { get; set; } = 0.0;
        public ReactiveCommand<Unit, Unit> GetScaleCommand { get; }
        public ReactiveCommand<Unit, Unit> SetScaleCommand;
        [Reactive] public bool GetScaleSucceeded { get; set; }

        [Reactive] public StringOption Probe { get; set; }
        public ReactiveCommand<Unit, Unit> GetProbeCommand { get; }
        public ReactiveCommand<Unit, Unit> SetProbeCommand;
        [Reactive] public bool GetProbeSucceeded { get; set; }

        [Reactive] public string Units { get; set; }
        public ReactiveCommand<Unit, Unit> GetUnitsCommand { get; }
        public ReactiveCommand<Unit, Unit> SetUnitsCommand;
        [Reactive] public bool GetUnitsSucceeded { get; set; }

        [Reactive] public string Vernier { get; set; }
        public ReactiveCommand<Unit, Unit> GetVernierCommand { get; }
        public ReactiveCommand<Unit, Unit> SetVernierCommand;
        [Reactive] public bool GetVernierSucceeded { get; set; }

        #endregion Scope Properties


        public ScopeChannelVM(int channelNumber)
        {
            Activator = new ViewModelActivator();

            Model = new ScopeChannel();

            ChannelNumber = channelNumber;
            ChannelName = $"CH{channelNumber}";

            #region IsActive
            var canSetIsActive = this.WhenValueChanged(x => x.GetIsActiveSucceeded)
                .Where(x => x == true);
            GetIsActiveCommand = ReactiveCommand.CreateFromTask(SendIsActiveQueryAsync);
            SetIsActiveCommand = ReactiveCommand.CreateFromTask(async () => {
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

            #region IsInverted
            var canSetIsInverted = this.WhenValueChanged(x => x.GetIsInvertedSucceeded)
                .Where(x => x == true);
            GetIsInvertedCommand = ReactiveCommand.CreateFromTask(SendIsInvertedQueryAsync);
            SetIsInvertedCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await SendIsInvertedCommandAsync();
            }, canSetIsInverted);
            #endregion

            #region Offset
            var canSetOffset = this.WhenValueChanged(x => x.GetOffsetSucceeded)
                .Where(x => x == true);
            GetOffsetCommand = ReactiveCommand.CreateFromTask(SendOffsetQueryAsync);
            SetOffsetCommand = ReactiveCommand.CreateFromTask(SendOffsetCommandAsync, canSetOffset);
            #endregion

            #region Range
            var canSetRange = this.WhenValueChanged(x => x.GetRangeSucceeded)
                .Where(x => x == true);
            GetRangeCommand = ReactiveCommand.CreateFromTask(SendRangeQueryAsync);
            SetRangeCommand = ReactiveCommand.CreateFromTask(SendRangeCommandAsync, canSetRange);
            #endregion

            #region TCal
            var canSetTCal = this.WhenValueChanged(x => x.GetTCalSucceeded)
                .Where(x => x == true);
            GetTCalCommand = ReactiveCommand.CreateFromTask(SendTCalQueryAsync);
            SetTCalCommand = ReactiveCommand.CreateFromTask(SendTCalCommandAsync, canSetTCal);
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

            #region Units
            var canSetUnits = this.WhenValueChanged(x => x.GetUnitsSucceeded)
                .Where(x => x == true);
            GetUnitsCommand = ReactiveCommand.CreateFromTask(SendUnitsQueryAsync);
            SetUnitsCommand = ReactiveCommand.CreateFromTask(SendUnitsCommandAsync, canSetUnits);
            #endregion

            #region Vernier
            var canSetVernier = this.WhenValueChanged(x => x.GetVernierSucceeded)
                .Where(x => x == true);
            GetVernierCommand = ReactiveCommand.CreateFromTask(SendVernierQueryAsync);
            SetVernierCommand = ReactiveCommand.CreateFromTask(SendVernierCommandAsync, canSetVernier);
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

                this.WhenPropertyChanged(x => x.IsInverted)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetIsInvertedCommand)
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

                this.WhenValueChanged(x => x.Probe)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetProbeCommand)
                    .DisposeWith(disposables);

                this.WhenValueChanged(x => x.Units)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetUnitsCommand)
                    .DisposeWith(disposables);

                this.WhenValueChanged(x => x.Vernier)
                    .ToSignal()
                    .InvokeCommand(this, x => x.SetVernierCommand)
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
            await SendIsInvertedQueryAsync();
            await SendOffsetQueryAsync();
            await SendCouplingQueryAsync();
            await SendRangeQueryAsync();
            await SendTCalQueryAsync();
            await SendScaleQueryAsync();
            await SendProbeQueryAsync();
            await SendUnitsQueryAsync();
            await SendVernierQueryAsync();
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
            var response = await SendQueryAsync(ChannelSubcommand.DISPlay);
            if (response == "") return;
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
            if (Enum.TryParse<Coupling>(response, out var result))
            {
                Model.Coupling = result;
                this.Coupling = result.ToString();
                GetCouplingSucceeded = true;
            }
            Log($"Coupling: {response}");
        }

        public async Task SendCouplingCommandAsync()
        {
            await SendCommandAsync(ChannelSubcommand.COUPling, this.Coupling.ToString());
        }
        #endregion

        #region Inverted
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

        #region Offset
        public async Task SendOffsetQueryAsync()
        {
            GetOffsetSucceeded = false;
            var response = await SendQueryAsync(ChannelSubcommand.OFFSet);
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
                Model.DelayCalibrationTime = result;
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
                Model.ProbeRatio = result;
                this.Probe = StringOptions.ProbeRatio.GetByValue(val + "x");
                GetProbeSucceeded = true;
            }
            Log($"Probe: {this.Probe}");
        }


        public async Task SendProbeCommandAsync()
        {
            var value = this.Probe.Parameter;
            await SendCommandAsync(ChannelSubcommand.PROBe, value);
        }
        #endregion

        #region Units
        public async Task SendUnitsQueryAsync()
        {
            GetUnitsSucceeded = false;
            var response = await SendQueryAsync(ChannelSubcommand.UNITs);
            if (response == "") return;
            var result = StringOptions.Units.GetByParameter(response);
            if (result != null)
            {
                Model.Units = (ChannelUnits)Enum.Parse(typeof(ChannelUnits), result.Value);
                this.Units = result.ToString();
                GetUnitsSucceeded = true;
            }
            Log($"Units: {response}");
        }

        public async Task SendUnitsCommandAsync()
        {
            await SendCommandAsync(ChannelSubcommand.UNITs, this.Units);
        }
        #endregion

        #region Vernier
        public async Task SendVernierQueryAsync()
        {
            GetVernierSucceeded = false;
            var response = await SendQueryAsync(ChannelSubcommand.VERNier);
            if (response == "") return;
            var result = response == "1";
            Model.FineAdjust = result;
            this.Vernier = result == true ? "Fine" : "Coarse";
            GetVernierSucceeded = true;
            Log($"Vernier: {response}");
        }

        public async Task SendVernierCommandAsync()
        {
            var param = this.Vernier == "Fine";
            await SendCommandAsync(ChannelSubcommand.VERNier, param);
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
