using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Scoopy.Protocols;
using Splat;
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

            //var protocol = new ProtocolCommandSet();
            //var storage = Locator.Current.GetService<IScreenshotStorage>();
            //var jsonFile = System.IO.Path.Combine(storage.ScreenshotFolder, "rigol.json");
            //protocol.Serialize(jsonFile);
        }

        [ObservableAsProperty]
        public bool WelcomeScreenCollapsed { get; }

    }
}
