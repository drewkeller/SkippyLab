using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Skippy.Extensions;
using Skippy.Protocols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Skippy.ViewModels
{

    public class TriggerVM : ReactiveObject, IActivatableViewModel, IProtocolVM
    {

        public ViewModelActivator Activator { get; }

        public TriggerProtocol Protocol { get; set; }
        IProtocolCommand IProtocolVM.Protocol => Protocol as IProtocolCommand;

        public ICommand GetAll { get; internal set; }

        public ICommand SetAll { get; internal set; }


        #region Properties

        private List<IScopeCommand> AllScopeCommands { get; set; }

        public ScopeCommand<string> Mode { get; set; }

        #region Edge mode properties
        public extern bool IsEdgeMode { [ObservableAsProperty]get; }
        public ScopeCommand<string> EdgeSource { get; set; }
        public ScopeCommand<string> EdgeSlope { get; set; }
        public ScopeCommand<double> EdgeLevel { get; set; }
        #endregion Edge mode properties

        #endregion Properties


        public TriggerVM()
        {
            Activator = new ViewModelActivator();
            Protocol = new TriggerProtocol(null);

            Mode = new ScopeCommand<string>(this, Protocol.Mode, nameof(ModeStringOptions.Edge));
            EdgeSource = new ScopeCommand<string>(this, Protocol.Edge.Source, "CHAN1");
            EdgeSlope = new ScopeCommand<string>(this, Protocol.Edge.Slope, "POS");
            EdgeLevel = new ScopeCommand<double>(this, Protocol.Edge.Level, "0");

            AllScopeCommands = new List<IScopeCommand>()
            {
                Mode, EdgeSource, EdgeSlope, EdgeLevel,
            };

            var GetAllMessage = ReactiveCommand.Create(() =>
                Debug.WriteLine("------- Retrieving all TRIGGER values from device ---------"));
            GetAll = ReactiveCommand.CreateCombined(new[]
            {
                GetAllMessage,
                Mode.GetCommand,
                EdgeSource.GetCommand,
                EdgeSlope.GetCommand,
                EdgeLevel.GetCommand,
            });

            var SetAllMessage = ReactiveCommand.Create(() =>
                Debug.WriteLine("------- Setting all TRIGGER values on device ---------"));
            SetAll = ReactiveCommand.CreateCombined(new[]
            {
                SetAllMessage,
                Mode.SetCommand,
                EdgeSource.SetCommand,
                EdgeSlope.SetCommand,
                EdgeLevel.SetCommand,
            });


            this.WhenActivated(disposables =>
            {
                this.HandleActivation();

                Disposable
                    .Create(() => this.HandleDeactivation())
                    .DisposeWith(disposables);

                foreach (var scopeCommand in AllScopeCommands)
                {
                    scopeCommand.WhenActivated(disposables);
                }

                this.WhenAnyValue(x => x.Mode.Value, (x) => x == nameof(ModeStringOptions.Edge))
                    .ToPropertyEx(this, x => x.IsEdgeMode);
            });

        }

        private void HandleActivation()
        {
        }

        private void HandleDeactivation()
        {
        }


    }

}
