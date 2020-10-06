using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.XamForms;
using Skippy.Extensions;
using Skippy.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.PlatformServices;
using System.Text.RegularExpressions;
using Xamarin.CustomControls;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Skippy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScopePage : ReactiveContentPage<ScopeVM>, IViewFor<ScreenControlVM>
    {
        [Reactive] private double width { get; set; }
        [Reactive] private double height { get; set; }
        [Reactive] private Xamarin.Forms.Internals.DeviceOrientation Orientation { get; set; }

        private List<ChannelView> ChannelPanels { get; set; }

        ScreenControlVM IViewFor<ScreenControlVM>.ViewModel { get => ScreenControlVM; set => ScreenControlVM = value; }
        public ScreenControlVM ScreenControlVM { get; set; }

        [Reactive] public DisplayInfo DisplayInfo { get; set; }

        //public ScreenControlView ScreenControlView { get; set; }
        //public TimebaseView TimebaseView { get; set; }
        //public TriggerView TriggerView { get; set; }

        public ScopePage()
        {
            InitializeComponent();
            AppLocator.CurrentPage = this;
            ViewModel = new ScopeVM();
            ScreenControlVM = this.ScreenPanel.ViewModel;
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

                this.WhenAnyValue(x => x.SidebarButton.Text)
                    .SubscribeOnUI()
                    .Subscribe(x => {
                        SideScroll.IsVisible = !SidebarButtonIsPressed();
                    });

                SidebarButton.Events().Clicked
                .SubscribeOnUI()
                .Subscribe(e => {
                    if (SidebarButtonIsPressed()) ShowSidebar();
                    else HideSidebar();
                });

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

            RunPanel.Click += SidePanel_Click;
            ScreenPanel.Click += SidePanel_Click;
            TriggerPanel.Click += SidePanel_Click;
            TimebasePanel.Click += SidePanel_Click;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!_handledFirstTime)
            {
                _handledFirstTime = true;
                ChannelPanels = new List<ChannelView>();
                for (var i = 1; i <= 4; i++)
                {
                    var channelVM = new ScopeChannelVM(i);
                    var channelPanel = new ChannelView(channelVM);
                    pnlChannels.Children.Add(channelPanel);
                    ChannelPanels.Add(channelPanel);
                    channelPanel.WidthRequest = AppLocator.ChannelStackClosedWidth;
                    channelPanel.Click += ChannelPanel_Click;
                }
            }
        }

        private void SidePanel_Click(object sender, Xamarin.CustomControls.AccordionItemClickEventArgs e)
        {
            UpdateSidePanel();
        }

        private void ChannelPanel_Click(object sender, AccordionItemClickEventArgs e)
        {
            UpdateChannelPanels(sender as AccordionItemView);
        }

        private bool _handledFirstTime;

        [Reactive] bool ShowChannelsOnBottom {get;set;}

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width != this.width || height != this.height)
            {
                this.width = width;
                this.height = height;

                if (width > height)
                {
                    Orientation = Xamarin.Forms.Internals.DeviceOrientation.Landscape;
                }
                else
                {
                    Orientation = Xamarin.Forms.Internals.DeviceOrientation.Portrait;
                }
                UpdateLayout();
                UpdateSidePanel();
                UpdateChannelPanels(null);

                if (!_firstDrawn)
                {
                    _firstDrawn = true;
                    OnFirstDrawing();
                }

            }
        }
        bool _firstDrawn;

        private void UpdateChannelPanels(AccordionItemView sender)
        {
            if (ChannelPanels == null) return;

            var panels = ChannelPanels.Cast<AccordionItemView>();
            var anyOpen = panels.Any(x => x.IsOpen);

            var offset = 0.0;
            var imageHeight = Math.Max(BlankScreenImage.Height, ScopeScreenImage.Height);
            var imageWidth = Math.Max(BlankScreenImage.Height, ScopeScreenImage.Width);
            if (imageWidth < 800)
            {
                imageHeight *= imageWidth / 800;
                offset = (800 - imageHeight) / 2;
            }
            if (imageHeight <= 0) imageHeight = this.height;

            var maxOpenHeight = 450;
            var minOpenHeight = 300;
            var closedHeight = 50;
            var closedWidth = 150;
            var openWidth = 300;

            var height = anyOpen
                ? Math.Min(maxOpenHeight, Math.Max(minOpenHeight, this.Height - imageHeight))
                : closedHeight;

            if (sender != null)
            {
                sender.WidthRequest = sender.IsOpen ? openWidth : closedWidth;
            }

            ChannelStack.HeightRequest = height;
            //RelativeLayout.SetHeightConstraint(ChannelStack,
            //    Constraint.RelativeToParent(parent => parent.Height - height));
        }

        private void UpdateSidePanel() 
        {
            var panels = new List<AccordionItemView> { RunPanel, ScreenPanel, TriggerPanel, TimebasePanel };
            var anyOpen = panels.Any(x => x.IsOpen);

            var imageWidth = Math.Max(ScopeScreenImage.Width, BlankScreenImage.Width);
            if (imageWidth <= 0) imageWidth = this.width;

            var maxOpenedWidth = 500;
            var minOpenedWidth = 250;
            var closedWidth = 150;

            var width = anyOpen
                ? Math.Min(maxOpenedWidth, Math.Max(minOpenedWidth, this.Width - imageWidth))
                : closedWidth;

            SideStack.WidthRequest = width;
            RelativeLayout.SetXConstraint(SideStack,
                Constraint.RelativeToParent(parent => parent.Width - width));
        }

        private void UpdateLayout()
        {
            var imageWidth = Math.Max(ScopeScreenImage.Width, BlankScreenImage.Width);
            var imageeHeight = Math.Max(ScopeScreenImage.Height, BlankScreenImage.Height);
            if (Orientation == Xamarin.Forms.Internals.DeviceOrientation.Landscape)
            {
            }
            else
            {
            }
            return;
            if (Orientation == Xamarin.Forms.Internals.DeviceOrientation.Landscape) 
            { 
                ChannelStack.Orientation = StackOrientation.Horizontal;
                ChannelStack.WidthRequest = imageWidth / 4;
                AbsoluteLayout.SetLayoutBounds(ChannelStack,
                    new Rectangle(0, ScopeScreenImage.Height, ScopeScreenImage.Width,
                    this.height - ScopeScreenImage.Height));
            } else {
                var sideWidth = this.width - imageWidth;
                if (sideWidth > 100)
                {
                    // right side, vertical
                    //SideStack.Orientation = StackOrientation.Vertical;
                    //SideScroll.Orientation = ScrollOrientation.Vertical;
                    Sidebar.Orientation = StackOrientation.Vertical;
                    AbsoluteLayout.SetLayoutBounds(SideStack,
                        new Rectangle(imageWidth, 0,
                        sideWidth, ChannelStack.HeightRequest));
                }
                else
                {
                    // 
                    //SideStack.Orientation = StackOrientation.Horizontal;
                    //SideScroll.Orientation = ScrollOrientation.Horizontal;
                    Sidebar.Orientation = StackOrientation.Horizontal;
                    AbsoluteLayout.SetLayoutBounds(SideStack,
                        new Rectangle(0, imageeHeight,
                        this.width, 50));
                }
                //ChannelStack.Orientation = StackOrientation.Vertical;
                //ChannelStack.WidthRequest = Math.Max(800, this.width - imageWidth);
                //AbsoluteLayout.SetLayoutBounds(ChannelStack,
                //    new Rectangle(SideStack.X, SideStack.Y,
                //    SideStack.Width, ChannelStack.HeightRequest));
            }
        }

        private void OnFirstDrawing()
        {
            if (this.width < 800 - 150) HideSidebar();
            else ShowSidebar();
        }

        private void HideSidebar()
        {
            SidebarButton.Text = "<";
        }

        private void ShowSidebar()
        {
            SidebarButton.Text = ">";
        }

        private bool SidebarButtonIsPressed()
        {
            return SidebarButton.Text == "<";
        }

    }
}