using Skippy.Extensions;
using System;

namespace Skippy.Models
{
    public class SI
    {
        public const double unity = 1.0;

        public const double T = G * 1000;
        public const double G = M * 1000;
        public const double M = k * 1000;
        public const double k = unity * 1000;
        public const double m = unity / 1000;
        public const double u = m / 1000;
        public const double n = u / 1000;
        public const double p = n / 1000;

        public const double tera = T;
        public const double giga = G;
        public const double mega = M;
        public const double kilo = k;
        public const double milli = m;
        public const double micro = u;
        public const double nano = n;
        public const double pico = p;

        public enum Units
        {
            unity,
            T, G, M, k, m, u, n, p,
            tera, giga, mega, kilo, milli, micro, nano, pico
        }

        internal static double Parse(string value)
        {
            var real = value.ToReal();
            return double.Parse(real);
        }

    }

}
