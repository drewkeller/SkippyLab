using ReactiveUI;
using Scoopy.Views;
using Splat;
using System.Reflection;
using Xamarin.Forms;

namespace Scoopy
{
    public partial class App : Application
    {
        public App()
        {
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());

            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
