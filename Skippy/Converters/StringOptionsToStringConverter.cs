using ReactiveUI;
using Skippy.Protocols;
using System;

namespace Skippy.Converters
{
    public class StringOptionsToStringConverter : IBindingTypeConverter
    {

        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            if (fromType == typeof(StringOptions))
            {
                return 100;
            }
            return 0;
        }

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            result = null;

            if (toType == typeof(string))
            {
                result = from?.ToString();
                return true;
            }
            if (toType == typeof(StringOption))
            {
                if (from is string value)
                {
                    var option = StringOptions.GetAnyByValue(value);
                    return option != null;
                }

            }
            if (toType == typeof(Enum))
            {
                // compiler doesn't recognize this...?
                //if (Enum.TryParse(toType, from.ToString(), out var val))
                result = Enum.Parse(toType, from.ToString());
                return true;
            }
            if (toType == typeof(object))
            {
                result = from?.ToString();
                return true;
            }
            return false;
        }
    }
}
