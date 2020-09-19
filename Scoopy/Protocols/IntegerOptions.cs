using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Scoopy.Protocols
{

    public class IntegerOption : IOption
    {
        public string Name { get; set; }
        public string Term { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public IntegerOption(int min, int max)
        {
            MinValue = min; MaxValue = max;
        }
    }

    public class IntegerOptions : Collection<IntegerOption>, IOptions, IEnumerable<IOption>
    {
        #region Implement ICollection<IOption>

        public bool IsReadOnly => false;

        public void Add(IOption item)
        {
            base.Add(item as IntegerOption);
        }

        public bool Contains(IOption item)
        {
            return base.Contains(item as IntegerOption);
        }

        public void CopyTo(IOption[] array, int arrayIndex)
        {
            base.CopyTo(array as IntegerOption[], arrayIndex);
        }

        public bool Remove(IOption item)
        {
            return base.Remove(item as IntegerOption);
        }

        IEnumerator<IOption> IEnumerable<IOption>.GetEnumerator()
        {
            return base.GetEnumerator();
        }

        #endregion Implement ICollection
    }

}
