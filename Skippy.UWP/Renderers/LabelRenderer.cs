using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using Windows.UI.Xaml.Media;

[assembly: ExportRenderer(typeof(Label), typeof(Skippy.Droid.Renderers.MyLabelRenderer))]
namespace Skippy.Droid.Renderers
{
    public class MyLabelRenderer : LabelRenderer
    {
        public MyLabelRenderer() : base()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null && Control != null)
            {
                var textColor = AppLocator.TextColor;
                if (textColor.R == 0 && textColor.G == 0 && textColor.B == 0 && textColor.A == 0)
                {
                    var brush = Control.Foreground as Windows.UI.Xaml.Media.SolidColorBrush;
                    var color = brush.Color;
                    AppLocator.TextColor = Xamarin.Forms.Color.FromRgba(color.R, color.G, color.B, color.A);
                }
            }
        }
    }
}