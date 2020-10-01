using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Skippy.Protocols
{
    public class BooleanOption : IOption
    {
        public string Name { get; set; }
        public string Term { get; set; }

        public BooleanOption() { }

        /// <summary>
        /// Create a boolean option
        /// </summary>
        /// <param name="term"></param>
        /// <param name="name"></param>
        public BooleanOption(string name, string term)
        {
            Name = name;
            Term = term;
        }

        public override string ToString()
        {
            //return $"{ID}: {Value}";
            return $"{Name}";
        }
    }


    public class BooleanOptions : Collection<BooleanOption>, IOptions
    {
        public static readonly BooleanOptions Boolean = new BooleanOptions
        {
            new BooleanOption() { Name = "ON", Term = "1" },
            new BooleanOption() { Name = "OFF", Term = "0" },
        };

        public static readonly BooleanOptions BWLimit = new BooleanOptions
        {
            new BooleanOption("20M", "20M"),
            new BooleanOption("OFF", "OFF")
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

        public object GetIncrementedValue(object currentValue)
        {
            var val = currentValue.ToString();
            if (val == "OFF" || val == "0")
            {
                return "ON";
            }
            return null;
        }

        public object GetDecrementedValue(object currentValue)
        {
            var val = currentValue.ToString();
            if (val == "ON" || val == "1")
            {
                return "OFF";
            }
            return null;
        }

        public BooleanOptions()
        {
            this.Add(new BooleanOption() { Name = "ON", Term = "1" });
            this.Add(new BooleanOption() { Name = "OFF", Term = "0" });
        }
    }

}
