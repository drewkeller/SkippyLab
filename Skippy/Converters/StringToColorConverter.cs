using ReactiveUI;
using System;

namespace Skippy.Converters
{
    public class StringToColorConverter : IBindingTypeConverter
    {

        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            if (fromType == typeof(Xamarin.Forms.Color))
            {
                return 100;
            }
            if (fromType == typeof(string))
            {
                return 100;
            }
            return 0;
        }

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            if (from == null)
            {
                { }
            }
            result = null;
            if (toType == typeof(string))
            {
                if (from is Xamarin.Forms.Color color)
                {
                    var str = color.ToHex();
                    result = str;
                    return true;
                }
            }
            if (toType == typeof(Xamarin.Forms.Color))
            {
                if (from is string str)
                {
                    if (str.StartsWith("#"))
                    {
                        var color = Xamarin.Forms.Color.FromHex(from?.ToString());
                        result = color;
                        return true;
                    }
                    // ....
                }
            }
            return false;
        }
    }
}
