using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.XamForms;
using Skippy.Extensions;
using Skippy.ViewModels;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Skippy.Views
{
    public partial class MainPage : ReactiveContentPage<MainPageVM>
    {
        public ViewModelActivator Activator { get; }

        public MainPage()
        {
            AppLocator.Init();
            Activator = new ViewModelActivator();

            AppLocator.CurrentPage = this;

            InitializeComponent();
            ViewModel = new MainPageVM();
            var connectVM = ConnectView.ViewModel;
            this.AddProgressDisplay();

            // navigate to scope page when connection is achieved
            AppLocator.TelnetService.WhenValueChanged(x => x.Connected)
                .Where(x => x == true)
                .ToSignal()
                .Subscribe(x => Navigation.PushAsync(new ScopePage()));

            this.WhenActivated(disposables =>
            {
                this.HandleActivation();

                this.OneWayBind(ViewModel,
                    vm => vm.Title,
                    v => v.Title)
                .DisposeWith(disposables);

            });

        }

        private void HandleActivation()
        {
            AppLocator.TelnetService.Connected = false;
        }

    }
}
