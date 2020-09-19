using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Scoopy.Protocols
{
    public class RealOption : IOption
    {
        public string Name { get; set; }
        public string Term { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public RealOption(double min, double max)
        {
            MinValue = min; MaxValue = max;
        }
    }

    public class RealOptions : Collection<RealOption>, IOptions, IEnumerable<IOption>
    {
        #region Implement ICollection<IOption>

        public bool IsReadOnly => false;

        public void Add(IOption item)
        {
            base.Add(item as RealOption);
        }

        public bool Contains(IOption item)
        {
            return base.Contains(item as RealOption);
        }

        public void CopyTo(IOption[] array, int arrayIndex)
        {
            base.CopyTo(array as RealOption[], arrayIndex);
        }

        public bool Remove(IOption item)
        {
            return base.Remove(item as RealOption);
        }

        IEnumerator<IOption> IEnumerable<IOption>.GetEnumerator()
        {
            return base.GetEnumerator();
        }

        #endregion Implement ICollection
    }

}
