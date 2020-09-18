using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.XamForms;
using Scoopy.Extensions;
using Scoopy.ViewModels;
using System;
using System.Linq;
using System.Reactive.Linq;
using Xamarin.Forms;

namespace Scoopy.Views
{
    public partial class MainPage : ReactiveContentPage<MainPageVM>
    {
        public ViewModelActivator Activator { get; }

        public MainPage()
        {
            AppLocator.Init();
            Activator = new ViewModelActivator();

            InitializeComponent();
            ViewModel = new MainPageVM();
            var connectVM = ConnectView.ViewModel;

            // navigate to scope page when connection is achieved
            AppLocator.TelnetService.WhenValueChanged(x => x.Connected)
                .Where(x => x == true)
                .ToSignal()
                .Subscribe(x => Navigation.PushAsync(new ScopePage()));

            this.WhenActivated(disposables =>
            {
                this.HandleActivation();
            });

        }

        private void HandleActivation()
        {
            AppLocator.TelnetService.Connected = false;
        }

    }
}
