﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Skippy.Protocols
{
    /// <summary>
    /// Provides information for displaying a value and sending the value via SCPI.
    /// </summary>
    public class StringOption : IOption
    {
        /// <summary>
        /// The text to use when getting or sending the value via SCPI.
        /// </summary>
        public string Term { get; set; }

        /// <summary>
        /// The human-readable value
        /// </summary>
        public string Name { get; set; }

        public StringOption() { }

        /// <summary>
        /// Create a string option
        /// </summary>
        /// <param name="term"></param>
        /// <param name="name"></param>
        public StringOption(string name, string term)
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

    /// <summary>
    /// Provides ways to manipulate values for a command (increment,
    /// decrement, etc).
    /// </summary>
    public class StringOptions : ObservableCollection<StringOption>, IOptions
    {
        public static readonly List<StringOptions> All = new List<StringOptions> {
            Coupling, Units, Vernier, ProbeRatio
        };

        public static readonly StringOptions Boolean = new StringOptions
        {
            new StringOption("ON", "1"),
            new StringOption("OFF", "0")
        };

        public static readonly StringOptions BWLimit = new StringOptions
        {
            new StringOption("True", "20M"),
            new StringOption("False", "OFF"),
        };
        public static readonly StringOptions Coupling = new StringOptions
        {
            new StringOption("AC", "AC"),
            new StringOption("DC", "DC"),
            new StringOption("GND", "GND")
        };
        public static readonly StringOptions Units = new StringOptions
        {
            new StringOption("Volts", "VOLT"),
            new StringOption("Watts", "WATT"),
            new StringOption("Amps", "AMP"),
            new StringOption("?", "UNKN")
        };
        public static readonly StringOptions Vernier = new StringOptions
        {
            new StringOption("Coarse", "0"),
            new StringOption("Fine", "1"),
        };

        public static readonly StringOptions ProbeRatio = new StringOptions
        {
            new StringOption(".01x", "1.000000e-02"),
            new StringOption(".02x", "2.000000e-02"),
            new StringOption(".05x", "5.000000e-02"),
            new StringOption(".1x", "1.000000e-01"),
            new StringOption(".2x", "2.000000e-01"),
            new StringOption(".5x", "5.000000e-01"),
            new StringOption("?", ""),
            new StringOption("1x", "1.000000e+00"),
            new StringOption("2x", "2.000000e+00"),
            new StringOption("5x", "5.000000e+00"),
            new StringOption("10x", "1.000000e+01"),
            new StringOption("20x", "2.000000e+01"),
            new StringOption("50x", "5.000000e+01"),
            new StringOption("100x", "1.000000e+02"),
            new StringOption("200x", "2.000000e+02"),
            new StringOption("500x", "5.000000e+02"),
            new StringOption("1000x", "1.000000e+03"),
        };

        public List<string> ToNames()
        {
            return this.Cast<StringOption>().Select(x => x.Name).ToList();
        }
        public static List<string> ToNames(IOptions options)
        {
            var stringOption = options as StringOptions;
            return options.Select(x => x.Name).ToList();
        }

        public List<string> ToTerms()
        {
            return this.Cast<StringOption>().Select(x => x.Term).ToList();
        }
        public static List<string> ToTerms(IOptions options)
        {
            var stringOption = options as StringOptions;
            return options.Select(x => x.Term).ToList();
        }

        public static StringOption GetByTerm(IOptions options, string term)
        {
            var stringOptions = options as StringOptions;
            return stringOptions
                .Cast<StringOption>()
                .Where(x => x.Term.ToLower() == term.ToLower())
                .FirstOrDefault();
        }

        public StringOption GetByValue(string value)
        {
            //return this.Where(x => x.Value == value).First();
            return this
                .Cast<StringOption>()
                .Where(x => x.Name.ToLower() == value.ToLower())
                .FirstOrDefault();
        }
        public string GetTermForValue(string value)
        {
            var option = GetByValue(value);
            return option.Term;
        }

        public StringOption GetByParameter(string parameter)
        {
            return this.Cast<StringOption>().Where(x => x.Term == parameter).FirstOrDefault();
        }

        public static StringOption GetAnyByValue(string value)
        {
            foreach (var set in All)
            {
                var option = set.GetByValue(value);
                if (option != null) return option;
            }
            return null;
        }

        public void AddRange(IEnumerable<StringOption> items)
        {
            foreach (var item in items)
            {
                Items.Add(item);
            }
            OnCollectionChanged(
                new System.Collections.Specialized.NotifyCollectionChangedEventArgs(
                    System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }

        #region Implement ICollection<IOption>

        public bool IsReadOnly => false;

        public void Add(IOption item)
        {
            base.Add(item as StringOption);
        }

        public bool Contains(IOption item)
        {
            return base.Contains(item as StringOption);
        }

        public void CopyTo(IOption[] array, int arrayIndex)
        {
            base.CopyTo(array as StringOption[], arrayIndex);
        }

        public bool Remove(IOption item)
        {
            return base.Remove(item as StringOption);
        }

        IEnumerator<IOption> IEnumerable<IOption>.GetEnumerator()
        {
            return base.GetEnumerator();
        }

        #endregion Implement ICollection

        #region Implement List<IOption>

        public int IndexOf(IOption option)
        {
            return base.IndexOf(option as StringOption);
        }

        public void Insert(int index, IOption option)
        {
            base.Insert(index, option as StringOption);
        }

        IOption IList<IOption>.this[int index]
        {
            get => base[index];
            set => base[index] = value as StringOption;
        }

        #endregion

        #region Implement IOption

        public object GetIncrementedValue(object currentValue)
        {
            var option = GetByValue(currentValue as string);
            var index = IndexOf(option);
            if (index + 1 < this.Count)
            {
                return this[index + 1].Name;
            }
            return null;
        }

        public object GetDecrementedValue(object currentValue)
        {
            var option = GetByValue(currentValue as string);
            var index = IndexOf(option);
            if (index - 1 >= 0)
            {
                return this[index - 1].Name;
            }
            return null;
        }

        #endregion

    }

}
