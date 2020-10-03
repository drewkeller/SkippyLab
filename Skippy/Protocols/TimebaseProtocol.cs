using Skippy.Models;

namespace Skippy.Protocols
{
    public class TimebaseProtocol : ProtocolCommand
    {
        public override string Name => "Timebase";
        public override string Term => "TIM";
        public override string Path => ":TIM";
        public override IProtocolCommandList Subcommands => new ProtocolCommandList() {
            Delay, Offset, Scale, Mode
        };

        #region Subcommands

        public TimebaseDelayCommands Delay { get; set; }
        public ProtocolCommand Offset { get; set; }
        public ProtocolCommand Scale { get; set; }
        public ProtocolCommand Mode { get; set; }

        #endregion Subcommands

        public TimebaseProtocol(IProtocolCommand parent) : base(parent)
        {
            Delay = new TimebaseDelayCommands(this);

            Offset = new ProtocolCommand(this, "Offset", "OFFS")
            {
                Path = ":MAIN:OFFS",
                // depends on whether scope is in slow sweep mode
                // Normal    : (-0.5 x MemDepth / SampleRate) to 1s
                // Slow sweep: (-MemDepth/SampleRate) to (1s + 0.5 x MemDepth/SampleRate)
                // Highest memdepth is 24000000
                // MemDepth = SampleRate x WaveformLength
                // WaveformLength = Scale x number of scales
                // number of scales for DS1000Z is 12
                Options = new RealOptions(-12, 12)
            };

            Scale = new ProtocolCommand(this, "Scale", "SCAL")
            {
                Path = ":MAIN:SCAL",
                // YT mode  : 5ns/div to 50s/div in 1,2,5 steps
                // Roll mode: 200ms/div to 50s/div in 1,2,5 steps
                Options = TimebaseScaleOptions.YT,
            };

            Mode = new ProtocolCommand(this, "Mode", "MODE")
            {
                Options = new StringOptions
                {
                    new StringOption("Main", "MAIN"),
                    new StringOption("XY", "XY"),
                    new StringOption("Roll", "ROLL"),
                }
            };
        }

    }

    public class TimebaseScaleOptions : StringOptions, IOptions
    {
        public static IOptions Roll = new StringOptions()
        {
            new StringOption("200m", ".2"),
            new StringOption("500m", ".5"),
            new StringOption("1", "1"),
            new StringOption("2", "2"),
            new StringOption("5", "5"),
            new StringOption("10", "10"),
            new StringOption("20", "20"),
            new StringOption("50", "50"),
        };

        public static IOptions YT = new StringOptions() {
            new StringOption("5n", "5.0000000e-09"),
            new StringOption("10n", "1.0000000e-08"),
            new StringOption("20n", "2.0000000e-08"),
            new StringOption("50n", "5.0000000e-08"),
            new StringOption("100n", "1.0000000e-07"),
            new StringOption("200n", "2.0000000e-07"),
            new StringOption("500n", "5.0000000e-07"),
            new StringOption("1u", "1.0000000e-06"),
            new StringOption("2u", "2.0000000e-06"),
            new StringOption("5u", "5.0000000e-06"),
            new StringOption("10u", "1.0000000e-05"),
            new StringOption("20u", "2.0000000e-05"),
            new StringOption("50u", "5.0000000e-05"),
            new StringOption("100u", "1.0000000e-04"),
            new StringOption("200u", "2.0000000e-04"),
            new StringOption("500u", "5.0000000e-04"),
            new StringOption("1m", "1.0000000e-03"),
            new StringOption("2m", "2.0000000e-03"),
            new StringOption("5m", "5.0000000e-03"),
            new StringOption("10m", "1.0000000e-02"),
            new StringOption("20m", "2.0000000e-02"),
            new StringOption("50m", "5.0000000e-02"),
            new StringOption("100m", "1.0000000e-01"),
            new StringOption("200m", "2.0000000e-01"),
            new StringOption("500m", "5.0000000e-01"),
            new StringOption("1", "1.0000000e-00"),
            new StringOption("2", "2.0000000e+00"),
            new StringOption("5", "5.0000000e+00"),
            new StringOption("10", "1.0000000e+01"),
            new StringOption("20", "2.0000000e+01"),
            new StringOption("50", "5.0000000e+01"),
            };
    }

    #region Reusable commands

    public class TimebaseEnableCommand : ProtocolCommand
    {
        public override string Name => "Enable";
        public override string Term => "ENAB";

        public override IOptions Options => new BooleanOptions();

        public TimebaseEnableCommand(IProtocolCommand parent) : base(parent)
        {
        }

    }

    #endregion Reusable commands

    public class TimebaseDelayCommands : ProtocolCommand
    {
        public override string Name => "Delay";
        public override string Term => "DEL";

        public override IProtocolCommandList Subcommands => new ProtocolCommandList() {
            Enable, Offset, Scale
        };

        public TimebaseEnableCommand Enable { get; set; }
        public ProtocolCommand Offset { get; set; }
        public ProtocolCommand Scale { get; set; }

        public TimebaseDelayCommands(IProtocolCommand parent) : base(parent)
        {
            Enable = new TimebaseEnableCommand(this);

            Offset = new ProtocolCommand(this, "Offset", "OFFS")
            {
                // LeftTime = 6x MainScale - MainOffset
                // RightTime = 6x MainScale - MainOffset
                // DelayRange = 12x DelayScale
                Options = new RealOptions(-12, 12)
            };

            Scale = new ProtocolCommand(this, "Scale", "SCAL")
            {
                // max scale: MainScale
                // min scale: 50 / (SampleRate x AmplificationFactor)
                // Amplification factor is (10 x the sum of): 
                //    Number of enabled analog channels
                //    Number of analog channels set as trigger sources
                //    Number of enabled digital channel groups (D0 to D7 is one, D8 to D15 is the other)
                // TODO: This needs more work, I'm just guessing here
                Options = new RealOptions(10 * UnitPrefix.nano, 50)
            };
        }

    }

}
