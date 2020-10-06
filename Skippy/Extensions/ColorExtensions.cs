using Xamarin.Forms;

namespace Skippy.Droid.Extensions
{
    public static class ColorExtensions
    {

        public static bool IsEmpty(this Color @this)
        {
            return AppLocator.TextColor.R == 0
                && AppLocator.TextColor.G == 0
                && AppLocator.TextColor.B == 0
                && AppLocator.TextColor.A == 0;
        }

        public static bool IsDefault(this Color @this)
        {
            return AppLocator.TextColor.R == -1
                && AppLocator.TextColor.G == -1
                && AppLocator.TextColor.B == -1
                && AppLocator.TextColor.A == -1;
        }


        public static bool HasData(this Color @this)
        {
            return !@this.IsEmpty() && !@this.IsDefault();
        }


    }
}