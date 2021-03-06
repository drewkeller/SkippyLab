﻿using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Skippy
{
    public enum Coupling
    {
        AC, DC, GND
    }

    public enum ChannelUnits
    {
        Volts, Watts, Amps, Unknown
    }

    public class ScopeChannel : ReactiveObject
    {
        public int ChannelNumber { get; set; }

        public string Name { get; set; }

        [Reactive] public bool IsActive { get; set; }

        [Reactive] public string Coupling { get; set; }

        /// <summary>
        /// The offset Value is related to the current vertical scale and probe ratio.
        /// f( 1X, scale >= 500) = -100 to +100
        /// f( 1X, scale <  500) = -2 to +2
        /// f(10X, scale >= 5) = -1000 to 1000
        /// f(10X, scale <  5) = -20 to 20
        /// </summary>
        [Reactive] public double Offset { get; set; }

        public double Range { get; set; }

        /// <summary>
        /// Indirectly modifies the vertical scale:
        /// Vertical Scale = Vertical Range / 8
        /// 
        /// </summary>
        public double Scale { get; set; }

        /// <summary>
        /// Discrete steps: .01, .02, .05, .1, .2, .5, 1, 2, 5, 10, 20, 50, 100, 200, 500, 1000
        /// </summary>
        public string Probe { get; set; }

        [Reactive] public bool IsInverted { get; set; }

        [Reactive] public bool IsBandwidthLimited { get; set; }

        public bool IsVernier { get; set; }

        public double TCal { get; set; }

        public string Units { get; set; }

    }

}
