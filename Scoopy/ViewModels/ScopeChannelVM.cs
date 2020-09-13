using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Scoopy.ViewModels
{
    public class ScopeChannelVM : ReactiveObject
    {
        ScopeChannel Model { get; set; }

        TelnetService Telnet => AppLocator.TelnetService;

        #region Scope Properties

        // is there a way to read this from the scope?
        public int ChannelNumber { get; set; }

        [Reactive] public string ChannelName { get; set; }

        [Reactive] public bool IsActive { get; set; }
        public ReactiveCommand<Unit,Unit> GetIsActiveCommand { get; }
        public ReactiveCommand<Unit,Unit> SetIsActiveCommand { get; }

        [Reactive] public bool IsInverted { get; set; }
        public ReactiveCommand<Unit,Unit> GetIsInvertedCommand { get; }
        public ReactiveCommand<Unit,Unit> SetIsInvertedCommand { get; }

        [Reactive] public double Offset { get; set; }

        #endregion Scope Properties


        public ScopeChannelVM(int channelNumber)
        {
            Model = new ScopeChannel();

            ChannelNumber = channelNumber;
            ChannelName = $"CH{channelNumber}";

            GetIsActiveCommand = ReactiveCommand.CreateFromTask(SendIsActiveQueryAsync);
            SetIsActiveCommand = ReactiveCommand.CreateFromTask(SendIsActiveCommandAsync);

            GetIsInvertedCommand = ReactiveCommand.CreateFromTask(SendIsActiveQueryAsync);
            SetIsInvertedCommand = ReactiveCommand.CreateFromTask(SendIsActiveCommandAsync);

            // watch our own properties and call commands that update the model

            //this.WhenPropertyChanged(x => x.IsActive)
            //    .InvokeCommand(SetIsActiveCommand);

            Model.WhenPropertyChanged(x => x.Name)
                .ToProperty(this, nameof(ChannelName));

            Model.WhenPropertyChanged(x => x.IsActive)
                .ToProperty(this, nameof(IsActive));

            Model.WhenPropertyChanged(x => x.Inverted)
                .ToProperty(this, nameof(IsInverted));
        }

        #region Scope commands

        public async Task SendGetAllQuery()
        {
            await SendIsActiveQueryAsync();
            await SendIsInvertedQueryAsync();
        }

        #region IsActive
        public async Task SendIsActiveQueryAsync()
        {
            var response = await SendQueryAsync(ChannelSubcommand.DISPlay);
            Model.IsActive = response == "1";
        }

        public async Task SendIsActiveCommandAsync()
        {
            await SendCommandAsync(ChannelSubcommand.DISPlay, Model.IsActive);
        }
        #endregion

        #region Inverted
        public async Task SendIsInvertedQueryAsync()
        {
            var response = await SendQueryAsync(ChannelSubcommand.INVert);
            Model.Inverted = response == "ON";
            Log($"Inverted: {Model.Inverted}");
        }

        public async Task SendIsInvertedCommandAsync()
        {
            await SendCommandAsync(ChannelSubcommand.INVert, Model.Inverted);
        }
        #endregion

        #region Offset
        public async Task SendOffsetQueryAsync()
        {
            var response = await SendQueryAsync(ChannelSubcommand.OFFSet);
            if (double.TryParse(response, out double result))
            {
                Model.Offset = result;
            }
            Log($"Offset: {Model.Offset}");
        }

        private void Log(string message)
        {
            Debug.WriteLine($"CH{Model.ChannelNumber} {message}");
        }


        public async Task SendOffsetCommandAsync()
        {
            var value = Model.Offset.ToString();
            await SendCommandAsync(ChannelSubcommand.OFFSet, value);
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

        #endregion // command helpers


    }
}
