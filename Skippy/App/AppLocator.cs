using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Skippy.Converters;
using Skippy.Interfaces;
using Skippy.Models;
using Skippy.Services;
using Skippy.ViewModels;
using Skippy.Views;
using Splat;
using Xamarin.Forms;

namespace Skippy
{
    public class AppLocator
    {
        
        public static App App { get; set; }

        public static ScopeVM ScopeVM => Locator.Current.GetService<ScopeVM>();

        [Reactive] public static TimebaseVM Timebase { get; set; }

        public static TelnetService TelnetService => Locator.Current.GetService<TelnetService>();

        public static DialogService Dialogs => Locator.Current.GetService<IDialogService>() as DialogService;

        public static Settings Settings => Locator.Current.GetService<Settings>();

        public static Color BackgroundColor { get; set; }
        public static Color TextColor { get; set; }

        public static double ChannelStackClosedWidth = 150;
        public static double ChannelStackOpenedWidth = 300;

        /// <summary>
        /// Provides a means for UI activities requiring a page, such as <see cref="DisplayAlert"/>.
        /// </summary>
        public static ContentPage CurrentPage { get; internal set; }

        static public void Init()
        {
            // register converters
            Locator.CurrentMutable.RegisterConstant(
                new DoubleToStringConverter(),
                typeof(IBindingTypeConverter)
            );

            // singletons (RegisterConstant)
            Locator.CurrentMutable.RegisterConstant(new Settings()); // need settings before others
            Locator.CurrentMutable.RegisterConstant(new TelnetService());
            Locator.CurrentMutable.RegisterConstant(new DialogService(), typeof(IDialogService));
            Locator.CurrentMutable.RegisterConstant(new TimebaseVM(), typeof(TimebaseVM));
        }

    }
}
