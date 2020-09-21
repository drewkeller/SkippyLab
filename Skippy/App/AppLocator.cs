using ReactiveUI;
using Skippy.Converters;
using Skippy.Models;
using Skippy.Services;
using Skippy.ViewModels;
using Splat;

namespace Skippy
{
    public class AppLocator
    {

        public static ScopeVM ScopeVM => Locator.Current.GetService<ScopeVM>();

        public static TelnetService TelnetService => Locator.Current.GetService<TelnetService>();

        public static Settings Settings => Locator.Current.GetService<Settings>();

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
        }

    }
}
