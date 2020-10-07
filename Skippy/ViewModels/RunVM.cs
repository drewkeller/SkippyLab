using ReactiveUI;
using Skippy.Extensions;
using Skippy.Protocols;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace Skippy.ViewModels
{
    public class RunVM : ReactiveObject, IActivatableViewModel, IProtocolVM
    {
        public ViewModelActivator Activator { get; set; }

        public RootProtocol Protocol { get; set; }
        IProtocolCommand IProtocolVM.Protocol => Protocol;

        private List<IScopeCommand> AllScopeCommands { get; set; }


        #region Properties

        public ScopeCommand<string> AutoScale { get; set; }
        public ScopeCommand<string> Clear { get; set; }
        public ScopeCommand<string> Run { get; set; }
        public ScopeCommand<string> Stop { get; set; }
        public ScopeCommand<string> SingleTrigger { get; set; }
        public ScopeCommand<string> ForceTrigger { get; set; }

        public ScopeCommand<string> Status { get; set; }

        #endregion Properties

        public RunVM()
        {
            Activator = new ViewModelActivator();
            Protocol = new RootProtocol();

            AutoScale = new ScopeCommand<string>(this, Protocol.AutoScale);
            Clear = new ScopeCommand<string>(this, Protocol.Clear);
            Run = new ScopeCommand<string>(this, Protocol.Run);
            Stop = new ScopeCommand<string>(this, Protocol.Stop);
            SingleTrigger = new ScopeCommand<string>(this, Protocol.Single);
            ForceTrigger = new ScopeCommand<string>(this, Protocol.Force);

            Status = new ScopeCommand<string>(this, Protocol.Status, "AUTO");

            AllScopeCommands = new List<IScopeCommand>
            {
                AutoScale, Clear, Run, Stop, SingleTrigger, ForceTrigger,
            };

            this.WhenActivated(disposables =>
            {
                this.HandleActivation();

                Disposable
                    .Create(() => this.HandleDeactivation())
                    .DisposeWith(disposables);

                var statusTimer = Observable.Interval(TimeSpan.FromMilliseconds(1000))
                    .ToSignal()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .InvokeCommand(Status.GetCommand);

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
