using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Scoopy.Protocols
{
    public class BooleanOption : IOption
    {
        public string Name { get; set; }
        public string Term { get; set; }
    }

    public class BooleanOptions : Collection<BooleanOption>, IOptions
    {
        public static readonly BooleanOptions Boolean = new BooleanOptions
        {
            new BooleanOption() { Name = "ON", Term = "1" },
            new BooleanOption() { Name = "OFF", Term = "0" },
        };

        #region Implement ICollection<IOption>

        public bool IsReadOnly => false;

        public void Add(IOption item)
        {
            base.Add(item as BooleanOption);
        }

        public bool Contains(IOption item)
        {
            return base.Contains(item as BooleanOption);
        }

        public void CopyTo(IOption[] array, int arrayIndex)
        {
            base.CopyTo(array as BooleanOption[], arrayIndex);
        }

        public bool Remove(IOption item)
        {
            return base.Remove(item as BooleanOption);
        }

        IEnumerator<IOption> IEnumerable<IOption>.GetEnumerator()
        {
            return base.GetEnumerator();
        }

        #endregion Implement ICollection

        public BooleanOptions()
        {
            this.Add(new BooleanOption() { Name = "ON", Term = "1" });
            this.Add(new BooleanOption() { Name = "OFF", Term = "0" });
        }
    }

}
