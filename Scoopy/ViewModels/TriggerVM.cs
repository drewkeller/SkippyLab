using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Scoopy.Protocols;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Scoopy.ViewModels
{

    public class TriggerVM : ReactiveObject, IActivatableViewModel, IProtocolVM
    {

        public ViewModelActivator Activator { get; }

        public RootProtocol RootProtocol { get; set; }

        public TriggerProtocol Protocol { get; set; }
        IProtocolCommand IProtocolVM.Protocol => Protocol as IProtocolCommand;

        public ICommand GetAll { get; internal set; }

        public ICommand SetAll { get; internal set; }


        #region Properties

        private List<IScopeCommand> AllScopeCommands { get; set; }

        public ScopeCommand<string> AutoScale { get; set; }
        public ScopeCommand<string> Clear { get; set; }
        public ScopeCommand<string> Run { get; set; }
        public ScopeCommand<string> Stop { get; set; }
        public ScopeCommand<string> SingleTrigger { get; set; }
        public ScopeCommand<string> ForceTrigger { get; set; }

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
            RootProtocol = new RootProtocol();

            AutoScale = new ScopeCommand<string>(this, RootProtocol.AutoScale);
            Clear = new ScopeCommand<string>(this, RootProtocol.Clear);
            Run = new ScopeCommand<string>(this, RootProtocol.Run);
            Stop = new ScopeCommand<string>(this, RootProtocol.Stop);
            SingleTrigger = new ScopeCommand<string>(this, RootProtocol.Single);
            ForceTrigger = new ScopeCommand<string>(this, RootProtocol.Force);

            Mode = new ScopeCommand<string>(this, Protocol.Mode, "EDGE");
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

                this.WhenAnyValue(x => x.Mode.Value, (x) => x == "Edge")
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
