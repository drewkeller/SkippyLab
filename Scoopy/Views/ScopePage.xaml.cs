using ReactiveUI;
using ReactiveUI.XamForms;
using Scoopy.ViewModels;
using System.Reactive.Disposables;
using Xamarin.Forms.Xaml;

namespace Scoopy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScopePage : ReactiveContentPage<ScopeVM>
    {

        public ScopePage() { }

        public ScopePage(ScopeVM vm)
        {
            ViewModel = vm;
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.Bind(ViewModel, 
                    x => x.Screen, 
                    x => x.imgScreen.Source)
                    .DisposeWith(disposable);

                this.BindCommand(ViewModel,
                    x => x.GetScreenshotCommand, 
                    x => x.btnScreenshot)
                    .DisposeWith(disposable);
            });

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            for (int i = 1; i <= 4; i++)
            {
                var channelPanel = new ScopeChannelView(i);
                pnlChannels.Children.Add(channelPanel);
            }
        }

    }
}