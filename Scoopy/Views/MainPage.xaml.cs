using DynamicData.Binding;
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
        public MainPage()
        {
            AppLocator.Init();

            InitializeComponent();
            ViewModel = new MainPageVM();

            // navigate to scope page when connection is achieved
            AppLocator.ConnectVM.WhenValueChanged(x => x.IsConnected)
                .Where(x => x == true)
                .ToSignal()
                .Subscribe(x => Navigation.PushAsync(new ScopePage(new ScopeVM())));
        }

    }
}
