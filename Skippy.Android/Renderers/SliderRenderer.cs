using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Slider), typeof(Skippy.Droid.Renderers.NoMarginSliderRenderer))]
namespace Skippy.Droid.Renderers
{
    public class NoMarginSliderRenderer : SliderRenderer
    {
        public NoMarginSliderRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
        {
            base.OnElementChanged(e);
            Control.SetPadding(15, 0, 15, 0);
        }
    }
}