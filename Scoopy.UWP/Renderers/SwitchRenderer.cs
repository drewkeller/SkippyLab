using Scoopy.UWP;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Switch), typeof(CustomSwitchViewRenderer))]

namespace Scoopy.UWP
{
    /// <summary>
    /// Fixes too much padding in UWP implementation of Switch.
    /// See https://forums.xamarin.com/discussion/172376/right-align-switch-rendering-in-uwp-not-correct
    /// </summary>
    public class CustomSwitchViewRenderer : SwitchRenderer
    {
        public CustomSwitchViewRenderer() : base()
        { }

        protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control != null)
                {
                    // Eliminate text for on and off states
                    Control.OffContent = string.Empty;
                    Control.OnContent = string.Empty;

                    // Hack to force 'end' alignment of UWP switch
                    //var alignment = e.NewElement.HorizontalOptions.Alignment;
                    Control.Margin = new Windows.UI.Xaml.Thickness { Right = -110 };
                }
            }
        }


    }
}