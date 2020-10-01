using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Skippy.Extensions;
using Skippy.Protocols;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Skippy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopSliderView : PopupPage, IActivatableView
    {
        public const double SliderMargin = 15.0;

        [Reactive] public string SelectedItem { get; set; }

        [Reactive] public ObservableCollection<string> Items {get;set;}

        public PopSliderView(string name, string value, StringOptions options, Action<string> returnValue)
        {

            InitializeComponent();
            this.BindingContext = options;

            Items = new ObservableCollection<string>(options.ToNames());

            this.WhenActivated((Action<Action<IDisposable>>)(disposables =>
            {
                this.nameLabel.Text = name;
                var values = options.ToNames();
                // x is the count, so we need -1 to get the index
                if (values.Count < 1)
                    throw new InvalidOperationException("The slider requires a set of values");
                if (!values.Contains((string)value))
                    throw new InvalidOperationException($"The slider values doesn't contain the set value '{value}'");
                this.slider.Maximum = values.Count - 1;
                this.slider.Minimum = 0;
                this.slider.Value = values.IndexOf((string)value);
                DrawSliderLabel();
            }));

            OKButton.Events().Clicked
                .Subscribe(x => PopupNavigation.Instance.PopAsync());

            IncrementButton.Events().Clicked
                .Subscribe(x =>
                {
                    if (slider.Value < Items.Count) slider.Value++;
                });


            DecrementButton.Events().Clicked
                .Subscribe(x =>
                {
                    if (slider.Value > 0) slider.Value--;
                });

            slider.Events().ValueChanged
                .SubscribeOnUI()
                .Subscribe(X => 
                {
                    SetToNearestIndex();
                    DrawSliderLabel();
                    returnValue(SelectedItem);
                });

            this.Events().LayoutChanged
                .SubscribeOnUI()
                .Subscribe(x => {
                    if (slider.Width > 0)
                    {
                        DrawTicks();
                        DrawSliderLabel();
                    }
                });
        }

        private void SetToNearestIndex()
        {
            var index = (int)Math.Round(slider.Value);
            slider.Value = index;
            SelectedItem = Items[index];
        }

        private void DrawSliderLabel()
        {
            var values = Items;

            // round to the nearest index
            var index = (int)Math.Round(slider.Value);
            slider.Value = index;
            var val = values[index];
            //Debug.WriteLine($"slider: {slider.Value}, index: {index}, val: {val}");

            var label = valueLabel;
            valueLabel.Text = $"{val}";

            // calculate total width available for each label
            var padding = SliderMargin / DeviceDisplay.MainDisplayInfo.Density;
            var sliderWidth = slider.Width;
            if (Device.RuntimePlatform == Device.Android)
            {
                // why do we need to do this on Android and not on UWP... ?
                sliderWidth -= 2 * SliderMargin / DeviceDisplay.MainDisplayInfo.Density;
            }
            var targetWidth = sliderWidth / (values.Count - 1);

            // padding is set to 15 on custom renderer
            var halfWidth = label.Width / 2.0;
            labelGrid.Margin = new Thickness(padding, 0, padding, 0); // padding doesn't affect things drawn inside?
            var positionX = index * targetWidth;
            positionX = positionX + padding - halfWidth;
            if (positionX < padding) positionX = padding;
            Debug.WriteLine($"slider: {slider.Value}, index: {index}, labelWidth: {label.Width} positionX: {positionX}");
            if (positionX + halfWidth >= (slider.Width - padding))
            {
                positionX = slider.Width - padding - halfWidth;
            }
            if (index == Items.Count - 1 && Device.RuntimePlatform == Device.Android)
            {
                positionX = Math.Min(positionX, slider.Width - padding - label.Width);
            }
            label.TranslateTo(positionX, 0, 10);
        }

        private bool drawing;
        private void DrawTicks()
        {
            if (drawing) return;
            drawing = true;

            var values = Items;

            var tickWidth = 1.0;
            var tickHeight = 10.0;

            // calculate total width available for each label
            var sliderWidth = slider.Width;
            if (Device.RuntimePlatform == Device.Android)
            {
                // why do we need to do this on Android and not on UWP... ?
                sliderWidth -= 2 * SliderMargin / DeviceDisplay.MainDisplayInfo.Density;
            }
            var targetWidth = sliderWidth / (values.Count - 1);
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
            var values = Items;
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