using ReactiveUI;
using ReactiveUI.XamForms;
using Scoopy.Converters;
using Scoopy.ViewModels;
using System.Reactive.Disposables;
using Xamarin.Forms.Xaml;

namespace Scoopy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScopePage : ReactiveContentPage<ScopeVM>
    {

        public ScopePage()
        {
            ViewModel = new ScopeVM();
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.Bind(ViewModel,
                    x => x.Screen,
                    x => x.imgScreen.Source)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.AutorefreshEnabled,
                    x => x.uiAutorefresh.IsToggled)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.ScreenRefreshRate,
                    x => x.uiScreenRefreshRate.Text,
                    vmToViewConverterOverride: new IntToStringConverter()
                    )
                    .DisposeWith(disposable);

                this.BindCommand(ViewModel,
                    x => x.RefreshScreenCommand, 
                    x => x.RefreshScreenButton)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.ScreenshotFolder,
                    x => x.ScreenshotFolder.Text)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.ScreenshotFolderHasError,
                    x => x.uiScreenshotFolderError.IsVisible)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.ScreenshotFolderError,
                    x => x.uiScreenshotFolderError.Text)
                    .DisposeWith(disposable);

                this.BindCommand(ViewModel,
                    x => x.SaveScreenshotCommand,
                    x => x.SaveScreenshotButton)
                    .DisposeWith(disposable);

            });

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            for (int i = 1; i <= 4; i++)
            {
                var channelVM = new ScopeChannelVM(i);
                var channelPanel = new ScopeChannelView(channelVM);
                pnlChannels.Children.Add(channelPanel);
            }
        }

    }
}