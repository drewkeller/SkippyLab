using System.Collections.ObjectModel;
using System.Linq;

namespace Scoopy.Enums
{

    /// <summary>
    /// Provides information for displaying a value and sending the value via SCPI.
    /// </summary>
    public class StringOption
    {
        /// <summary>
        /// The text to use when getting or sending the value via SCPI.
        /// </summary>
        public string Parameter { get; set; }

        /// <summary>
        /// The human-readable value
        /// </summary>
        public string Value { get; set; }

        public StringOption() { }

        /// <summary>
        /// Create a string option
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        public StringOption(string value, string parameter)
        {
            Value = value;
            Parameter = parameter; 
        }

        public override string ToString()
        {
            //return $"{ID}: {Value}";
            return $"{Value}";
        }
    }

    public class StringOptions : ObservableCollection<StringOption>
    {
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
            new StringOption(".01x", "0.01"),
            new StringOption(".02x", "0.02"),
            new StringOption(".05x", "0.05"),
            new StringOption(".1x", "0.1"),
            new StringOption(".2x", "0.2"),
            new StringOption(".5x", "0.5"),
            new StringOption("?", ""),
            new StringOption("1x", "1"),
            new StringOption("2x", "2"),
            new StringOption("5x", "5"),
            new StringOption("10x", "10"),
            new StringOption("20x", "20"),
            new StringOption("50x", "50"),
            new StringOption("100x", "100"),
            new StringOption("200x", "200"),
            new StringOption("500x", "500"),
            new StringOption("1000x", "1000"),
        };

        public StringOption GetByValue(string value)
        {
            //return this.Where(x => x.Value == value).First();
            return this.Where(x => x.Value == value).FirstOrDefault();
        }
        public StringOption GetByParameter(string parameter)
        {
            return this.Where(x => x.Parameter == parameter).FirstOrDefault();
        }
    }

}
