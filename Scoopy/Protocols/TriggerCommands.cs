using Scoopy.Models;

namespace Scoopy.Protocols
{

    public class TriggerCommands : ProtocolCommand
    {

        public override string Name => "TRIGger";
        public override string Term => "TRIG";

        public override IProtocolCommandList Subcommands => new ProtocolCommandList {
            Mode, Coupling, Status, Sweep, Holdoff, NoiseReject, Position, Edge,
            Slope, Video, Pattern, Duration, Timeout, Runt, Windows, Delay, SHold,
            NEdge, //RS232, IIC, SPI
        };



        public static readonly IProtocolCommand Mode = new ProtocolCommand
        {
            Name = "MODE",
            Term = "MODE",
            Options = new StringOptions()
            {
                new StringOption("EDGE", "EDGE"),
                new StringOption("PULSe", "PULS"),
                new StringOption("RUNT", "RUNT"),
                new StringOption("WIND", "WIND"),
                new StringOption("NEDG", "NEDG"),
                new StringOption("SLOPe", "SLOP"),
                new StringOption("VIDeo", "VID"),
                new StringOption("PATTern", "PATT"),
                new StringOption("DELay", "DEL"),
                new StringOption("TIMeout", "TIM"),
                new StringOption("DURation", "DUR"),
                new StringOption("SHOLd", "SHOL"),
                new StringOption("RS232", "RS232"),
                new StringOption("IIC", "IIC"),     // I2C ??
                new StringOption("SPI", "SPI"),
            }
        };

        public static readonly IProtocolCommand Coupling = new ProtocolCommand
        {
            Name = "COUPling",
            Term = "COUP",
            Options = new StringOptions()
            {
                new StringOption("AC", "AC"),
                new StringOption("DC", "DC"),
                new StringOption("LFReject", "LFR"),
                new StringOption("HFReject", "HFR")
            }
        };

        /// <summary>
        /// Query the current status
        /// </summary>
        public static readonly IProtocolCommand Status = new ProtocolCommand
        {
            Name = "STATus",
            Term = "STAT",
            Options = new StringOptions
            {
                new StringOption("TD", "TD"),
                new StringOption("WAIT", "WAIT"),
                new StringOption("RUN", "RUN"),
                new StringOption("AUTO", "AUTO"),
                new StringOption("STOP", "STOP")
            }
        };

        public static readonly IProtocolCommand Sweep = new ProtocolCommand
        {
            Name = "SWEep",
            Term = "SWE",
            Options = new StringOptions
            {
                new StringOption("AUTO", "AUTO"),
                new StringOption("NORMal", "NORM"),
                new StringOption("SINGle", "SING"),
            }
        };

        /// <summary>
        /// Amount of time that is waited before the scope re-arms the trigger circuitry.
        /// This setting is not available when the trigger type is video, timeout, setup/hold, 
        /// nth edge, RS232, I2c or SPI.
        /// </summary>
        public static readonly IProtocolCommand Holdoff = new ProtocolCommand
        {
            Name = "HOLDoff",
            Term = "HOLD",
            Options = new RealOptions
            {
                new RealOption(16 * UnitPrefix.nano, 10)
            }
        };

        /// <summary>
        /// Enable or disable noise rejection.
        /// </summary>
        public static readonly IProtocolCommand NoiseReject = new ProtocolCommand
        {
            Name = "NoiseReject",
            Term = "NREJ",
            Options = StringOptions.Boolean
        };

        /// <summary>
        /// Query the position in the internal memory that corresponds to the waveform
        /// trigger position. 
        /// -2 : not triggered, no position
        /// -1 : triggered outside the internal memory
        /// >0 : the position inthe internal memory
        /// </summary>
        public static readonly IProtocolCommand Position = new ProtocolCommand
        {
            Name = "POSition",
            Term = "POS",
        };

        public static readonly IProtocolCommand Edge = new TriggerEdgeCommands();

        public static readonly IProtocolCommand Pulse = new TriggerPulseCommands();

        public static readonly IProtocolCommand Slope = new TriggerSlopeCommands();

        public static readonly IProtocolCommand Video = new TriggerVideoCommands();

        public static readonly IProtocolCommand Pattern = new TriggerPatternCommands();

        public static readonly IProtocolCommand Duration = new TriggerDurationCommands();

        public static readonly IProtocolCommand Timeout = new TriggerTimeoutCommands();

        public static readonly IProtocolCommand Runt = new TriggerRuntCommands();

        public static readonly IProtocolCommand Windows = new TriggerWindowsCommands();

        public static readonly IProtocolCommand Delay = new TriggerDelayCommands();

        public static readonly IProtocolCommand SHold = new TriggerSHoldCommands();

        public static readonly IProtocolCommand NEdge = new TriggerNEdgeCommands();
    }

    #region Reused commands

    /// <summary>
    /// Range is ( +/- 5x VerticalScale - Offset)
    /// depending on probe ratio, scale is 1mv to 100V
    /// depending on probe ratio, offset is -1000 to 1000
    /// </summary>
    public class TriggerLevelCommand : ProtocolCommand
    {
        public override string Name => "Level";
        public override string Term => "LEVel";
        public override IOptions Options => new RealOptions
            {
                // TODO: Need a way to change this dynamically
                new RealOption(-6000, 6000)
            };
    }

    /// <summary>
    /// Contains only channels sources.
    /// </summary>
    public class TriggerSourceChannelCommand : ProtocolCommand
    {
        public override string Name => "SOURce";
        public override string Term => "SOUR";
        public override IOptions Options => new StringOptions
        {
            new StringOption("CH1", "CHAN1"),
            new StringOption("CH2", "CHAN2"),
            new StringOption("CH3", "CHAN3"),
            new StringOption("CH4", "CHAN4"),
        };
    }

    public class TriggerSourceCommand : ProtocolCommand
    {
        public override string Name => "SOURce";
        public override string Term => "SOUR";
        public override IOptions Options => new StringOptions
        {
            new StringOption("CH1", "CHAN1"),
            new StringOption("CH2", "CHAN2"),
            new StringOption("CH3", "CHAN3"),
            new StringOption("CH4", "CHAN4"),
            new StringOption("AC", "AC"),
            new StringOption("D0", "D0"),
            new StringOption("D1", "D1"),
            new StringOption("D2", "D2"),
            new StringOption("D3", "D3"),
            new StringOption("D4", "D4"),
            new StringOption("D5", "D5"),
            new StringOption("D6", "D6"),
            new StringOption("D7", "D7"),
            new StringOption("D8", "D8"),
            new StringOption("D9", "D9"),
            new StringOption("D10", "D10"),
            new StringOption("D11", "D11"),
            new StringOption("D12", "D12"),
            new StringOption("D13", "D13"),
            new StringOption("D14", "D14"),
            new StringOption("D15", "D15"),
        };
    }

    public class TriggerSlopeCommand : ProtocolCommand
    {
        public override string Name => "SLOPe";
        public override string Term => "SLOP";
        public override IOptions Options => new StringOptions
        {
            new StringOption("POSitive", "POS"),
            new StringOption("NEGative", "NEG"),
            new StringOption("R/F Edge", "RFALI"),
        };
    }

    public class TriggerTimeCommand : ProtocolCommand
    {
        public override string Name => "TIMe";
        public override string Term => "TIM";

        public override IOptions Options => new RealOptions
        {
            new RealOption(16 * UnitPrefix.nano, 10),
        };
        public TriggerTimeCommand() { }
        public TriggerTimeCommand(double min, double max)
        {
            Options.Add(new RealOption(min, max));
        }
    };

    public class TriggerTypeCommand : ProtocolCommand
    {
        public override string Name => "TYPe";
        public override string Term => "TYP";
        public override IOptions Options => new StringOptions
        {
            new StringOption("GREater", "GRE"),
            new StringOption("LESS", "LESS"),
            new StringOption("GLESs", "GLES"),
            new StringOption("GOUT", "GOUT"),
        };
    }

    public class TriggerWhenCommand : ProtocolCommand
    {
        public override string Name => "WHEN";
        public override string Term => "WHEN";

        public override IOptions Options => new StringOptions {
            new StringOption("STARt", "STAR"),
            new StringOption("ERRor", "ERR"),
            new StringOption("PARity", "PAR"),
            new StringOption("DATA", "DATA"),
        };
    }

    #endregion Reused commands

    #region Extended commands

    // Done
    public class TriggerEdgeCommands : ProtocolCommand
    {
        public override string Name => "EDGe";
        public override string Term => "EDG";

        public override IProtocolCommandList Subcommands => new ProtocolCommandList
        {
            Source, Slope, Level
        };

        public static readonly IProtocolCommand Source = new TriggerSourceCommand()
        { Description = "The trigger source in edge trigger" };

        public static readonly IProtocolCommand Slope = new TriggerSlopeCommand();

        public static readonly IProtocolCommand Level = new TriggerLevelCommand();
    }

    /// <summary>
    /// Set or query the trigger source in pulse width.
    /// </summary>
    public class TriggerPulseCommands : ProtocolCommand
    {
        public override string Name => "PULSe";
        public override string Term => "PULS";
        public override IProtocolCommandList Subcommands => new ProtocolCommandList()
        {
            Source, // TODO: WHEN, WIDTh, UWIDth, LWIDth, LEVel
        };

        public static readonly IProtocolCommand Source = new TriggerSourceCommand();

    }

    public class TriggerSlopeCommands : ProtocolCommand
    {
        public override string Name => "SLOPe";
        public override string Term => "SLOP";
        public override IProtocolCommandList Subcommands => new ProtocolCommandList()
        {
            Source, // TODO: WHEN, TIME, TUPPer, TLOWer, WINDow, ALEVel, BLEVel
        };

        public static readonly IProtocolCommand Source = new ProtocolCommand()
        {
            Name = "SOURce",
            Term = "SOUR",
            Options = new StringOptions
            {
                new StringOption("CH1", "CHAN1"),
                new StringOption("CH2", "CHAN2"),
                new StringOption("CH3", "CHAN3"),
                new StringOption("CH4", "CHAN4"),
            }
        };

    }

    public class TriggerVideoCommands : ProtocolCommand
    {
        public override string Name => "VIDeo";
        public override string Term => "VID";
        public override IProtocolCommandList Subcommands => new ProtocolCommandList()
        {
            Source, // TODO: POLarity, MODE, LINE, STANdard, LEVel
        };

        public static readonly IProtocolCommand Source = new TriggerSourceCommand();
    }

    // done
    public class TriggerPatternCommands : ProtocolCommand
    {
        public override string Name => "PATTern";
        public override string Term => "PATT";
        public override IProtocolCommandList Subcommands => new ProtocolCommandList()
        {
            Pattern, Level
        };

        public static readonly IProtocolCommand Pattern = new ProtocolCommand()
        {
            Name = "PATTern",
            Term = "PATT",
            Options = new StringOptions
            {
                new StringOption("H", "H"),
                new StringOption("L", "L"),
                new StringOption("X", "X"),
                new StringOption("R", "R"),
                new StringOption("F", "F"),
            }
        };

        public static readonly IProtocolCommand Level = new TriggerLevelCommand();
    }

    public class TriggerDurationCommands : ProtocolCommand
    {
        public override string Name => "DURATion";
        public override string Term => "DURAT";
        public override IProtocolCommandList Subcommands => new ProtocolCommandList
        {
            Source, // TODO: TYPe, WHEN, TUPPer, TLOWer
        };

        public static readonly IProtocolCommand Source = new TriggerSourceCommand();
    }

    // Done
    public class TriggerTimeoutCommands : ProtocolCommand
    {
        public override string Name => "TIMEout";
        public override string Term => "TIME";
        public override IProtocolCommandList Subcommands => new ProtocolCommandList
        {
            Source, Slope, Time
        };

        public static readonly IProtocolCommand Source = new TriggerSourceCommand();

        public static readonly IProtocolCommand Slope = new TriggerSlopeCommand();

        public static readonly IProtocolCommand Time = new ProtocolCommand()
        {
            Name = "TIMe",
            Term = "TIM",
            Options = new RealOptions
            {
                new RealOption(16 * UnitPrefix.nano, 10),
            }
        };
    }

    public class TriggerRuntCommands : ProtocolCommand
    {
        public override string Name => "RUNT";

        public override string Term => "RUNT";

        public override IProtocolCommandList Subcommands => new ProtocolCommandList
        {
            Source, // TODO: Polarity, When, WUpper, WLOWer, ALEVel, BLEVel
        };

        // only channel sources
        public static readonly IProtocolCommand Source = new TriggerSourceCommand();
    }

    // Done
    public class TriggerWindowsCommands : ProtocolCommand
    {
        public override string Name => "Windows";
        public override string Term => "WIND";
        public override IProtocolCommandList Subcommands => new ProtocolCommandList() {
            Source, Slope, Position, Time, ALevel, BLevel
        };

        public static readonly IProtocolCommand Source = new TriggerSourceChannelCommand();

        public static readonly IProtocolCommand Slope = new TriggerSlopeCommand();

        public static readonly IProtocolCommand Time = new TriggerTimeCommand(8 * UnitPrefix.nano, 10);

        public static readonly IProtocolCommand Position = new ProtocolCommand()
        {
            Name = "POSition",
            Term = "POS",
            Options = new StringOptions()
            {
                new StringOption("EXIT", "EXIT"),
                new StringOption("ENTER", "ENTER"),
                new StringOption("TIMe", "TIM"),
            }
        };

        public static readonly IProtocolCommand ALevel = new TriggerLevelCommand();

        public static readonly IProtocolCommand BLevel = new TriggerLevelCommand();

    }

    // Done
    public class TriggerDelayCommands : ProtocolCommand
    {
        public override string Name => "DELay";
        public override string Term => "DEL";
        public override IProtocolCommandList Subcommands => new ProtocolCommandList
        {
            SourceA, SlopeA, SourceB, SlopeB, Type
        };

        public static readonly IProtocolCommand SourceA = new TriggerSourceChannelCommand()
        {
            Name = "SourceA",
            Term = "SA",
        };

        public static readonly IProtocolCommand SlopeA = new TriggerSlopeCommand()
        {
            Name = "SLOPeA",
            Term = "SLOPA",
        };

        public static readonly IProtocolCommand SourceB = new TriggerSourceChannelCommand()
        {
            Name = "SourceB",
            Term = "SB",
        };

        public static readonly IProtocolCommand SlopeB = new TriggerSlopeCommand()
        {
            Name = "SLOPeB",
            Term = "SLOPB",
        };

        public static readonly IProtocolCommand Type = new TriggerTypeCommand();

        public static readonly IProtocolCommand TUpper = new TriggerTimeCommand()
        {
            Name = "TUPPer",
            Term = "TUPP",
            Options = new RealOptions()
            {
                new RealOption(16*UnitPrefix.nano, 10)
            }
        };

        public static readonly IProtocolCommand TLower = new TriggerTimeCommand()
        {
            Name = "TLOWer",
            Term = "TLOW",
            Options = new RealOptions()
            {
                new RealOption(8 * UnitPrefix.nano, 10)
            }
        };

    }

    // Done
    public class TriggerSHoldCommands : ProtocolCommand
    {
        public override string Name => "SHOLd";
        public override string Term => "SHOL";

        public override IProtocolCommandList Subcommands => new ProtocolCommandList
        {
            DataSource, ClockSource, Slope, Pattern, Type, SetupTime, HoldTime,
        };

        public static readonly IProtocolCommand DataSource = new TriggerSourceChannelCommand();

        public static readonly IProtocolCommand ClockSource = new TriggerSourceChannelCommand();

        public static readonly IProtocolCommand Slope = new TriggerSlopeCommand()
        {
            Options = new StringOptions
            {
                new StringOption("POSitive", "POS"),
                new StringOption("NEGative", "NEG"),
            }
        };

        public static readonly IProtocolCommand Pattern = new TriggerPatternCommands()
        {
            Options = new StringOptions()
            {
                new StringOption("H", "H"),
                new StringOption("L", "L"),
            }
        };

        public static readonly IProtocolCommand Type = new ProtocolCommand()
        {
            Name = "TYP",
            Term = "TYP",
            Options = new StringOptions
            {
                new StringOption("SETup", "SET"),
                new StringOption("HOLd", "HOL"),
                new StringOption("SETHOLd", "SETHOL"),
            }
        };

        public static readonly IProtocolCommand SetupTime = new TriggerTimeCommand(8 * UnitPrefix.nano, 1)
        {
            Name = "SHOLd",
            Term = "SHOL"
        };

        public static readonly IProtocolCommand HoldTime = new TriggerTimeCommand(8 * UnitPrefix.nano, 1)
        {
            Name = "HTIMe",
            Term = "HTIM"
        };

    }

    // Done
    public class TriggerNEdgeCommands : ProtocolCommand
    {
        public override string Name => "NEDGe";
        public override string Term => "NEDG";
        public override IProtocolCommandList Subcommands => new ProtocolCommandList
        {
            Source, Slope, Idle, Edge, Level
        };

        public static readonly IProtocolCommand Source = new TriggerSourceCommand();

        public static readonly IProtocolCommand Slope = new TriggerSlopeCommand()
        {
            Options = new StringOptions {
                new StringOption("POSitive", "POS"),
                new StringOption("NEGative", "NEG"),
            }
        };

        /// <summary>
        /// Set or query the idle time in the Nth edge trigger.
        /// </summary>
        public static readonly IProtocolCommand Idle = new TriggerTimeCommand(16 * UnitPrefix.nano, 10)
        {
            Name = "IDLE",
            Term = "IDLE",
        };

        public static readonly IProtocolCommand Edge = new ProtocolCommand()
        {
            Name = "EDGE",
            Term = "EDGE",
            Options = new IntegerOptions { new IntegerOption(1, 65535) }
        };

        public static readonly IProtocolCommand Level = new TriggerLevelCommand();
    }

    #endregion Extended commands

}
