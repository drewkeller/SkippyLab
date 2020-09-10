using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.XamForms;
using Scoopy.Extensions;
using Scoopy.ViewModels;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace Scoopy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectView : ReactiveContentView<ConnectVM>
    {
        public ConnectView()
        {
            InitializeComponent();
            ViewModel = AppLocator.ConnectVM;

            this.WhenActivated(disposable =>
            {
                this.Bind(ViewModel,
                    x => x.Hostname,
                    x => x.txtHost.Text)
                .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.Port,
                    x => x.txtPort.Text,
                    x => x.ToString(),
                    x => int.Parse(x))
                .DisposeWith(disposable);

                this.BindCommand(ViewModel,
                    x => x.ConnectCommand,
                    x => x.btnConnect)
                .DisposeWith(disposable);
            });

        }

    }
}