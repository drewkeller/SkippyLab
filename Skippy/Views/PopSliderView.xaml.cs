﻿using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reactive.Disposables;
using Rg.Plugins.Popup.Services;
using ReactiveUI.Fody.Helpers;
using DynamicData.Binding;
using System.Reactive.Linq;
using Skippy.ViewModels;
using Skippy.Converters;
using System.Net;
using System.Diagnostics;
using Skippy.Interfaces;

namespace Skippy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopSliderView : PopupPage, IActivatableView, IViewFor<PopupSliderVM>
    {
        public PopupSliderVM ViewModel { get; set; }

        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as PopupSliderVM; }

        public PopSliderView()
        {

            InitializeComponent();

            ViewModel = new PopupSliderVM();
            this.BindingContext = ViewModel;

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, 
                    vm => vm.Name,
                    v => v.nameLabel.Text);

                // maximum always needs to be higher than the minimum
                ViewModel.WhenAnyValue(x => x.Values.Count, y => y.Value)
                .SubscribeOn(RxApp.MainThreadScheduler)
                .Subscribe( _ =>
                {
                    var values = ViewModel.Values;
                    var value = ViewModel.Value;
                    // x is the count, so we need -1 to get the index
                    if (values.Count < 1)
                        throw new InvalidOperationException("The slider requires a set of values");
                    if (!values.Contains(value))
                        throw new InvalidOperationException($"The slider doesn't recognize value '{value}'");
                    this.slider.Maximum = values.Count - 1;
                    this.slider.Minimum = 0;
                    this.slider.Value = values.IndexOf(value);
                    this.valueLabel.Text = $"{value}";
                })
                .DisposeWith(disposables);

                WireEvents();
            });
        }

        private void WireEvents()
        {
            OKButton.Events().Clicked
                .Subscribe(x => PopupNavigation.Instance.PopAsync());

            slider.Events().ValueChanged
                .Subscribe(X =>
                {
                    // round to the nearest index
                    var index = (int)Math.Round(slider.Value);
                    slider.Value = index;
                    var val = ViewModel.Values[index];
                    Debug.WriteLine($"slider: {slider.Value}, index: {index}, val: {val}");
                    valueLabel.Text = $"{val}";
                });

            slider.Events().SizeChanged
                .Subscribe(x => {
                    if (slider.Width > 0) DrawTicks();
                });

        }

        private bool drawing;
        private void DrawTicks()
        {
            if (drawing) return;
            drawing = true;

            var values = ViewModel.Values;

            // calculate total width available for each label
            // use margin at each end of the slider to make the ends appear
            // at the middle of the first and last labels
            var targetWidth = labelGrid.Width / (values.Count-1);
            sliderGrid.Margin = new Thickness(targetWidth / 2, 0, targetWidth / 2, 0);

            labelGrid.Children.Clear();

            for (int index = 0; index < values.Count; index++)
            {
                // add a label
                var label = new Label {
                    Text = values[index],
                    HorizontalTextAlignment = TextAlignment.Center,
                    LineBreakMode = LineBreakMode.NoWrap,
                    //BackgroundColor = Color.DarkGray,
                };
                labelGrid.Children.Add(label);
            }

            drawing = false;
        }

    }
}