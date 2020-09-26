using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.XamForms;
using Skippy.ViewModels;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Skippy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScopePage : ReactiveContentPage<ScopeVM>, IViewFor<ScreenControlVM>
    {

        ScreenControlVM IViewFor<ScreenControlVM>.ViewModel { get => ScreenControlVM; set => ScreenControlVM = value; }
        public ScreenControlVM ScreenControlVM { get; set; }

        [Reactive] public DisplayInfo DisplayInfo { get; set; }

        //public ScreenControlView ScreenControlView { get; set; }
        //public TimebaseView TimebaseView { get; set; }
        //public TriggerView TriggerView { get; set; }

        public ScopePage()
        {
            InitializeComponent();
            ViewModel = new ScopeVM();
            ScreenControlVM = this.ScreenControlView.ViewModel;
            this.BindingContext = ViewModel;

            // overlay with our blank image until the scope screen is loaded
            this.BlankScreenImage.Source = ImageSource.FromResource("Skippy.Resources.blank.png");
            this.ScopeScreenImage.IsVisible = false;

            this.WhenActivated(disposable =>
            {

                this.WhenAnyValue(vm => vm.ScreenControlVM.Screen)
                    .Where(x => x != null)
                    .SubscribeOn(RxApp.MainThreadScheduler)
                    .Subscribe(x =>
                    {
                        this.ScopeScreenImage.Source = ScreenControlVM.Screen;
                        this.ScopeScreenImage.IsVisible = ScreenControlVM.Screen != null;
                        this.BlankScreenImage.IsVisible = ScreenControlVM.Screen == null;
                    })
                    .DisposeWith(disposable);

                //this.WhenAnyValue(
                //    v => v.Width, 
                //    v => Height)
                //.SubscribeOn(RxApp.MainThreadScheduler)
                //.Subscribe(x =>
                //{
                //    var effectiveHeight = DisplayInfo.Width / DisplayInfo.Density;
                //    { }
                //}).DisposeWith(disposable);

                //DeviceDisplay.MainDisplayInfoChanged += (s, e) =>
                //    DisplayInfo = DeviceDisplay.MainDisplayInfo;

            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!_handledFirstTime)
            {
                _handledFirstTime = true;
                for (var i = 1; i <= 4; i++)
                {
                    var channelVM = new ScopeChannelVM(i);
                    var channelPanel = new ScopeChannelView(channelVM);
                    pnlChannels.Children.Add(channelPanel);
                }
            }
        }
        private bool _handledFirstTime;

        [Reactive] bool ShowChannelsOnBottom {get;set;}

    }
}