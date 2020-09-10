using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Scoopy
{
    public enum Coupling
    {
        AC, DC, GND
    }

    public enum ChannelUnits
    {
        Volts, Watts, Amps, Unknown
    }

    public class ScopeChannel
    {

        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool Inverted { get; set; }
        public Coupling Coupling { get; set; }

        public double Offset { get; set; }

        public double Range { get; set; }

        public double DelayCalibrationTime { get; set; }

        public int Scale { get; set; }

        /// <summary>
        /// Discrete steps: .01, .02, .05, .1, .2, .5, 1, 2, 5, 10, 20, 50, 100, 200, 500, 1000
        /// </summary>
        public double ProbeRatio { get; set; }

        public ChannelUnits Units { get; set; }

        public bool FineAdjust { get; set; }

    }

    /// <summary>
    /// Handles responses from ScpiCommands.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScpiCommandHandler<T> 
    { 

    }


    public class ScpiCommand
    {
        public string Command { get; set; }
        public bool WaitForResponse { get; set; }
        public string Response { get; set; }

        /// <summary>
        /// Send a command with no arguments. No response expected.
        /// </summary>
        public void SendCommand()
        {
        }

        /// <summary>
        /// Send a command, formatted by the provided arguments. 
        /// The number of arguments provided must match the number of formatting
        /// placeholders in the command's definition.
        /// </summary>
        /// <param name="args">The arguments to use for formatting the command.</param>
        public void SendCommand(params object[] args) 
        {
            var formatted = string.Format(Command, args);
        }

        /// <summary>
        /// Send a command and get a response.
        /// </summary>
        /// <typeparam name="T">The type of the response to get.</typeparam>
        /// <param name="defaultValue">In case the command fails, this is the return value.</param>
        /// <returns></returns>
        public T SendCommand<T>(T defaultValue)
        {
            return defaultValue;
        }

        /// <summary>
        /// Send a command, formatted by the provided arguments, and get a response.
        /// </summary>
        /// <typeparam name="T">The type of the response to get.</typeparam>
        /// <param name="defaultValue">In case the command fails, this is the return value.</param>
        /// <param name="args">The arguments to use for formatting the command.</param>
        /// <returns></returns>
        public T SendCommand<T>(T defaultValue, params object[] args) 
        {
            var formatted = string.Format(Command, args);
            return defaultValue;
        }



    }

    public class ScpiCommands
    {
        public readonly ScpiCommand SetChannelOffsetCommand = new ScpiCommand()
        {
            Command = ":CHAN{0}:OFF {1}",
        };
    }

}
