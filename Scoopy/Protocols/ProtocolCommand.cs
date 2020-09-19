using System;

namespace Scoopy.Protocols
{

    public interface IProtocolCommand
    {
        /// <summary>
        /// The user friendly name of the command.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The term that appears in protocol commands/responses.
        /// </summary>
        string Term { get; set; }

        /// <summary>
        /// Information about the command.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// The type of parameter in the command or response.
        /// </summary>
        Type ParameterType { get; set; }

        /// <summary>
        /// If this is a command category, the list of subcommands or subcategories.
        /// </summary>
        IProtocolCommandList Subcommands { get; set; }

        /// <summary>
        /// If this is a command, a list of acceptable values or range of values.
        /// </summary>
        IOptions Options { get; set; }
    }

    /// <summary>
    /// Base protocol command. A command may contain either subcommands or options.
    /// </summary>
    public class ProtocolCommand : IProtocolCommand
    {
        /// <summary>
        /// The user friendly name of the command.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// The term that appears in protocol commands/responses.
        /// </summary>
        public virtual string Term { get; set; }

        /// <summary>
        /// Information about the command.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// The type of parameter in the command or response.
        /// </summary>
        public virtual Type ParameterType { get; set; }

        /// <summary>
        /// If this is a command category, the list of subcommands or subcategories.
        /// </summary>
        public virtual IProtocolCommandList Subcommands { get; set; }

        /// <summary>
        /// If this is a command, a list of acceptable values or range of values.
        /// </summary>
        public virtual IOptions Options { get; set; }
    }


}
