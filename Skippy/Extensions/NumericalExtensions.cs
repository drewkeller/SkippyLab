using Skippy.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skippy.Extensions
{
    public static class NumericalExtensions
    {
        public static SI.Units GetUnitPrefix(string @this)
        {
            if (@this.EndsWith("G"))
            {
                return SI.Units.G;
            }
            if (@this.EndsWith("M"))
            {
                return SI.Units.M;
            }
            else if (@this.EndsWith("k"))
            {
                return SI.Units.k;
            }
            else if (@this.EndsWith("m"))
            {
                return SI.Units.m;
            }
            else if (@this.EndsWith("u"))
            {
                return SI.Units.u;
            }
            else if (@this.EndsWith("n"))
            {
                return SI.Units.n;
            }
            else if (@this.EndsWith("p"))
            {
                return SI.Units.p;
            } else
            {
                return SI.Units.unity;
            }

        }

        /// <summary>
        /// Parses the number-like string and formatts it to the SCPI real format.
        /// The initial string may contain number prefixes like "m", "u", "p", etc.
        /// The initial string may contain unit: "V", "A", "W", "Volts", "Amps", "Watts".
        /// The initial string may end with "/div".
        /// Example: 1u => 1.000000e-06
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static string ToReal(this string @this)
        {
            var value = 0.0;

            @this = @this.Replace("/div", "");
            @this = @this.Replace("s", "").Replace("secs", "");
            @this = @this.Replace("V", "").Replace("Volts", "");
            @this = @this.Replace("A", "").Replace("Amps", "");
            @this = @this.Replace("W", "").Replace("Watts", "");
            @this = @this.Replace(" ", "");

            if (@this.EndsWith("G"))
            {
                value = double.Parse(@this.Replace("G", "")) * SI.M;
            }
            else if (@this.EndsWith("M"))
            {
                value = double.Parse(@this.Replace("M", "")) * SI.M;
            }
            else if (@this.EndsWith("k"))
            {
                value = double.Parse(@this.Replace("k", "")) * SI.k;
            }
            else if (@this.EndsWith("m"))
            {
                value = double.Parse(@this.Replace("m", "")) * SI.m;
            }
            else if (@this.EndsWith("u"))
            {
                value = double.Parse(@this.Replace("u", "")) * SI.u;
            }
            else if (@this.EndsWith("n"))
            {
                value = double.Parse(@this.Replace("n", "")) * SI.n;
            }
            else if (@this.EndsWith("p"))
            {
                value = double.Parse(@this.Replace("p", "")) * SI.p;
            } else
            {
                value = double.Parse(@this);
            }

            var result = value.ToString("e6");
            result = result.Replace("e-0", "e-").Replace("e0", "e+");
            return result;
        }

        /// <summary>
        /// Returns a numerical string formatted to the SCPI real format.
        /// Example: 1 micro => 1.000000e-06
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static string ToReal(this double @this)
        {
            // we want a format like:
            //   x.000 000 e +0y
            // The "e" specifier always results in an exponent of 3 digits,
            // so we blank out the first one.
            return @this.ToString("e6").Replace("e0", "e");
        }

        /// <summary>
        /// Returns a numerical string formatted to the SCPI real format.
        /// Example: 1 micro => 1.000000e-06
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static string ToReal(this int @this)
        {
            // we want a format like:
            //   x.000 000 e +0y
            // The "e" specifier always results in an exponent of 3 digits,
            // so we blank out the first one.
            return @this.ToString("e6").Replace("e0", "e");
        }

        public static string ToSIString(this double @this, string unit = "")
        {
            if (@this >= 1) return @this.ToString() + unit;
            if (@this < 1 * SI.p) return @this.ToString() + unit;
            if (@this < 1 * SI.n) return (@this * SI.T).ToString() + "p" + unit;
            if (@this < 1 * SI.u) return (@this * SI.G).ToString() + "n" + unit;
            if (@this < 1 * SI.m) return (@this * SI.M).ToString() + "u" + unit;
            return (@this * SI.k).ToString() + "m";
        }

    }
}
