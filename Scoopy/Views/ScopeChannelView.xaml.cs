using ReactiveUI;
using ReactiveUI.XamForms;
using Scoopy.Converters;
using Scoopy.Enums;
using Scoopy.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scoopy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScopeChannelView : ReactiveContentView<ScopeChannelVM>
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

        public StringOptions CouplingOptions => Enums.StringOptions.Coupling;

        public ScopeChannelView(ScopeChannelVM viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            this.BindingContext = viewModel;

            // initialize some stuff
            txtLabel.MainLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
            barCoupling.ItemsSource = StringOptions.Coupling;
            barUnits.ItemsSource = StringOptions.Units;
            barVernier.ItemsSource = StringOptions.Vernier;
            cboRatio.ItemsSource = StringOptions.ProbeRatio;

            this.WhenActivated(async disposable =>
            {
                this.Bind(ViewModel,
                    x => x.ChannelName,
                    x => x.txtLabel.Text)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.IsActive,
                    x => x.chkActive.IsOn)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.Coupling,
                    x => x.barCoupling.SelectedItems,
                    vmToViewConverterOverride: new StringOptionsToStringConverter())
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.IsInverted,
                    x => x.chkInverted.IsToggled)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.Offset,
                    x => x.txtOffset.Text)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.Range,
                    x => x.txtRange.Text)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.TCal,
                    x => x.txtTCal.Text)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.Scale,
                    x => x.txtScale.Text)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.Probe,
                    x => x.cboRatio.SelectedItem)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.Units,
                    x => x.barUnits.SelectedItems,
                    vmToViewConverterOverride: new StringOptionsToStringConverter())
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.Vernier,
                    x => x.barVernier.SelectedItems,
                    vmToViewConverterOverride: new StringOptionsToStringConverter())
                    .DisposeWith(disposable);

                // disable setting things until we get the current value from the scope
                this.Bind(ViewModel, x => x.GetIsActiveSucceeded, x => x.chkActive.IsEnabled);
                this.Bind(ViewModel, x => x.GetIsInvertedSucceeded, x => x.chkInverted.IsEnabled);
                this.Bind(ViewModel, x => x.GetOffsetSucceeded, x => x.txtOffset.IsEnabled);
                this.Bind(ViewModel, x => x.GetCouplingSucceeded, x => x.barCoupling.IsEnabled);
                this.Bind(ViewModel, x => x.GetRangeSucceeded, x => x.txtRange.IsEnabled);
                this.Bind(ViewModel, x => x.GetTCalSucceeded, x => x.txtTCal.IsEnabled);
                this.Bind(ViewModel, x => x.GetScaleSucceeded, x => x.txtScale.IsEnabled);
                this.Bind(ViewModel, x => x.GetProbeSucceeded, x => x.cboRatio.IsEnabled);
                this.Bind(ViewModel, x => x.GetUnitsSucceeded, x => x.barUnits.IsEnabled);
                this.Bind(ViewModel, x => x.GetVernierSucceeded, x => x.barVernier.IsEnabled);

                await viewModel.SendVernierQueryAsync();
                //await ViewModel.SendGetAllQuery();

                Debug.WriteLine($"barVernier: {barVernier.SelectedItems}");
            });

        }

        private void BarFine_SelectedItemsChanged(object sender, Controls.TogglesBarSelectionChangedEventArgs e)
        {
            { }
        }

        //private IReactiveBinding<ScopeChannelView, ScopeChannelVM, (object? view, bool isViewModel)> Bind
        //    (object viewModelProperty, object viewProperty)
        //    {
        //        this.Bind(ViewModel, x = )
        //    }

    }
}