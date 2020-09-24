using System;
using System.Collections.Generic;
using System.Text;

namespace Skippy.Protocols
{
    public class RootProtocol : ProtocolCommand
    {
        public override string Name => "Root"; // not used in the actual protocol

        /// <summary>
        /// The root commands don't have a path at the root level.
        /// The subcommands terms are the first terms that appear on the path.
        /// </summary>
        public override string Path => "";

        public override IProtocolCommandList Subcommands => new ProtocolCommandList
        {
            AutoScale, Clear, Run, Stop, Single, Force
        };

        public ProtocolCommand AutoScale { get; set; }

        public ProtocolCommand Status { get; set; }

        public ProtocolCommand Clear { get; set; }
        public ProtocolCommand Run { get; set; }
        public ProtocolCommand Stop { get; set; }
        public ProtocolCommand Single { get; set; }
        public ProtocolCommand Force { get; set; }

        public RootProtocol() : base(null)
        {
            /// <summary>
            /// Query the current status (this duplicates the Trigger Status command).
            /// </summary>
            Status = new ProtocolCommand(this, nameof(Status), "STAT")
            {
                Path=":TRIG:STAT",
                Options = new StringOptions
                {
                    new StringOption("TD", "TD"),
                    new StringOption("WAIT", "WAIT"),
                    new StringOption("RUN", "RUN"),
                    new StringOption("AUTO", "AUTO"),
                    new StringOption("STOP", "STOP")
                }
            };


            AutoScale = new ProtocolSimpleCommand(this, nameof(AutoScale), "AUT");
            Clear = new ProtocolSimpleCommand(this, nameof(Clear), "CLE");
            Run = new ProtocolSimpleCommand(this, nameof(Run), "RUN");
            Stop = new ProtocolSimpleCommand(this, nameof(Stop), "STOP");
            Single = new ProtocolSimpleCommand(this, nameof(Single), "SING");
            Force = new ProtocolSimpleCommand(this, nameof(Force), "TFOR");
        }

    }

}
