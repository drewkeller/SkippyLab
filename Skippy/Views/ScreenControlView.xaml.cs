using ReactiveUI;
using ReactiveUI.XamForms;
using Skippy.Converters;
using Skippy.Extensions;
using Skippy.Protocols;
using Skippy.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.CustomControls;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Skippy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScreenControlView : AccordionItemView, IViewFor<ScreenControlVM>
    {
        public ScreenControlVM ViewModel { get; set; }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as ScreenControlVM; }


        public ScreenControlView()
        {
            InitializeComponent();
            ViewModel = new ScreenControlVM();
            this.BindingContext = ViewModel;

            this.WhenActivated(disposable =>
            {
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
    }
}