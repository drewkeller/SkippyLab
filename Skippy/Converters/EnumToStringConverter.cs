using ReactiveUI;
using System;

namespace Skippy.Converters
{
    public class EnumToStringConverter : IBindingTypeConverter
    {

        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            if (fromType == typeof(Coupling))
            {
                return 100;
            }
            if (fromType == typeof(Enum))
            {
                return 100;
            }
            if (fromType == typeof(string))
            {
                return 100;
            }
            if (fromType == typeof(int))
            {
                return 1;
            }
            return 0;
        }

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            result = null;
            if (toType == typeof(string))
            {
                result = from.ToString();
                return true;
            }
            if (toType == typeof(Enum))
            {
                // compiler doesn't recognize this...?
                //if (Enum.TryParse(toType, from.ToString(), out var val))
                result = Enum.Parse(toType, from.ToString());
                return true;
            }
            return false;
        }
    }
}
