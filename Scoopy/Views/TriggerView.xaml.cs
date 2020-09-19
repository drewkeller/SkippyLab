using ReactiveUI;
using ReactiveUI.XamForms;
using Scoopy.Converters;
using Scoopy.Protocols;
using Scoopy.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scoopy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TriggerView : ReactiveContentView<TriggerVM>
    {

        public TriggerView()
        {
            InitializeComponent();
            ViewModel = new TriggerVM();

            this.BindingContext = ViewModel;

            // initialize some stuff
            //txtLabel.MainLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
            Mode.ItemsSource = StringOptions.GetStringValues(TriggerCommands.Mode.Options);
            Source.ItemsSource = StringOptions.GetStringValues(TriggerEdgeCommands.Source.Options);
            Slope.ItemsSource = StringOptions.GetStringValues(TriggerEdgeCommands.Slope.Options);

            this.WhenActivated(disposable =>
            {
                this.Bind(ViewModel,
                    x => x.Mode.Value,
                    x => x.Mode.SelectedItem,
                    vmToViewConverterOverride: new StringOptionsToStringConverter())
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                    x => x.Mode.GetSucceeded,
                    x => x.Mode.IsEnabled)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.Source.Value,
                    x => x.Source.SelectedItem,
                    vmToViewConverterOverride: new StringOptionsToStringConverter())
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                    x => x.Source.GetSucceeded,
                    x => x.Source.IsEnabled)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.Slope.Value,
                    x => x.Slope.SelectedItems,
                    vmToViewConverterOverride: new StringOptionsToStringConverter())
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                    x => x.Slope.GetSucceeded,
                    x => x.Slope.IsEnabled)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.Level.Value,
                    x => x.Level.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                    x => x.Level.GetSucceeded,
                    x => x.Level.IsEnabled)
                    .DisposeWith(disposable);

                ViewModel.GetAll.Execute(null);
                WireEvents();
            });
        }

        private void WireEvents()
        {

        }

    }
}