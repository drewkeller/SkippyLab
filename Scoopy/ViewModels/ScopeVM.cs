using Acr.UserDialogs;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Scoopy.Extensions;
using Scoopy.Models;
using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Scoopy.ViewModels
{
    public class ScopeVM : ReactiveObject
    {
        public ViewModelActivator Activator { get; }

        Settings Settings => AppLocator.Settings;

        public Scope Scope { get; set; }

        public string Hostname { get => _hostname; set => this.RaiseAndSetIfChanged(ref _hostname, value); }
        private string _hostname;

        [Reactive] public int Port { get; set; }

        // read/write property
        [Reactive] public ImageSource Screen { get; set; }
        [Reactive] private byte[] ScreenData { get; set; }
        public string ScreenshotFolder => Settings.ScreenshotFolder;

        [Reactive] public bool ScreenshotFolderExists { get; set; }

        [Reactive] public int ScreenRefreshRate { get; set; } = 5000;
        [Reactive] private TimeSpan NextScreenshotTime { get; set; }
        [Reactive] private IObservable<long> ScreenshotTimer { get; set; }
        public IDisposable TimerSubscription { get; private set; }
        [Reactive] public bool AutorefreshEnabled { get; set; } = true;
        [Reactive] public bool HasScreenshot { get; set; }

        public ReactiveCommand<Unit, Unit> RefreshScreenCommand { get; }
        private IObservable<bool> CanExecuteRefreshScreen =>
            Observable.Return(AppLocator.TelnetService.Connected && !CopyingScreenshot);
        public ReactiveCommand<Unit, Unit> SaveScreenshotCommand { get; }
        [Reactive] private bool CopyingScreenshot { get; set; }
        [Reactive] private bool SavingScreenshot { get; set; }
        private IObservable<bool> CanExecuteSaveScreenshot =>
            Observable.Return(ScreenshotFolderExists && HasScreenshot && !SavingScreenshot);

        public ScopeVM()
        {
            Activator = new ViewModelActivator();

            RefreshScreenCommand = ReactiveCommand
                .CreateFromTask(ExecuteRefreshScreen, CanExecuteRefreshScreen);

            RefreshScreenCommand.ThrownExceptions.Subscribe(ex =>
            {
                UserDialogs.Instance.Alert(ex.Message);
            });

            SaveScreenshotCommand = ReactiveCommand
                .Create(SaveScreenshotExecute, CanExecuteSaveScreenshot);
            SaveScreenshotCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(ex => UserDialogs.Instance.Alert(ex.Message));

            // restart timer when autorefresh is changed
            this.WhenPropertyChanged(x => x.AutorefreshEnabled)
                .Where(x => x.Value == true)
                .Subscribe(x => StartTimer());

            this.WhenPropertyChanged(x => x.ScreenshotFolder)
                .Subscribe(x => ScreenshotFolderExists =
                    ScreenshotFolder.HasData() && Directory.Exists(ScreenshotFolder));

            var ts = AppLocator.TelnetService;
            ts.WhenAnyValue(x => x.Connected)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToSignal()
                .InvokeCommand(RefreshScreenCommand);

            //GetScreenshotCommand.CanExecute
            //    .Where(x => x == true)
            //    .Subscribe(x => StartTimer());

            //Observable
            //    .Interval(TimeSpan.FromMilliseconds(ScreenRefreshRate))
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Subscribe(x =>
            //   {
            //       if (AppLocator.TelnetService.Connected && AutorefreshEnabled)
            //           RefreshScreenCommand.Execute();
            //   });

            //StartTimer();
        }

        private void SaveScreenshotExecute()
        {
            try
            {
                SavingScreenshot = true;
                CopyingScreenshot = true;
                var data = ScreenData;
                CopyingScreenshot = false;
                // save to file
                var folder = ScreenshotFolder;
                var filename = DateTime.Now.ToString("yyyy-MM-dd_HHmm_ss") + ".png";
                File.WriteAllBytes(Path.Combine(folder, filename), data);
            }
            finally
            {
                SavingScreenshot = false;
            }
        }

        #region GetScreenshot

        private async Task<Unit> ExecuteRefreshScreen()
        {
            var telnet = AppLocator.TelnetService;

            if (_waitingForScreenshot) return Unit.Default;
            _waitingForScreenshot = true;

            var bytes = await telnet.GetScreenshot();
            if (bytes == null)
            {
                _waitingForScreenshot = false;
                Debug.WriteLine($"{DateTime.Now}: Screenshot failed");
                return Unit.Default;
            }

            // display screenshot
            Debug.Write($"{DateTime.Now}: Screenshot succeeded");
            var img = ImageSource.FromStream(() => new MemoryStream(bytes));
            Screen = img;
            _waitingForScreenshot = false;
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
                        Debug.WriteLine($"{DateTime.Now}: Requesting screenshot");
                        RefreshScreenCommand.Execute();
                        StartTimer();
                    }
                });
        }

    }

}
