using ReactiveUI;
using ReactiveUI.XamForms;
using Scoopy.Enums;
using Scoopy.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scoopy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScopeChannelView : ReactiveContentView<ScopeChannelVM>
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

        public ScopeChannelView(ScopeChannelVM viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;

            // initialize some stuff
            txtLabel.MainLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
            barCoupling.ItemsSource = Enums.StringOptions.CouplingOptions;
            barUnits.ItemsSource = Enums.StringOptions.Units;
            barFine.ItemsSource = Enums.StringOptions.FineCourse;

            this.WhenActivated(async disposable =>
            {
                this.Bind(ViewModel,
                    x => x.ChannelName,
                    x => x.txtLabel.Text)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.IsActive,
                    x => x.chkActive.IsOn)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.IsInverted,
                    x => x.chkInverted.IsToggled)
                    .DisposeWith(disposable);

                //this.Bind(ViewModel, 
                //    x => x.)

                await ViewModel.SendIsActiveQueryAsync();
                //await ViewModel.SendGetAllQuery();

            });

            barCoupling.InitialValue = "DC";

            barUnits.InitialValue = "Volts";

            txtOffset.Text = "(offset)";
            txtRange.Text = "(range)";
            txtScale.Text = "(scale)";
            txtTCal.Text = "(tcal)";
            cboRatio.SelectedItem = "10:1";

            barFine.InitialValue = "Course";

        }

        //private IReactiveBinding<ScopeChannelView, ScopeChannelVM, (object? view, bool isViewModel)> Bind
        //    (object viewModelProperty, object viewProperty)
        //    {
        //        this.Bind(ViewModel, x = )
        //    }

    }
}