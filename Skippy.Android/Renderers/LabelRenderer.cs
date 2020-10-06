using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Label), typeof(Skippy.Droid.Renderers.MyLabelRenderer))]
namespace Skippy.Droid.Renderers
{
    public class MyLabelRenderer : LabelRenderer
    {
        public MyLabelRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            var color = Control.CurrentTextColor;
            if (AppLocator.TextColor.R == -1)
            {
                AppLocator.TextColor = Xamarin.Forms.Color.FromUint((uint)color);
            }
        }
    }
}