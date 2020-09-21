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

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Skippy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TriggerView : ReactiveContentView<TriggerVM>
    {

        public TriggerView()
        {
            InitializeComponent();
            ViewModel = new TriggerVM();

            this.BindingContext = ViewModel;

            var protocol = ViewModel.Protocol;

            // initialize some stuff
            //txtLabel.MainLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
            Mode.ItemsSource = StringOptions.GetStringValues(protocol.Mode.Options);
            EdgeSource.ItemsSource = StringOptions.GetStringValues(protocol.Edge.Source.Options);
            EdgeSlope.ItemsSource = StringOptions.GetStringValues(protocol.Edge.Slope.Options);

            this.WhenActivated(disposable =>
            {
                this.Bind(ViewModel,
                    vm => vm.Status.Value,
                    v => v.Status.Text)
                    .DisposeWith(disposable);

                this.BindToProperty(ViewModel,
                    vm => vm.Mode.Value,
                    vm => vm.Mode.GetSucceeded,
                    v => v.Mode.SelectedItem,
                    v => v.Mode.IsEnabled,
                    vmToViewConverterOverride: new StringOptionsToStringConverter(),
                    disposable);

                #region Edge panel

                this.Bind(ViewModel, vm => vm.IsEdgeMode, v => v.EdgeModePanel.IsVisible);

                this.Bind(ViewModel,
                    x => x.EdgeSource.Value,
                    x => x.EdgeSource.SelectedItem,
                    vmToViewConverterOverride: new StringOptionsToStringConverter())
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                    x => x.EdgeSource.GetSucceeded,
                    x => x.EdgeSource.IsEnabled)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.EdgeSlope.Value,
                    x => x.EdgeSlope.SelectedItems,
                    vmToViewConverterOverride: new StringOptionsToStringConverter())
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                    x => x.EdgeSlope.GetSucceeded,
                    x => x.EdgeSlope.IsEnabled)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.EdgeLevel.Value,
                    x => x.EdgeLevel.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                    x => x.EdgeLevel.GetSucceeded,
                    x => x.EdgeLevel.IsEnabled)
                    .DisposeWith(disposable);

                #endregion Edge panel

                ViewModel.GetAll.Execute(null);
                WireEvents(disposable);
            });
        }

        private void WireEvents(CompositeDisposable disposable)
        {
            GetAllButton
                .Events().Clicked.Select(args => Unit.Default)
                .InvokeCommand(ViewModel.GetAll)
                .DisposeWith(disposable);

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