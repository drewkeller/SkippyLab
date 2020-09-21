using ReactiveUI;
using ReactiveUI.XamForms;
using Skippy.Converters;
using Skippy.ViewModels;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Skippy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScopePage : ReactiveContentPage<ScopeVM>
    {

        public ScopePage()
        {
            InitializeComponent();
            ViewModel = new ScopeVM();
            this.BindingContext = ViewModel;

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

                this.CopyFolderButton
                    .Events().Clicked
                    .Select(args => Unit.Default)
                    .Subscribe(async (x) =>
                    {
                        await Clipboard.SetTextAsync(ViewModel.ScreenshotFolder);
                    })
                    .DisposeWith(disposable);

            });

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            for (var i = 1; i <= 4; i++)
            {
                var channelVM = new ScopeChannelVM(i);
                var channelPanel = new ScopeChannelView(channelVM);
                pnlChannels.Children.Add(channelPanel);
            }
        }

    }
}