using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;

namespace Scoopy.Extensions
{
    public static class IObservableExtensions
    {

        public static IObservable<Unit> ToSignal<TDontCare>(this IObservable<TDontCare> source)
            => source.Select(_ => Unit.Default);

        public static IObservable<bool> ThrottleMs(this IObservable<bool> source, int milliseconds)
            => source.Throttle(TimeSpan.FromMilliseconds(milliseconds));

    }

}
