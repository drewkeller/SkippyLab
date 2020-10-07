using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.XamForms;
using Rg.Plugins.Popup.Services;
using Skippy.Controls;
using Skippy.Converters;
using Skippy.Extensions;
using Skippy.Protocols;
using Skippy.ViewModels;
using Splat;
using System;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.CustomControls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Skippy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimebaseView : AccordionItemView, IViewFor<TimebaseVM>
    {
        public TimebaseVM ViewModel { get; set; }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as TimebaseVM; }

        public TimebaseView()
        {
            InitializeComponent();
            ViewModel = Locator.Current.GetService<TimebaseVM>();

            var protocol = ViewModel.Protocol;

            // initialize some stuff
            Mode.ItemsSource = StringOptions.ToNames(protocol.Mode.Options);

            this.WhenActivated(disposable =>
            {
                #region Offset

                //this.BindToProperty(ViewModel,
                //    vm => vm.Offset.Value, 
                //    vm => vm.Offset.GetSucceeded,
                //    v => v.Offset.Text, 
                //    v => v.Offset.IsEnabled, 
                //    disposable);
                this.Bind(ViewModel, x => x.Offset.IsEnabled, x => x.Offset.IsEnabled);

                Offset.Events().Clicked
                    .SubscribeOnUI()
                    .Subscribe(async x =>
                    {
                        popup = new PopSlider(
                            protocol.Offset.Name,
                            ViewModel.Offset.Value,
                            protocol.Offset.Options as RealOptions,
                            y => { ViewModel.Offset.Value = y; });
                        await PopupNavigation.Instance.PushAsync(popup);
                    });

                DecrementOffset.Events().Clicked
                    .Select(args => Unit.Default)
                    .InvokeCommand(ViewModel.Offset.Decrement);

                IncrementOffset.Events().Clicked
                    .Select(args => Unit.Default)
                    .InvokeCommand(ViewModel.Offset.Increment);

                #endregion Offset

                #region Scale
                this.Bind(ViewModel,
                    x => x.Scale.IsEnabled,
                    x => x.uiScale.IsEnabled)
                    .DisposeWith(disposable);

                uiScale.Events().Clicked
                    .SubscribeOnUI()
                    .Subscribe(async x =>
                    {
                        popup = new PopSlider(
                            protocol.Scale.Name,
                            ViewModel.Scale.Value,
                            protocol.Scale.Options as RealOptions,
                            y => { ViewModel.Scale.Value = y; });
                        await PopupNavigation.Instance.PushAsync(popup);
                    });
                DecrementScale.Events().Clicked
                    .Select(args => Unit.Default)
                    .InvokeCommand(ViewModel.Scale.Decrement);
                IncrementScale.Events().Clicked
                    .Select(args => Unit.Default)
                    .InvokeCommand(ViewModel.Scale.Increment);
                #endregion

                this.BindToProperty(ViewModel,
                    vm => vm.Mode.Value, 
                    vm => vm.Mode.IsEnabled,
                    v => v.Mode.SelectedItems, 
                    v => v.Mode.IsEnabled,
                    vmToViewConverterOverride: new StringOptionsToStringConverter(),
                    disposable);
                //this.Bind(ViewModel,
                //    x => x.Mode.Value,
                //    x => x.Mode.SelectedItems,
                //    vmToViewConverterOverride: new StringOptionsToStringConverter())
                //    .DisposeWith(disposable);
                //this.Bind(ViewModel,
                //    x => x.Mode.GetSucceeded,
                //    x => x.Mode.IsEnabled)
                //    .DisposeWith(disposable);

                ViewModel.GetAll.Execute(null);
                WireEvents(disposable);
            });

        }

        PopSlider popup;

        private void WireEvents(CompositeDisposable disposable)
        {
            GetAllButton.Events().Clicked
                .Select(args => Unit.Default)
                .InvokeCommand(ViewModel.GetAll)
                .DisposeWith(disposable);

            SetAllButton.Events().Clicked
                .Select(args => Unit.Default)
                .InvokeCommand(ViewModel.SetAll)
                .DisposeWith(disposable);

        }
    }

}