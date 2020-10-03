using ReactiveUI;
using ReactiveUI.XamForms;
using Rg.Plugins.Popup.Services;
using Skippy.Controls;
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
using Xamarin.CustomControls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Skippy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScopeChannelView : AccordionItemView, IViewFor<ScopeChannelVM> // ReactiveContentView<ScopeChannelVM>
    {
        public class NameValue
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        public class Coupling : List<NameValue>
        {
            public void Add(string name, string value)
            {
                var item = new NameValue() { Name = name, Value = value };
                base.Add(item);
            }
        }

        public StringOptions CouplingOptions => Protocols.StringOptions.Coupling;

        public ScopeChannelVM ViewModel { get; set; }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as ScopeChannelVM; }

        public ScopeChannelView(ScopeChannelVM viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            this.BindingContext = viewModel;
            var protocol = ViewModel.Protocol;

            //this.LeftImage = FileImageSource.FromResource("Skippy.Resources.arrowRight.png");


            // initialize some stuff
            //txtLabel.MainLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
            uiCoupling.ItemsSource = StringOptions.Coupling.ToNames();
            uiUnits.ItemsSource = StringOptions.Units.ToNames();
            uiProbeRatio.ItemsSource = StringOptions.ProbeRatio.ToNames();

            this.WhenActivated(disposable =>
            {
                this.Click += (s,e) => { 
                    this.WidthRequest = IsOpen ? 300 : 100;
                };

                this.Bind(ViewModel,
                    x => x.Name,
                    x => x.Text)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                    x => x.Color,
                    x => x.ButtonBackgroundColor,
                    vmToViewConverterOverride: new StringToColorConverter())
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                    x => x.Color,
                    x => x.ButtonActiveBackgroundColor,
                    vmToViewConverterOverride: new StringToColorConverter())
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                    x => x.Color,
                    x => x.Frame.BorderColor,
                    vmToViewConverterOverride: new StringToColorConverter())
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                    x => x.Color,
                    x => x.uiIsActive.OnColor,
                    vmToViewConverterOverride: new StringToColorConverter())
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.Display.Value,
                    x => x.uiIsActive.IsOn)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.Coupling.Value,
                    x => x.uiCoupling.SelectedItems,
                    vmToViewConverterOverride: new StringOptionsToStringConverter())
                    .DisposeWith(disposable);
                uiCoupling.SelectedItemsChanged += (s, e) =>
                {
                    var selection = uiCoupling.SelectedItems;
                };

                #region Offset
                uiOffset.Events().Clicked
                    .SubscribeOnUI()
                    .Subscribe(async x =>
                    {
                        var popup = new PopSlider(
                            protocol.Offset.Name,
                            ViewModel.Offset.Value,
                            protocol.Offset.Options as RealOptions,
                            y =>
                            {
                                ViewModel.Offset.Value = y;
                            });
                        await PopupNavigation.Instance.PushAsync(popup);
                    });
                DecrementOffset.Events().Clicked
                    .Select(args => Unit.Default)
                    .InvokeCommand(ViewModel.Offset.Decrement);
                IncrementOffset.Events().Clicked
                    .Select(args => Unit.Default)
                    .InvokeCommand(ViewModel.Offset.Increment);
                #endregion

                #region Scale
                uiScale.Events().Clicked
                    .SubscribeOnUI()
                    .Subscribe(async x =>
                    {
                        var popup = new PopSlider(
                            protocol.Scale.Name,
                            ViewModel.Scale.Value,
                            protocol.Scale.Options as RealOptions,
                            y =>
                            {
                                ViewModel.Scale.Value = y;
                            });
                        await PopupNavigation.Instance.PushAsync(popup);
                    });
                DecrementScale.Events().Clicked
                    .Select(args => Unit.Default)
                    .InvokeCommand(ViewModel.Scale.Decrement);
                IncrementScale.Events().Clicked
                    .Select(args => Unit.Default)
                    .InvokeCommand(ViewModel.Scale.Increment);
                #endregion

                this.Bind(ViewModel,
                    vm => vm.Probe.Value,
                    view => view.uiProbeRatio.SelectedItem,
                    vmToViewConverterOverride: new StringOptionsToStringConverter())
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.BWLimit.Value,
                    x => x.uiIsBandwidthLimited.IsToggled)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.Invert.Value,
                    x => x.uiIsInverted.IsToggled)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.Vernier.Value,
                    x => x.uiIsVernier.IsToggled)
                    .DisposeWith(disposable);

#if TCAL
                this.Bind(ViewModel,
                    x => x.TCal,
                    x => x.uiTCal.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                    x => x.TCalUnits,
                    x => x.uiTCalUnits.Text)
                    .DisposeWith(disposable);
#endif

                this.Bind(ViewModel,
                    x => x.Units.Value,
                    x => x.uiUnits.SelectedItems,
                    vmToViewConverterOverride: new StringOptionsToStringConverter())
                    .DisposeWith(disposable);
                uiUnits.SelectedItemsChanged += (s, e) =>
                {
                    var selection = uiUnits.SelectedItems;
                };

                // disable setting things until we get the current value from the scope
                this.Bind(ViewModel, x => x.Display.GetSucceeded, x => x.uiIsActive.IsEnabled);
                this.Bind(ViewModel, x => x.Offset.GetSucceeded, x => x.uiOffset.IsEnabled);
                this.Bind(ViewModel, x => x.Coupling.GetSucceeded, x => x.uiCoupling.IsEnabled);
                this.Bind(ViewModel, x => x.Scale.GetSucceeded, x => x.uiScale.IsEnabled);
                this.Bind(ViewModel, x => x.Probe.GetSucceeded, x => x.uiProbeRatio.IsEnabled);
                this.Bind(ViewModel, x => x.BWLimit.GetSucceeded, x => x.uiIsBandwidthLimited.IsEnabled);
                this.Bind(ViewModel, x => x.Invert.GetSucceeded, x => x.uiIsInverted.IsEnabled);
                this.Bind(ViewModel, x => x.Vernier.GetSucceeded, x => x.uiIsVernier.IsEnabled);
#if TCAL
                this.Bind(ViewModel, x => x.GetTCalSucceeded, x => x.uiTCal.IsEnabled);
                this.Bind(ViewModel, x => x.GetTCalSucceeded, x => x.uiTCalUnits.IsEnabled);
#endif
                this.Bind(ViewModel, x => x.Units.GetSucceeded, x => x.uiUnits.IsEnabled);

                ViewModel.GetAll.Execute(null);

                //Debug.WriteLine($"barVernier: {barVernier.SelectedItems}");

                WireEvents(disposable);
            });

            this.Click += ScopeChannelView_OnClick;
        }

        private void ScopeChannelView_OnClick(object sender, AccordionItemClickEventArgs e)
        {
            { }
        }

        private void WireEvents(CompositeDisposable disposable)
        {
            SelectChannelButton
                .Events().Clicked
                .Select(args => Unit.Default)
                .InvokeCommand(this, x => x.ViewModel.SelectChannel)
                .DisposeWith(disposable);

            GetAllButton
                .Events().Clicked
                .Select(args => Unit.Default)
                .InvokeCommand(this, X => X.ViewModel.GetAll)
                .DisposeWith(disposable);

            SetAllButton
                .Events().Clicked
                .Select(args => Unit.Default)
                .InvokeCommand(this, X => X.ViewModel.SetAll)
                .DisposeWith(disposable);
        }

        

    }
}