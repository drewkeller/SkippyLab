using ReactiveUI;
using ReactiveUI.Fody.Helpers;

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

    public class ScopeChannel : ReactiveObject
    {
        public int ChannelNumber { get; set; }
        
        public string Name { get; set; }

        [Reactive] public bool IsActive { get; set; }

        [Reactive] public bool Inverted { get; set; }

        [Reactive] public Coupling Coupling { get; set; }

        /// <summary>
        /// The offset Value is related to the current vertical scale and probe ratio.
        /// f( 1X, scale >= 500) = -100 to +100
        /// f( 1X, scale <  500) = -2 to +2
        /// f(10X, scale >= 5) = -1000 to 1000
        /// f(10X, scale <  5) = -20 to 20
        /// </summary>
        [Reactive] public double Offset { get; set; }

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
