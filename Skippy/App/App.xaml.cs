using ReactiveUI;
using Skippy.Interfaces;
using Skippy.ViewModels;
using Skippy.Views;
using Splat;
using System.Reflection;
using Xamarin.Forms;

namespace Skippy
{
    public partial class App : Application
    {
        public static bool Mock => true;

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
