using System;
using System.Collections.Generic;
using System.Text;

namespace Scoopy.Protocols
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

        public ProtocolCommand Clear { get; set; }
        public ProtocolCommand Run { get; set; }
        public ProtocolCommand Stop { get; set; }
        public ProtocolCommand Single { get; set; }
        public ProtocolCommand Force { get; set; }

        public RootProtocol() : base(null)
        {
            AutoScale = new ProtocolSimpleCommand(this, "AutoScale", "AUT");
            Clear = new ProtocolSimpleCommand(this, "Clear", "CLE");
            Run = new ProtocolSimpleCommand(this, "Run", "RUN");
            Stop = new ProtocolSimpleCommand(this, "Stop", "STOP");
            Single = new ProtocolSimpleCommand(this, "Single", "SING");
            Force = new ProtocolSimpleCommand(this, "Force", "TFOR");
        }

    }

}
