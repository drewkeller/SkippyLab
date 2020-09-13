using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Scoopy.Enums
{

    public class StringOptions : ObservableCollection<string>
    {
        public static readonly StringOptions CouplingOptions = new StringOptions { "AC", "DC", "GND" };
        public static readonly StringOptions Units = new StringOptions { "Volts", "Watts", "Amps", "?" };
        public static readonly StringOptions FineCourse = new StringOptions { "Course", "Fine" };
    }

}
