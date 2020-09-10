using Acr.UserDialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Scoopy.Extensions;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Scoopy.ViewModels
{
    public class ScopeVM : ReactiveObject
    {
        public Scope Scope { get; set; }

        public string Hostname { get => _hostname; set => this.RaiseAndSetIfChanged(ref _hostname, value); }
        private string _hostname;

        [Reactive] public int Port { get; set; }

        // read/write property
        public ImageSource Screen { get => _image; set => this.RaiseAndSetIfChanged(ref _image, value); }
        private ImageSource _image;

        public ReactiveCommand<Unit, Unit> GetScreenshotCommand { get; }

        public ScopeVM()
        {
            GetScreenshotCommand = ReactiveCommand
                .CreateFromTask(ExecuteGetScreenshot, CanExecuteGetScreenShot);
                //.CreateFromObservable(ExecuteGetScreenshot, CanExecuteGetScreenShot);
            GetScreenshotCommand.ThrownExceptions.Subscribe(ex =>
            {
                UserDialogs.Instance.Alert(ex.Message);
            });

            var ts = AppLocator.TelnetService;
            ts.WhenAnyValue(x => x.Connected)
                .ToSignal()
                .InvokeCommand(GetScreenshotCommand);
        }

        #region GetScreenshot

        private async Task<Unit> ExecuteGetScreenshot()
        {
            var telnet = AppLocator.TelnetService;
            if (!telnet.Connected) return Unit.Default;

            var bytes = await telnet.GetScreenshot();
            if (bytes == null) return Unit.Default;

            // display screenshot
            var img = ImageSource.FromStream(() => new MemoryStream(bytes));
            Screen = img;
            return Unit.Default;
        }

        private IObservable<bool> CanExecuteGetScreenShot
        {
            get
            {
                return Observable.Return(AppLocator.TelnetService.Connected);
            }
        }

        #endregion


    }

}
