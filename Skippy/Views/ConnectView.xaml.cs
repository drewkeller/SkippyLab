using ReactiveUI;
using ReactiveUI.XamForms;
using Skippy.ViewModels;
using System.Reactive.Disposables;
using Xamarin.Forms.Xaml;

namespace Skippy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectView : ReactiveContentView<ConnectVM>
    {
        public ConnectView()
        {
            InitializeComponent();
            ViewModel = new ConnectVM();

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