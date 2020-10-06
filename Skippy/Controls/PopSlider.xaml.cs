using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Skippy.Extensions;
using Skippy.Protocols;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Skippy.Controls
{
    public enum StepMode { None, Normal, Discrete }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopSlider : PopupPage, IActivatableView
    {
        #region Properties

        public const double SliderMargin = 15.0;
        private ObservableCollection<string> TickItems { get; set; }

        /// <summary>
        /// Only valid if using <seealso cref="StringOptions"/>.
        /// </summary>
        [Reactive] public string SelectedItem { get; set; }

        /// <summary>
        /// Only valid if using <seealso cref="StringOptions"/>.
        /// </summary>
        [Reactive] public ObservableCollection<string> Items {get;set;}

        /// <summary>
        /// Only valid if using <seealso cref="RealOptions"/>
        /// </summary>
        [Reactive] public double Value { get; set; }

        public double Step { get; set; } = 0;

        private StepMode StepMode { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a slider with discrete string options. Enables use of
        /// <seealso cref="Items"/> and <seealso cref="SelectedItem"/>.
        /// </summary>
        /// <param name="name">Name that appears as the title</param>
        /// <param name="value">Initial value of the slider</param>
        /// <param name="options">The <seealso cref="IOption.Name"/>s 
        /// of this are used to set <seealso cref="SelectedItem"/></param>
        /// <param name="returnValue"></param>
        public PopSlider(string name, string value, StringOptions options, Action<string> returnValue)
        {
            InitializeComponent();
            this.BindingContext = options;
            InitializeDiscreteSteps(name, value, options.ToNames());
            WireEvents(returnValue, null);
        }

        /// <summary>
        /// Creates a slider that has a <seealso cref="double"/> <seealso cref="Value"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="options">This is used to set the slider min/max and (optional) step</param>
        /// <param name="returnValue"></param>
        public PopSlider(string name, double value, RealOptions options, Action<double> returnValue)
        {
            var option = options[0] as RealOption;

            InitializeComponent();
            this.BindingContext = options;

            this.BackgroundFrame.BackgroundColor = AppLocator.BackgroundColor;
            this.BackgroundFrame.Opacity = 0.9;
            this.measurementLabel.TextColor = this.BackgroundColor;
            this.measurementLabel.Opacity = .001;

            if (option.Steps == null)
            {
                InitializeNotDiscrete(name, value, option.MinValue, option.MaxValue, option.Step);
            }
            else
            {
                InitializeDiscreteSteps(name, value, option.Steps.Select(x => x.ToSIString()));
            }

            WireEvents(null, returnValue);
        }

        #endregion Constructors

        #region Initialization

        private void InitializeNotDiscrete(string name, double value, double min, double max, double step)
        {
            StepMode = step == 0 ? StepMode.None : StepMode.Normal;
            TickItems = new ObservableCollection<string>() { $"{min}", $"{max}" };
            Step = step;

            this.WhenActivated((Action<Action<IDisposable>>)(disposables =>
            {
            }));
                this.nameLabel.Text = name;
                this.slider.Maximum = max;
                this.slider.Minimum = min;
                this.slider.Value = value;
                DrawSliderLabel();
        }

        private void InitializeDiscreteSteps(string name, object value, IEnumerable<string> items)
        {
            StepMode = StepMode.Discrete;
            Items = new ObservableCollection<string>(items);
            TickItems = Items;
            Step = 1; // step on index

            if (Items.Count < 1)
                throw new InvalidOperationException("The slider requires a set of values");
            if (value is double dbl)
            {
                if (!Items.Contains(dbl.ToSIString()))
                {
                    throw new InvalidOperationException($"The slider values doesn't contain the set value '{value}'");
                }
            }
            else if (!Items.Contains(value.ToString()))
            {
                throw new InvalidOperationException($"The slider values doesn't contain the set value '{value}'");
            }

            this.WhenActivated((Action<Action<IDisposable>>)(disposables =>
            {
                this.nameLabel.Text = name;
                this.slider.Maximum = Items.Count - 1;
                this.slider.Minimum = 0;
                this.slider.Value = Items.IndexOf(value.ToString());
                DrawSliderLabel();
            }));
        }

        private void WireEvents(Action<string> returnString, Action<double> returnDouble) 
        {

            OKButton.Events().Clicked
                .Subscribe(x => PopupNavigation.Instance.PopAsync());

            IncrementButton.Events().Clicked
                .Subscribe(x =>
                {
                    if (StepMode == StepMode.Discrete)
                    {
                        if (slider.Value < Items.Count) slider.Value++;
                    } 
                    else if (StepMode == StepMode.Normal)
                    {
                        if (slider.Value + Step <= slider.Maximum) slider.Value += Step;
                    } else
                    {
                        // change by .1 percent
                        var increment = GetFriendlyIncrement();
                        var newValue = slider.Value + increment;
                        slider.Value = Math.Round(newValue / increment) * increment;
                    }
                });


            DecrementButton.Events().Clicked
                .Subscribe(x =>
                {
                    if (StepMode == StepMode.Discrete)
                    {
                        if (slider.Value < Items.Count) slider.Value--;
                    } 
                    else if (StepMode == StepMode.Normal)
                    {
                        if (slider.Value - Step >= slider.Minimum) slider.Value -= Step;
                    } else
                    {
                        // change by .1 percent
                        var increment = GetFriendlyIncrement();
                        var newValue = slider.Value - increment;
                        slider.Value = Math.Round(newValue / increment) * increment;
                    }
                });

            slider.Events().ValueChanged
                .SubscribeOnUI()
                //.Throttle(TimeSpan.FromMilliseconds(1))
                .Subscribe(X => 
                {
                    SetToNearestStep();
                    DrawSliderLabel();
                    if (returnDouble != null && double.TryParse(SelectedItem, out var dbl))
                        returnDouble(dbl);
                    else if (returnString != null)
                        returnString((string)SelectedItem);
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

        #endregion Initialization

        #region Step calculations

        private void SetToNearestStep()
        {
            if (Step == 0 || StepMode == StepMode.None)
            {
                if (slider.Maximum - slider.Minimum > 10)
                {
                    slider.Value = Math.Round(slider.Value, 10);
                    return;
                }
                slider.Value = (double)Math.Round((decimal)slider.Value, 13);
                //var places = GetDecimalPlaces(slider.Value);
                return;
            }

            var steppedValue = GetNearestStep();
            slider.Value = steppedValue;
            if (StepMode == StepMode.Discrete)
            {
                SelectedItem = Items[(int)steppedValue];
            } 
        }

        private double GetNearestStep()
        {
            if (Step == 0 || StepMode == StepMode.None)
            {
                if (slider.Maximum - slider.Minimum > 10)
                    return Math.Round(slider.Value, 10);
                return Math.Round(slider.Value, 13);
            }
            return (double)Math.Round((decimal)slider.Value / (decimal)Step) * Step;
        }

        /// <summary>
        /// Gets the number of decimal places.
        /// Sometimes the slider value (Android only?) has strange values, like
        /// 970.999999999 or 970.000000000001
        /// https://stackoverflow.com/questions/13477689/find-number-of-decimal-places-in-decimal-value-regardless-of-culture
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int GetDecimalPlaces(double n)
        {
            n = Math.Abs(n); //make sure it is positive.
            n -= (int)n;     //remove the integer part of the number.
            var decimalPlaces = 0;
            while (n > 0)
            {
                decimalPlaces++;
                n *= 10;
                n -= (int)n;
            }
            return decimalPlaces;
        }

        #endregion Step calculations

        private void DrawSliderLabel()
        {
            var values = TickItems;
            var value = GetNearestStep();
            //var places = GetDecimalPlaces(slider.Value);

            if (StepMode == StepMode.Discrete)
            {
                var index = GetNearestStep();
                //slider.Value = index;
                var val = values[(int)index];
                valueLabel.Text = $"{val}";
                //Debug.WriteLine($"slider: {slider.Value}, index: {index}, val: {val}");
            } else
            {
                valueLabel.Text = $"{value}";
            }

            // calculate total width available for each label
            var padding = SliderMargin / DeviceDisplay.MainDisplayInfo.Density;
            var sliderWidth = slider.Width;
            if (Device.RuntimePlatform == Device.Android)
            {
                // why do we need to do this on Android and not on UWP... ?
                sliderWidth -= 2 * SliderMargin / DeviceDisplay.MainDisplayInfo.Density;
            }
            var position = Math.Abs((value - slider.Minimum) / (slider.Maximum - slider.Minimum) * sliderWidth);
            var targetWidth = StepMode == StepMode.Discrete ? sliderWidth / (values.Count - 1) : 1;

            // padding is set to 15 on custom renderer
            var label = valueLabel;
            var halfWidth = label.Width / 2.0;
            labelGrid.Margin = new Thickness(padding, 0, padding, 0); // padding doesn't affect things drawn inside?
            var positionX = position + padding - halfWidth;
            if (positionX < padding) positionX = padding;
            //Debug.WriteLine($"slider: {slider.Value}, index: {index}, labelWidth: {label.Width} positionX: {positionX}");
            if (positionX + halfWidth >= (slider.Width - padding))
            {
                positionX = slider.Width - padding - halfWidth;
            }
            if (StepMode == StepMode.Discrete && (int)position == TickItems.Count - 1 && Device.RuntimePlatform == Device.Android)
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

            var values = TickItems;

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
                    TextColor = OKButton.TextColor,
                    //BackgroundColor = Color.DarkGray,
                };
                var tick = new BoxView
                {
                    //TextColor = (Color)Application.Current.Resources["TextPrimaryColor"],
                    Color = OKButton.TextColor,
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
            var values = TickItems;
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

        private double GetFriendlyIncrement()
        {
            var per = .01;
            var range = Math.Abs(slider.Maximum - slider.Minimum);
            if (range >= 1000) return per * 1000;
            if (range >= 100) return per * 100;
            if (range >= 10) return per * 10;
            if (range >= 1) return per * 1;
            if (range >= .1) return per * .1;
            if (range >= .01) return per * .01;
            if (range >= .001) return per * .001;
            if (range >= .0001) return per * .0001;
            if (range >= .00001) return per * .00001;
            if (range >= .000001) return per * .000001;
            if (range >= .0000001) return per * .0000001;
            if (range >= .00000001) return per * .00000001;
            if (range >= .000000001) return per * .000000001;
            if (range >= .0000000001) return per * .0000000001;
            if (range >= .00000000001) return per * .00000000001;
            return per * .000000000001;
        }

    }
}