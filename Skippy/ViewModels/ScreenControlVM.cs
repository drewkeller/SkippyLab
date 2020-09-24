using Acr.UserDialogs;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Skippy.Extensions;
using Skippy.Interfaces;
using Skippy.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CustomControls;
using Xamarin.Forms;

namespace Skippy.ViewModels
{
    public class ScreenControlVM : ReactiveObject, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; }

        public IScreenService ScreenService { get; private set; }

        #region Screen properties

        [Reactive] public ImageSource Screen { get; set; }

        [Reactive] private byte[] ScreenData { get; set; }
        [Reactive] public string ScreenshotFolder { get; set; }

        [Reactive] public bool ScreenshotFolderHasError { get; set; }
        [Reactive] public string ScreenshotFolderError { get; set; }

        [Reactive] public int ScreenRefreshRate { get; set; }
        [Reactive] private TimeSpan NextScreenshotTime { get; set; }
        [Reactive] private IObservable<long> ScreenshotTimer { get; set; }

        private readonly System.Timers.Timer ScreenRefreshTimer = new System.Timers.Timer();

        public IDisposable TimerSubscription { get; private set; }
        [Reactive] public bool AutorefreshEnabled { get; set; } = true;
        [Reactive] public bool HasScreenshot { get; set; }

        public ReactiveCommand<Unit, Unit> RefreshScreenCommand { get; }
        private IObservable<bool> CanExecuteRefreshScreen =>
            Observable.Return(AppLocator.TelnetService.Connected && !CopyingScreenshot);
        public ReactiveCommand<Unit, Unit> SaveScreenshotCommand { get; }
        [Reactive] private bool CopyingScreenshot { get; set; }
        //[Reactive] private bool SavingScreenshot { get; set; }

        #endregion

        public ScreenControlVM()
        {
            Activator = new ViewModelActivator();
            ScreenRefreshRate = 5000;
            var storage = Locator.Current.GetService<IScreenService>();
            //Assert.IsNotNull(storage, "Platform must implement IScreenshotStorage");
            ScreenshotFolder = Path.GetFullPath(storage.ScreenshotFolder);

            RefreshScreenCommand = ReactiveCommand
                .CreateFromTask(ExecuteRefreshScreen, CanExecuteRefreshScreen);

            RefreshScreenCommand.ThrownExceptions
                .SubscribeOn(RxApp.MainThreadScheduler)
                .Subscribe(ex =>
            {
                UserDialogs.Instance.Alert(ex.Message);
            });

            var canExecuteSaveScreenshot = this.WhenAnyValue(
                x => x.HasScreenshot,
                x => x.CopyingScreenshot,
                (hasScreenshot, copying) => hasScreenshot && !copying)
                .ObserveOn(RxApp.MainThreadScheduler);
            SaveScreenshotCommand = ReactiveCommand
                .Create(SaveScreenshotExecute, canExecuteSaveScreenshot);
            SaveScreenshotCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(ex => UserDialogs.Instance.Alert(ex.Message));

            this.WhenActivated(disposables =>
            {
                this.HandleActivation();

                Disposable
                    .Create(() => this.HandleDeactivation())
                    .DisposeWith(disposables);
                
                // restart timer when autorefresh is changed
                this.WhenPropertyChanged(x => x.AutorefreshEnabled)
                    .Where(x => x.Value == true)
                    .Subscribe(x => StartTimer());

                this.WhenPropertyChanged(x => x.ScreenRefreshRate)
                    .Subscribe(x =>
                    {
                        ScreenRefreshTimer.Stop();
                        ScreenRefreshTimer.Interval = ScreenRefreshRate;
                        ScreenRefreshTimer.Start();
                    });

                var ts = AppLocator.TelnetService;
                ts.WhenAnyValue(x => x.Connected)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToSignal()
                    .InvokeCommand(RefreshScreenCommand);

            });

        }

        private void HandleActivation()
        {
        }

        private void HandleDeactivation()
        {
        }

        private void SaveScreenshotExecute()
        {
            try
            {
                //SavingScreenshot = true;
                CopyingScreenshot = true;
                var data = ScreenData;
                CopyingScreenshot = false;
                // save to file
                var storage = Locator.Current.GetService<IScreenService>();
                var folder = storage.ScreenshotFolder;
                var filename = DateTime.Now.ToString("yyyy-MM-dd_HHmm_ss") + ".png";
                File.WriteAllBytes(Path.Combine(folder, filename), data);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"Failed to save screenshot: \r\n{ex}");
            }
            finally
            {
                //SavingScreenshot = false;
            }
        }

        #region GetScreenshot

        private async Task<Unit> ExecuteRefreshScreen()
        {
            var telnet = AppLocator.TelnetService;

            if (_waitingForScreenshot)
            {
                return Unit.Default;
            }
            _waitingForScreenshot = true;

            var bytes = await telnet.GetScreenshot();
            if (bytes == null)
            {
                _waitingForScreenshot = false;
                Debug.WriteLine($"{DateTime.Now}: Screenshot failed");
                return Unit.Default;
            }

            // display screenshot
            //Debug.WriteLine($"{DateTime.Now}: Screenshot succeeded");
            ScreenData = bytes;
            var img = ImageSource.FromStream(() => new MemoryStream(bytes));
            this.Screen = img;
            _waitingForScreenshot = false;
            HasScreenshot = true;
            return Unit.Default;
        }
        private bool _waitingForScreenshot;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void StartTimer()
        {
            // maybe WhenPropertyChanged NextScreenshotTime in constructor?
            this.NextScreenshotTime = TimeSpan.FromMilliseconds(this.ScreenRefreshRate);
            this.ScreenshotTimer = Observable.Timer(NextScreenshotTime);
            this.TimerSubscription = ScreenshotTimer
                .ObserveOn(RxApp.MainThreadScheduler)
                .Distinct()
                .Subscribe(x =>
                {
                    if (AutorefreshEnabled)
                    {
                        //Debug.WriteLine($"{DateTime.Now}: Requesting screenshot (next in {ScreenRefreshRate/1000}s)");
                        RefreshScreenCommand.Execute();
                        StartTimer();
                    }
                });
        }



    }
}
