using ReactiveUI;
using Scoopy.Enums;
using System;

namespace Scoopy.Converters
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
