using DynamicData.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Skippy.Protocols
{
    public class ChannelProtocol : ProtocolCommand
    {
        public int ChannelNumber { get; set; }

        public override string Name => $"Channel{ChannelNumber}";

        public override string Term => "CHAN";

        public override string Path => $":CHAN{PathParameter}";

        public override IProtocolCommandList Subcommands => new ProtocolCommandList
        {
            BWLimit, Coupling, Display, Invert, Offset, Range, TCal, 
            Scale, Probe, Units, Vernier
        };

        #region Subcommands

        public ProtocolCommand BWLimit { get; set; }

        public ProtocolCommand Coupling { get; set; }

        public ProtocolCommand Display { get; set; }

        public ProtocolCommand Invert { get; set; }

        public ProtocolCommand Offset { get; set; }

        public ProtocolCommand Range { get; set; }

        public ProtocolCommand TCal { get; set; }

        public ProtocolCommand Scale { get; set; }

        public ProtocolCommand Probe { get; set; }

        public ProtocolCommand Units { get; set; }

        public ProtocolCommand Vernier { get; set; }

        #endregion // Subcommands

        public ChannelProtocol(IProtocolCommand parent, int channelNumber) : base(parent)
        {
            ChannelNumber = channelNumber;
            PathParameter = channelNumber;

            BWLimit = new ProtocolCommand(this, "Bandwidth Limit", "BWL")
            {
                Options = StringOptions.BWLimit,
            };

            Coupling = new ProtocolCommand(this, "Coupling", "COUP")
            {
                Options = StringOptions.Coupling,
            };

            Display = new ProtocolCommand(this, "Display", "DISP")
            {
                Options = BooleanOptions.Boolean,
            };

            Invert = new ProtocolCommand(this, "Invert", "INV")
            {
                Options = BooleanOptions.Boolean,
            };

            Offset = new ProtocolCommand(this, "Offset", "OFFS")
            {
                Options = RealOptions.ChannelOffset_ScaleGreaterThan500m,
            };

            Range = new ProtocolCommand(this, "Range", "RANG")
            {
                Options = RealOptions.ChannelRange,
            };

            TCal = new ProtocolCommand(this, "TCal", "TCAL")
            {
                Options = RealOptions.ChannelTCal,
            };

            Scale = new ProtocolCommand(this, "Scale", "SCAL")
            {
                Options = RealOptions.ChannelScale,
            };

            Probe = new ProtocolCommand(this, "Probe", "PROB")
            {
                Options = StringOptions.ProbeRatio,
            };

            Units = new ProtocolCommand(this, "Units", "UNIT")
            {
                Options = StringOptions.Units,
            };

            Vernier = new ProtocolCommand(this, "Vernier", "VERN")
            {
                Options = BooleanOptions.Boolean,
            };

        }
    }
}
