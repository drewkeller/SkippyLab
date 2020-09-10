using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;

namespace Scoopy.ViewModels
{
    public class MainPageVM : ReactiveObject
    {
        public MainPageVM()
        {
            var telnet = AppLocator.TelnetService;

            telnet.WhenValueChanged(x => x.Connected)
                .Where(x => x == true)
                .ToProperty(this, nameof(WelcomeScreenCollapsed));
        }

        [ObservableAsProperty]
        public bool WelcomeScreenCollapsed { get; }

    }
}
