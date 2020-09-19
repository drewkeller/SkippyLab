﻿using Newtonsoft.Json;
using System;

namespace Scoopy.Protocols
{

    public interface IProtocolCommand
    {
        IProtocolCommand Parent { get; set; }

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
        /// The path formatter (this term and the parameter).
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Gets the parameter for formatting this part of the protocol path.
        /// </summary>
        object PathParameter { get; set; }

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

        string FormatPath();

    }

    /// <summary>
    /// Base protocol command. A command may contain either subcommands or options.
    /// </summary>
    public class ProtocolCommand : IProtocolCommand
    {
        [JsonIgnore]
        public IProtocolCommand Parent { get; set; }

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
        /// If a parameter is needed, this is the formatter for both
        /// the <see cref="Term"/> and <see cref="PathParameter"/>.
        /// Otherwise, the path automatically includes the <see cref="Term"/>.
        /// This does not normally need to be changed.
        /// </summary>
        public virtual string Path { get; set; }

        /// <summary>
        /// Gets the parameter for formatting this part of the protocol path.
        /// </summary>
        public object PathParameter { get; set; }

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

        public ProtocolCommand(IProtocolCommand parent)
        {
            Parent = parent;
        }

        public ProtocolCommand(IProtocolCommand parent, object pathParameter)
        {
            Parent = parent;
            PathParameter = pathParameter;
        }

        public virtual string FormatPath()
        {
            var parentFormat = Parent == null ? "" : Parent.FormatPath();
            if (PathParameter == null)
            {
                return $"{parentFormat}:{Term}";
            }
            else
            {
                return string.Format($"{parentFormat}:{Path}", PathParameter);
            }
        }

    }


}
