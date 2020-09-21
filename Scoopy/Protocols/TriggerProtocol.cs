using Scoopy.Models;

namespace Scoopy.Protocols
{

    public class TriggerProtocol : ProtocolCommand
    {

        public override string Name => "Trigger";
        public override string Term => "TRIG";

        public override IProtocolCommandList Subcommands => new ProtocolCommandList {
            Mode, Coupling, Status, Sweep, Holdoff, NoiseReject, Position, Edge,
            Slope, Video, Pattern, Duration, Timeout, Runt, Windows, Delay, SHold,
            NEdge, //RS232, IIC, SPI
        };

        #region Subcommand properties

        public IProtocolCommand Mode { get; set; }

        public IProtocolCommand Coupling { get; set; }

        /// <summary>
        /// Query the current status
        /// </summary>
        public IProtocolCommand Status { get; set; }

        public IProtocolCommand Sweep { get; set; }

        /// <summary>
        /// Amount of time that is waited before the scope re-arms the trigger circuitry.
        /// This setting is not available when the trigger type is video, timeout, setup/hold, 
        /// nth edge, RS232, I2c or SPI.
        /// </summary>
        public IProtocolCommand Holdoff { get; set; }

        /// <summary>
        /// Enable or disable noise rejection.
        /// </summary>
        public IProtocolCommand NoiseReject { get; set; }

        /// <summary>
        /// Query the position in the internal memory that corresponds to the waveform
        /// trigger position. 
        /// -2 : not triggered, no position
        /// -1 : triggered outside the internal memory
        /// >0 : the position inthe internal memory
        /// </summary>
        public IProtocolCommand Position { get; set; }

        public TriggerEdgeCommands Edge { get; set; }

        public TriggerPulseCommands Pulse { get; set; }

        public TriggerSlopeCommands Slope { get; set; }

        public TriggerVideoCommands Video { get; set; }

        public TriggerPatternCommands Pattern { get; set; }

        public TriggerDurationCommands Duration { get; set; }

        public TriggerTimeoutCommands Timeout { get; set; }

        public TriggerRuntCommands Runt { get; set; }

        public TriggerWindowsCommands Windows { get; set; }

        public TriggerDelayCommands Delay { get; set; }

        public TriggerSHoldCommands SHold { get; set; }

        public TriggerNEdgeCommands  NEdge { get; set; }

        #endregion // subcommand properties

        public TriggerProtocol(IProtocolCommand parent) : base(parent)
        {
            Mode = new ProtocolCommand(this)
            {
                Name = "Mode",
                Term = "MODE",
                Options = new ModeStringOptions(),
            };

            Coupling = new ProtocolCommand(this)
            {
                Name = "Coupling",
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
            Status = new ProtocolCommand(this)
            {
                Name = "Status",
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

            Sweep = new ProtocolCommand(this)
            {
                Name = "Sweep",
                Term = "SWEep",
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
            Holdoff = new ProtocolCommand(this)
            {
                Name = "Holdoff",
                Term = "HOLDoff",
                Options = new RealOptions
                {
                    new RealOption(16 * UnitPrefix.nano, 10)
                }
            };

            /// <summary>
            /// Enable or disable noise rejection.
            /// </summary>
            NoiseReject = new ProtocolCommand(this)
            {
                Name = "NoiseReject",
                Term = "NREJect",
                Options = StringOptions.Boolean
            };

            /// <summary>
            /// Query the position in the internal memory that corresponds to the waveform
            /// trigger position. 
            /// -2 : not triggered, no position
            /// -1 : triggered outside the internal memory
            /// >0 : the position inthe internal memory
            /// </summary>
            Position = new ProtocolCommand(this)
            {
                Name = "Position",
                Term = "POSition",
            };

            Edge = new TriggerEdgeCommands(this);

            Pulse = new TriggerPulseCommands(this);

            Slope = new TriggerSlopeCommands(this);

            Video = new TriggerVideoCommands(this);

            Pattern = new TriggerPatternCommands(this);

            Duration = new TriggerDurationCommands(this);

            Timeout = new TriggerTimeoutCommands(this);

            Runt = new TriggerRuntCommands(this);

            Windows = new TriggerWindowsCommands(this);

            Delay = new TriggerDelayCommands(this);

            SHold = new TriggerSHoldCommands(this);

            NEdge = new TriggerNEdgeCommands(this);


        }

    }

    public class ModeStringOptions : StringOptions
    {
        public static readonly StringOption Edge = new StringOption(nameof(Edge), "EDGE");
        public static readonly StringOption Pulse = new StringOption(nameof(Pulse), "PULS");
        public static readonly StringOption Runt = new StringOption(nameof(Runt), "RUNT");
        public static readonly StringOption Window = new StringOption(nameof(Window), "WIND");
        public static readonly StringOption Nth_Edge = new StringOption(nameof(Nth_Edge), "NEDG");
        public static readonly StringOption Slope = new StringOption(nameof(Slope), "SLOP");
        public static readonly StringOption Video = new StringOption(nameof(Video), "VID");
        public static readonly StringOption Pattern = new StringOption(nameof(Pattern), "PATT");
        public static readonly StringOption Delay = new StringOption(nameof(Delay), "DEL");
        public static readonly StringOption Timeout = new StringOption(nameof(Timeout), "TIM");
        public static readonly StringOption Duration = new StringOption(nameof(Duration), "DUR");
        public static readonly StringOption Setup_Hold = new StringOption(nameof(Setup_Hold), "SHOL");
        public static readonly StringOption RS232 = new StringOption(nameof(RS232), "RS232");
        public static readonly StringOption I2C = new StringOption(nameof(I2C), "IIC");     // I2C ??
        public static readonly StringOption SPI = new StringOption(nameof(SPI), "SPI");
        public ModeStringOptions()
        {
            this.AddRange(new StringOptions { 
                Edge, Pulse, Runt, Window, Nth_Edge, Slope, Video, Pattern,
                Delay, Timeout, Duration, Setup_Hold, RS232, I2C, SPI
            });
        }
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
        public override IOptions Options => new RealOptions()
            {
                // TODO: Need a way to change this dynamically
                new RealOption(-6000, 6000)
            };

        public TriggerLevelCommand(IProtocolCommand parent) : base(parent)
        {
        }

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

        public TriggerSourceChannelCommand(IProtocolCommand parent) : base(parent)
        {
        }

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

        public TriggerSourceCommand(IProtocolCommand parent) : base(parent) 
        { }

    }

    public class TriggerSlopeCommand : ProtocolCommand
    {
        public override string Name => "SLOPe";
        public override string Term => "SLOP";
        public override IOptions Options => new StringOptions
        {
            new StringOption("Rising", "POS"),
            new StringOption("Falling", "NEG"),
            new StringOption("Both", "RFAL"),
        };
        public TriggerSlopeCommand(IProtocolCommand parent) : base(parent)
        {
        }

    }

    public class TriggerTimeCommand : ProtocolCommand
    {
        public override string Name => "TIMe";
        public override string Term => "TIM";

        public override IOptions Options => new RealOptions
        {
            new RealOption(16 * UnitPrefix.nano, 10),
        };
        public TriggerTimeCommand(IProtocolCommand parent) : base(parent)
        {
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
        public TriggerTypeCommand(IProtocolCommand parent) : base(parent)
        {
        }

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
        public TriggerWhenCommand(IProtocolCommand parent) : base(parent)
        {
        }

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

        public IProtocolCommand Source { get; set; }

        public IProtocolCommand Slope { get; set; }

        public IProtocolCommand Level { get; set; }

        public TriggerEdgeCommands(IProtocolCommand parent) : base(parent)
        {
            Source = new TriggerSourceCommand(this)
            {
                Description = "The trigger source in edge trigger",
            };
            Slope = new TriggerSlopeCommand(this);
            Level = new TriggerLevelCommand(this);
        }
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

        public IProtocolCommand Source { get; set; }

        public TriggerPulseCommands(IProtocolCommand parent) : base(parent)
        {
            Source = new TriggerSourceCommand(this);
        }
    }

    public class TriggerSlopeCommands : ProtocolCommand
    {
        public override string Name => "SLOPe";
        public override string Term => "SLOP";
        public override IProtocolCommandList Subcommands => new ProtocolCommandList()
        {
            Source, // TODO: WHEN, TIME, TUPPer, TLOWer, WINDow, ALEVel, BLEVel
        };

        public IProtocolCommand Source { get; set; }

        public TriggerSlopeCommands(IProtocolCommand parent) : base(parent)
        {
            Source = new ProtocolCommand(this)
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
    }

    public class TriggerVideoCommands : ProtocolCommand
    {
        public override string Name => "VIDeo";
        public override string Term => "VID";
        public override IProtocolCommandList Subcommands => new ProtocolCommandList()
        {
            Source, // TODO: POLarity, MODE, LINE, STANdard, LEVel
        };

        public IProtocolCommand Source { get; set; }

        public TriggerVideoCommands(IProtocolCommand parent) : base(parent)
        {
            Source = new TriggerSourceCommand(this);
        }
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

        public IProtocolCommand Pattern { get; set; }

        public IProtocolCommand Level { get; set; }

        public TriggerPatternCommands(IProtocolCommand parent) : base(parent)
        {
            Pattern = new ProtocolCommand(this)
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
            Level = new TriggerLevelCommand(this);
        }
    }

    public class TriggerDurationCommands : ProtocolCommand
    {
        public override string Name => "DURATion";
        public override string Term => "DURAT";
        public override IProtocolCommandList Subcommands => new ProtocolCommandList
        {
            Source, // TODO: TYPe, WHEN, TUPPer, TLOWer
        };

        public IProtocolCommand Source { get; set; }

        public TriggerDurationCommands(IProtocolCommand parent) : base(parent)
        {
            Source = new TriggerSourceCommand(this);
        }
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

        public IProtocolCommand Source { get; set; }

        public IProtocolCommand Slope { get; set; }

        public IProtocolCommand Time { get; set; }

        public TriggerTimeoutCommands(IProtocolCommand parent) : base(parent)
        {
            Source = new TriggerSourceCommand(this);

            Slope = new TriggerSlopeCommand(this);

            Time = new ProtocolCommand(this)
            {
                Name = "TIMe",
                Term = "TIM",
                Options = new RealOptions(16 * UnitPrefix.nano, 10),
            };

        }
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
        public IProtocolCommand Source { get; set; }

        public TriggerRuntCommands(IProtocolCommand parent) : base(parent)
        {
            Source = new TriggerSourceCommand(this);
        }
    }

    // Done
    public class TriggerWindowsCommands : ProtocolCommand
    {
        public override string Name => "Windows";
        public override string Term => "WIND";
        public override IProtocolCommandList Subcommands => new ProtocolCommandList() {
            Source, Slope, Position, Time, ALevel, BLevel
        };

        public IProtocolCommand Source { get; set; }

        public IProtocolCommand Slope { get; set; }

        public IProtocolCommand Time { get; set; }

        public IProtocolCommand Position { get; set; }

        public IProtocolCommand ALevel { get; set; }

        public IProtocolCommand BLevel { get; set; }

        public TriggerWindowsCommands(IProtocolCommand parent) : base(parent)
        {
            Source = new TriggerSourceChannelCommand(this);

            Slope = new TriggerSlopeCommand(this);

            Time = new TriggerTimeCommand(this)
            {
                Options = new RealOptions(8 * UnitPrefix.nano, 10)
            };

            Position = new ProtocolCommand(this)
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

            ALevel = new TriggerLevelCommand(this);

            BLevel = new TriggerLevelCommand(this);

        }
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

        public IProtocolCommand SourceA { get; set; }

        public IProtocolCommand SlopeA { get; set; }

        public IProtocolCommand SourceB { get; set; }

        public IProtocolCommand SlopeB { get; set; }

        public IProtocolCommand Typ { get; set; }

        public IProtocolCommand TUpper { get; set; }

        public IProtocolCommand TLower { get; set; }

        public IProtocolCommand Type { get; set; }

        public TriggerDelayCommands(IProtocolCommand parent) : base(parent)
        {
            SourceA = new TriggerSourceChannelCommand(this)
            {
                Name = "SourceA",
                Term = "SA",
            };

            SlopeA = new TriggerSlopeCommand(this)
            {
                Name = "SLOPeA",
                Term = "SLOPA",
            };

            SourceB = new TriggerSourceChannelCommand(this)
            {
                Name = "SourceB",
                Term = "SB",
            };

            SlopeB = new TriggerSlopeCommand(this)
            {
                Name = "SLOPeB",
                Term = "SLOPB",
            };

            Type = new TriggerTypeCommand(this);

            TUpper = new TriggerTimeCommand(this)
            {
                Name = "TUPPer",
                Term = "TUPP",
                Options = new RealOptions()
                {
                    new RealOption(16*UnitPrefix.nano, 10)
                }
            };

            TLower = new TriggerTimeCommand(this)
            {
                Name = "TLOWer",
                Term = "TLOW",
                Options = new RealOptions()
                {
                    new RealOption(8 * UnitPrefix.nano, 10)
                }
            };

        }
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

        public IProtocolCommand DataSource { get; set; }

        public IProtocolCommand ClockSource { get; set; }

        public IProtocolCommand Slope { get; set; }

        public IProtocolCommand Pattern { get; set; }

        public IProtocolCommand Type { get; set; }

        public IProtocolCommand SetupTime { get; set; }

        public IProtocolCommand HoldTime { get; set; }

        public TriggerSHoldCommands(IProtocolCommand parent) : base(parent)
        {

            DataSource = new TriggerSourceChannelCommand(this);

            ClockSource = new TriggerSourceChannelCommand(this);

            Slope = new TriggerSlopeCommand(this)
            {
                Options = new StringOptions
                {
                    new StringOption("POSitive", "POS"),
                    new StringOption("NEGative", "NEG"),
                }
            };
            
            Pattern = new TriggerPatternCommands(this)
            {
                Options = new StringOptions()
                {
                    new StringOption("H", "H"),
                    new StringOption("L", "L"),
                }
            };

            Type = new ProtocolCommand(this)
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

            SetupTime = new TriggerTimeCommand(this)
            {
                Name = "SHOLd",
                Term = "SHOL",
                Options = new RealOptions(8 * UnitPrefix.nano, 1),
            };

            HoldTime = new TriggerTimeCommand(this)
            {
                Name = "HTIMe",
                Term = "HTIM",
                Options = new RealOptions(8 * UnitPrefix.nano, 1),
            };

        }
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

        public IProtocolCommand Source { get; set; }

        public IProtocolCommand Slope { get; set; }

        /// <summary>
        /// Set or query the idle time in the Nth edge trigger.
        /// </summary>
        public IProtocolCommand Idle { get; set; }

        public IProtocolCommand Edge { get; set; }

        public IProtocolCommand Level { get; set; }

        public TriggerNEdgeCommands(IProtocolCommand parent) : base(parent)
        {
            Source = new TriggerSourceCommand(this);
            Slope = new TriggerSlopeCommand(this)
            {
                Options = new StringOptions {
                            new StringOption("POSitive", "POS"),
                            new StringOption("NEGative", "NEG"),
                        }
            };
            Idle = new TriggerTimeCommand(this)
            {
                Name = "IDLE",
                Term = "IDLE",
                Options = new RealOptions
                {
                    new RealOption(16 * UnitPrefix.nano, 10)
                }
            };
            Edge = new ProtocolCommand(this)
            {
                Name = "EDGE",
                Term = "EDGE",
                Options = new IntegerOptions { new IntegerOption(1, 65535) }
            }; 
            Level = new TriggerLevelCommand(this);
        }
    }

    #endregion Extended commands

}
