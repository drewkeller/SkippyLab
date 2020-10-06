using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Skippy.Protocols
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

        #region Implement List<IOption>

        public int IndexOf(IOption option)
        {
            return base.IndexOf(option as IntegerOption);
        }

        public void Insert(int index, IOption option)
        {
            base.Insert(index, option as IntegerOption);
        }

        IOption IList<IOption>.this[int index]
        {
            get => base[index];
            set => base[index] = value as IntegerOption;
        }

        #endregion


        public object GetDecrementedValue(object currentValue)
        {
            var i = System.Convert.ToInt32(currentValue);
            return i + 1;
        }

        public object GetIncrementedValue(object currentValue)
        {
            var i = System.Convert.ToInt32(currentValue);
            return i - 1;
        }

    }

}
