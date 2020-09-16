﻿using ReactiveUI;
using ReactiveUI.XamForms;
using Scoopy.Converters;
using Scoopy.Enums;
using Scoopy.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
            //txtLabel.MainLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
            uiCoupling.ItemsSource = StringOptions.Coupling; //.Select(x => x.Value);
            uiUnits.ItemsSource = StringOptions.Units; //.Select(x => x.Value);
            uiProbeRatio.ItemsSource = StringOptions.ProbeRatio;

            this.WhenActivated(disposable =>
            {
                this.Bind(ViewModel,
                    x => x.Name,
                    x => x.txtLabel.Text)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.IsActive,
                    x => x.uiIsActive.IsOn)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.Coupling,
                    x => x.uiCoupling.SelectedItems,
                    vmToViewConverterOverride: new StringOptionsToStringConverter())
                    .DisposeWith(disposable);
                uiCoupling.SelectedItemsChanged += (s,e) => {
                    var selection = uiCoupling.SelectedItems;
                };

                this.Bind(ViewModel,
                    x => x.Offset,
                    x => x.uiOffset.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                    x => x.OffsetUnits,
                    x => x.uiOffsetUnits.Text)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.Range,
                    x => x.uiRange.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                    x => x.RangeUnits,
                    x => x.uiRangeUnits.Text)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.Scale,
                    x => x.uiScale.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                    x => x.ScaleUnits,
                    x => x.uiScaleUnits.Text)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.ProbeRatio,
                    x => x.uiProbeRatio.SelectedItem)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.IsBandwidthLimited,
                    x => x.uiIsBandwidthLimited.IsToggled)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.IsInverted,
                    x => x.uiIsInverted.IsToggled)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.IsVernier,
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
                    x => x.Units,
                    x => x.uiUnits.SelectedItems,
                    vmToViewConverterOverride: new StringOptionsToStringConverter())
                    .DisposeWith(disposable);
                uiUnits.SelectedItemsChanged += (s, e) => {
                    var selection = uiUnits.SelectedItems;
                };

                // disable setting things until we get the current value from the scope
                this.Bind(ViewModel, x => x.GetIsActiveSucceeded, x => x.uiIsActive.IsEnabled);
                this.Bind(ViewModel, x => x.GetOffsetSucceeded, x => x.uiOffset.IsEnabled);
                this.Bind(ViewModel, x => x.GetCouplingSucceeded, x => x.uiCoupling.IsEnabled);
                this.Bind(ViewModel, x => x.GetRangeSucceeded, x => x.uiRange.IsEnabled);
                this.Bind(ViewModel, x => x.GetRangeSucceeded, x => x.uiRangeUnits.IsEnabled);
                this.Bind(ViewModel, x => x.GetScaleSucceeded, x => x.uiScale.IsEnabled);
                this.Bind(ViewModel, x => x.GetScaleSucceeded, x => x.uiScaleUnits.IsEnabled);
                this.Bind(ViewModel, x => x.GetProbeSucceeded, x => x.uiProbeRatio.IsEnabled);
                this.Bind(ViewModel, x => x.GetIsBandwidthLimitedSucceeded, x => x.uiIsBandwidthLimited.IsEnabled);
                this.Bind(ViewModel, x => x.GetIsInvertedSucceeded, x => x.uiIsInverted.IsEnabled);
                this.Bind(ViewModel, x => x.GetIsVernierSucceeded, x => x.uiIsVernier.IsEnabled);
#if TCAL
                this.Bind(ViewModel, x => x.GetTCalSucceeded, x => x.uiTCal.IsEnabled);
                this.Bind(ViewModel, x => x.GetTCalSucceeded, x => x.uiTCalUnits.IsEnabled);
#endif
                this.Bind(ViewModel, x => x.GetUnitsSucceeded, x => x.uiUnits.IsEnabled);

                // width of units columns should all be the same
                this.WhenAnyValue(x => x.uiScaleUnits.Width)
                    .Subscribe(x =>
                    {
                        var width = uiScaleUnits.Width;
                        uiOffsetUnits.WidthRequest = width;
                        uiRangeUnits.WidthRequest = width;
                    });

                //await viewModel.SendVernierQueryAsync();
                //await viewModel.SendIsBandwidthLimitedQueryAsync();
                //await viewModel.SendOffsetQueryAsync();
                //await ViewModel.SendUnitsQueryAsync();
                //await ViewModel.SendGetAllQuery();
                ViewModel.RefreshChannel.Execute(null);

                //Debug.WriteLine($"barVernier: {barVernier.SelectedItems}");

                WireEvents(disposable);
            });

        }

        private void WireEvents(CompositeDisposable disposable)
        {
            SelectChannelButton
                .Events().Clicked
                .Select(args => Unit.Default)
                .InvokeCommand(this, x => x.ViewModel.SelectChannel)
                .DisposeWith(disposable);

            RefreshChannelButton
                .Events().Clicked
                .Select(args => Unit.Default)
                .InvokeCommand(this, X => X.ViewModel.RefreshChannel);
        }

    }
}