using ReactiveUI;
using System;

namespace Scoopy.Converters
{
    public class DoubleToStringConverter : IBindingTypeConverter
    {

        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            if (fromType == typeof(double))
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
            if (fromType == typeof(float))
            {
                return 1;
            }
            if (fromType == typeof(decimal))
            {
                return 1;
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
                result = from.ToString();
                return true;
            }
            if (toType == typeof(double))
            {
                if (double.TryParse(from.ToString(), out var val))
                {
                    result = val;
                    return true;
                }
            }
            return false;
        }
    }
}
