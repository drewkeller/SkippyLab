using ReactiveUI;
using ReactiveUI.XamForms;
using Skippy.Converters;
using Skippy.Extensions;
using Skippy.Protocols;
using Skippy.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CustomControls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Skippy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RunView : AccordionItemView, IViewFor<RunVM>
    {
        public RunVM ViewModel { get; set; }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as RunVM; }

        public RunView()
        {
            InitializeComponent();
            ViewModel = new RunVM();

            var protocol = ViewModel.Protocol;

            this.WhenActivated(disposable =>
            {
                this.Bind(ViewModel,
                    vm => vm.Status.Value,
                    v => v.Status.Text)
                    .DisposeWith(disposable);

            });

        }

        private void WireEvents(CompositeDisposable disposable)
        {
            ClearButton
                .Events().Clicked.Select(args => Unit.Default)
                .InvokeCommand(ViewModel.Clear.SetCommand)
                .DisposeWith(disposable);

            RunButton
                .Events().Clicked.Select(args => Unit.Default)
                .InvokeCommand(ViewModel.Run.SetCommand)
                .DisposeWith(disposable);

            StopButton
                .Events().Clicked.Select(args => Unit.Default)
                .InvokeCommand(ViewModel.Stop.SetCommand)
                .DisposeWith(disposable);

            SingleTriggerButton
                .Events().Clicked.Select(args => Unit.Default)
                .InvokeCommand(ViewModel.SingleTrigger.SetCommand)
                .DisposeWith(disposable);

            ForceTriggerButton
                .Events().Clicked.Select(args => Unit.Default)
                .InvokeCommand(ViewModel.ForceTrigger.SetCommand)
                .DisposeWith(disposable);
        }

    }
}