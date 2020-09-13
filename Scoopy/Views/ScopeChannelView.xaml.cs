using Scoopy.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scoopy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScopeChannelView : ContentView
    {
        public class NameValue
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        public class Coupling : List<NameValue>
        {
            public void Add(string name, string value)
            {
                var item = new NameValue() { Name = name, Value = value };
                base.Add(item);
            }
        }

        public IEnumerable<string> CouplingOptions => Enums.StringOptions.CouplingOptions;

        public ScopeChannelView(int channelNumber)
        {
            InitializeComponent();

            txtLabel.Text = $"CH{channelNumber}"; // some initial text
            txtLabel.MainLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;

            barCoupling.ItemsSource = Enums.StringOptions.CouplingOptions;
            barCoupling.InitialValue = "DC";

            barUnits.ItemsSource = Enums.StringOptions.Units;
            barUnits.InitialValue = "Volts";

            txtOffset.Text = "(offset)";
            txtRange.Text = "(range)";
            txtScale.Text = "(scale)";
            txtTCal.Text = "(tcal)";
            cboRatio.SelectedItem = "10:1";

            barFine.ItemsSource = Enums.StringOptions.FineCourse;
            barFine.InitialValue = "Course";
        }

    }
}