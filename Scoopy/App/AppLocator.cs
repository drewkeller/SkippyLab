using ReactiveUI;
using Scoopy.Converters;
using Scoopy.Models;
using Scoopy.Services;
using Scoopy.ViewModels;
using Splat;

namespace Scoopy
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
