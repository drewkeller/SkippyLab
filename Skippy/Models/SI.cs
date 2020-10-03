namespace Skippy.Models
{
    public class SI
    {
        public const double unity = 1;

        public const double G = M * 1000;
        public const double M = k * 1000;
        public const double k = 1 * 1000;
        public const double m = 1 / 1000;
        public const double u = m / 1000;
        public const double n = u / 1000;
        public const double p = n / 1000;

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
            G, M, k, m, u, n, p,
            giga, mega, kilo, milli, micro, nano, pico
        }
    }

}
