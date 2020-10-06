using ReactiveUI;
using Skippy.Controls;
using Skippy.Interfaces;
using Skippy.ViewModels;
using Skippy.Views;
using Splat;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Skippy
{
    public partial class App : Application
    {
        /// <summary>
        /// Set to true to not require actual connection to equipment.
        /// </summary>
        public static bool Mock => true;

        public App()
        {
            AppLocator.App = this;
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());

            InitializeComponent();

            var mainPage = new MainPage();
            MainPage = new NavigationPage(mainPage);
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

        public void AddTogglesBarStyle()
        {
            var togglesBarStyle = new Style(typeof(TogglesBar))
            {
                Setters =
                {
                    new Setter {Property = TogglesBar.SelectedColorProperty, Value = AppLocator.TextColor}
                }
            };
            Resources.Add(togglesBarStyle);

            var frameStyle = new Style(typeof(Frame))
            {
                Setters = {
                    new Setter { Property = Frame.BorderColorProperty, Value = AppLocator.TextColor},
                    new Setter { Property = Frame.BackgroundColorProperty, Value = AppLocator.BackgroundColor },
                    new Setter { Property = Frame.OpacityProperty, Value = 0.9 },
                }
            };
            Resources.Add(frameStyle);


        }

    }
}
