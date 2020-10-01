using ReactiveUI;
using Skippy.Extensions;
using Skippy.Protocols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Windows.Input;

namespace Skippy.ViewModels
{
    public class TimebaseVM : ReactiveObject, IActivatableViewModel, IProtocolVM
    {
        public ViewModelActivator Activator { get; }

        public TimebaseProtocol Protocol { get; set; }
        IProtocolCommand IProtocolVM.Protocol => Protocol;

        public ICommand GetAll { get; internal set; }

        public ICommand SetAll { get; internal set; }

        #region Properties

        private List<IScopeCommand> AllCommands { get; set; }

        public ScopeCommand<double> Offset { get; set; }
        public ScopeCommand<string> Scale { get; set; }
        public ScopeCommand<string> Mode { get; set; }

        #endregion Properties

        public TimebaseVM()
        {
            Activator = new ViewModelActivator();
            Protocol = new TimebaseProtocol(null);

            Offset = new ScopeCommand<double>(this, Protocol.Offset, "0.0");
            Scale = new ScopeCommand<string>(this, Protocol.Scale, "1.0000000e-06"); // 1us/div
            Scale.Value = "1u";
            Mode = new ScopeCommand<string>(this, Protocol.Mode, "MAIN");

            AllCommands = new List<IScopeCommand>()
            {
                Offset, Scale, Mode,
            };

            var GetAllMessage = ReactiveCommand.Create(() => 
                Debug.WriteLine("------- Retrieving all TIMEBASE values from device ---------"));
            GetAll = ReactiveCommand
                .CreateCombined(new[]
                {
                    // TODO: Should also get items that are dependencies of these
                    //   like sample rate, memory depth, etc.
                    GetAllMessage,
                    Offset.GetCommand,
                    Scale.GetCommand,
                    Mode.GetCommand,
                });

            var SetAllMessage = ReactiveCommand.Create(() =>
                Debug.WriteLine("------- Setting all TIMEBASE values on device ---------"));
            SetAll = ReactiveCommand.CreateCombined(new[]
            {
                SetAllMessage,
                Offset.SetCommand,
                Scale.SetCommand,
                Mode.SetCommand,
            });

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
