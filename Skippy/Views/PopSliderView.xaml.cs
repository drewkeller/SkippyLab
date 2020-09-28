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
            var padding = 2;
            labelGrid.Padding = padding;

            var tickWidth = 2.0;
            var tickHeight = 10.0;

            // calculate total width available for each label
            // use margin at each end of the slider to make the ends appear
            // at the middle of the first and last labels
            var targetWidth = labelGrid.Width / (values.Count-1);
            //targetWidth -= 2 * padding;
            //slider.Margin = new Thickness(targetWidth / 2, 0, targetWidth / 2, 0);
            var rotation = GetRotation(targetWidth);
            var alignment = rotation == 0 ? TextAlignment.Center : TextAlignment.Start;

            labelGrid.Children.Clear();

            for (int index = 0; index < values.Count; index++)
            {
                // add a label
                var label = new Label {
                    Text = values[index],
                    HorizontalTextAlignment = alignment,
                    VerticalTextAlignment = alignment,
                    RotationX = measurementLabel.Height / 2,
                    RotationY = 0.0,
                    Rotation = rotation,
                    LineBreakMode = LineBreakMode.NoWrap,
                    //TextColor = (Color)Application.Current.Resources["TextPrimaryColor"],
                    TextColor = Color.White,
                    //BackgroundColor = Color.DarkGray,
                };
                var tick = new BoxView
                {
                    //TextColor = (Color)Application.Current.Resources["TextPrimaryColor"],
                    Color = Color.White,
                };
                var positionX = index * targetWidth;
                labelGrid.Children.Add(label, new Point(positionX - measurementLabel.Height/2, labelGrid.Height - measurementLabel.Height - tickHeight - 10));
                labelGrid.Children.Add(tick, new Rectangle(positionX - tickWidth/2, labelGrid.Height - tickHeight, tickWidth, tickHeight));
                // labeGrid apparently doesn't calculate its height correctly for rotated labels
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