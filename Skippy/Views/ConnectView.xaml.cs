using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.XamForms;
using Rg.Plugins.Popup.Services;
using Skippy.Extensions;
using Skippy.Models;
using Skippy.Protocols;
using Skippy.ViewModels;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
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

                this.Bind(ViewModel,
                    x => x.IsBusy,
                    x => x.lblConnecting.IsVisible)
                .DisposeWith(disposable);

                this.btnConnect.Events().Clicked
                .SubscribeOnUI()
                .Subscribe(X => this.lblConnecting.IsVisible = true);

                //btnConnect.Events().Clicked
                //    .SubscribeOn(RxApp.MainThreadScheduler)
                //    .Subscribe(async s =>
                //    {
                //        var popup = new Controls.PopSlider(
                //            "Testing 123",
                //            SI.Parse("200m"),
                //            TimebaseScaleOptions.YT as RealOptions,
                //            x =>
                //            {
                //                var y = x;
                //            });
                //        await PopupNavigation.Instance.PushAsync(popup);
                //    });

            });


        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);
            if (!first)
            {
                first = true;
                AppLocator.App.AddTogglesBarStyle();
            }
        }
        bool first;
    }
}