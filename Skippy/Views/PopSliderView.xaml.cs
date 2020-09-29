using Rg.Plugins.Popup.Pages;
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
using Skippy.Extensions;
using Xamarin.Essentials;

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
                ViewModel.WhenAnyValue(x => x.Values, y => y.Value)
                .SubscribeOn(RxApp.MainThreadScheduler)
                .Subscribe( _ =>
                {
                    var values = ViewModel.Values;
                    var value = ViewModel.Value;
                    // x is the count, so we need -1 to get the index
                    if (values.Count < 1)
                        throw new InvalidOperationException("The slider requires a set of values");
                    if (!values.Contains(value))
                        throw new InvalidOperationException($"The slider values doesn't contain the set value '{value}'");
                    this.slider.Maximum = values.Count - 1;
                    this.slider.Minimum = 0;
                    this.slider.Value = values.IndexOf(value);
                    this.valueLabel.Text = $"{value}";
                })
                .DisposeWith(disposables);

            });
            WireEvents();
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
                    //Debug.WriteLine($"slider: {slider.Value}, index: {index}, val: {val}");

                    var label = valueLabel;
                    valueLabel.Text = $"{val}";

                    // padding is set to 15 on custom renderer
                    var halfWidth = label.Width / 2.0;
                    var padding = 15.0 / DeviceDisplay.MainDisplayInfo.Density;
                    labelGrid.Margin = new Thickness(padding, 0, padding, 0); // padding doesn't affect things drawn inside?
                    var positionX = index / slider.Maximum * (slider.Width - 2 * padding);
                    positionX = positionX + padding - halfWidth;
                    if (positionX < padding) positionX = padding;
                    Debug.WriteLine($"slider: {slider.Value}, index: {index}, labelWidth: {label.Width} positionX: {positionX}");
                    if (positionX + halfWidth >= (slider.Width - padding))
                    {
                        positionX = slider.Width - padding - halfWidth;
                    }
                    if (index == ViewModel.Values.Count - 1)
                    {
                        positionX = Math.Min(positionX, slider.Width - padding - label.Width);
                    }
                    label.TranslateTo(positionX, 0, 10);
                });

            //slider.Events().SizeChanged
            this.Events().LayoutChanged
            //this.Events().Appearing
                .SubscribeOnUI()
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

            var tickWidth = 1.0;
            var tickHeight = 10.0;

            // calculate total width available for each label
            // use margin at each end of the slider to make the ends appear
            // at the middle of the first and last labels
            var targetWidth = slider.Width / (values.Count - 1);
            //targetWidth -= 2 * padding;
            //slider.Margin = new Thickness(targetWidth / 2, 0, targetWidth / 2, 0);
            var rotation = GetRotation(targetWidth);

            labelGrid.Children.Clear();

            for (int index = 0; index < values.Count; index++)
            {
                //var rotation = index == 3 ? -90.0 : 0.0;
                var alignment = rotation == 0 ? TextAlignment.Center : TextAlignment.Start;

                // add a label
                var label = new Label {
                    Text = values[index],
                    HorizontalTextAlignment = alignment,
                    VerticalTextAlignment = alignment,
                    Rotation = rotation,
                    AnchorX = rotation == 0 ? 0.5 : 0.0,
                    AnchorY = 0.5,
                    LineBreakMode = LineBreakMode.NoWrap,
                    //TextColor = (Color)Application.Current.Resources["TextPrimaryColor"],
                    TextColor = Color.FromHex("#FFFFFF"),
                    //BackgroundColor = Color.DarkGray,
                };
                var tick = new BoxView
                {
                    //TextColor = (Color)Application.Current.Resources["TextPrimaryColor"],
                    Color = Color.FromHex("#FFFFFF"),
                };
                var labelHeight = measurementLabel.Height;
                var positionX = index * targetWidth;
                var positionY = labelGrid.Height - tickHeight;

                //positionX = rotation == 0 ? positionX : positionX - measurementLabel.Height / 2;

                // add tick mark
                labelGrid.Children.Add(tick, new Rectangle(positionX - tickWidth/2, positionY, tickWidth, tickHeight));

                // add text label
                labelGrid.Children.Add(label, new Point(positionX, positionY - labelHeight)); // offset above tick mark
            }

            drawing = false;
        }

        // see if we need to rotate the labels based on
        // the size of first and last items
        private double GetRotation(double targetWidth)
        {
            var values = ViewModel.Values;
            var first = MeasureString(values[0]);
            var last = MeasureString(values[values.Count - 1]);
            var larger = Math.Max(first, last);
            if (larger > targetWidth)
                return -90.0;
            else
                return 0.0;
        }

        private double MeasureString(string text)
        {
            measurementLabel.Text = text;
            return MeasureString(measurementLabel);
        }

        private double MeasureString(Label label)
        {
            var measurement = label.Measure(int.MaxValue, int.MaxValue);
            var width = measurement.Request.Width;
            //var height = measurement.Request.Height;
            //if (width > labelGrid.Height)
            //{
            //    labelGrid.HeightRequest = width;
            //}
            return width;
        }

    }
}