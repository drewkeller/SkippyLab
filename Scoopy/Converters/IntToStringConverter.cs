using ReactiveUI;
using System;

namespace Scoopy.Converters
{
    public class IntToStringConverter : IBindingTypeConverter
    {

        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            if (fromType == typeof(int))
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
                result = from?.ToString();
                return true;
            }
            if (toType == typeof(int))
            {
                if (int.TryParse(from?.ToString(), out var val))
                {
                    result = val;
                    return true;
                }
            }
            return false;
        }
    }
}
