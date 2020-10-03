using Nito.AsyncEx;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Skippy.Services
{
    public class TelnetService : ReactiveObject, IDisposable
    {
        private readonly AsyncLock BusyObject = new AsyncLock();

        public bool IsBusy { get; set; }

        public string Hostname { get; set; }

        public int Port { get; set; }

        /// <summary>
        /// Get whether automatic screenshots are enabled or set a request to enable or disable them.
        /// More than one source might reqeuest to enable/disable, so a counter tracks the number of
        /// requests, incrementing each time disabling is requested and decrementing each time enable
        /// is requested. When the counter is zero, autoscreenshot is enabled.
        /// </summary>
        public bool AutoGetScreenshotAfterCommand { 
            get { return _autoGetScreenshotAfterCommand == 0; }
            set {
                if (value && _autoGetScreenshotAfterCommand > 0)
                    _autoGetScreenshotAfterCommand--;
                else
                    _autoGetScreenshotAfterCommand++;
            }
        }
        private int _autoGetScreenshotAfterCommand;

        public int Timeout
        {
            get { return (int)_timeout.TotalMilliseconds; }
            set { _timeout = TimeSpan.FromMilliseconds(value); }
        }
        private TimeSpan _timeout;
        private bool disposedValue;

        [Reactive] public bool Connected { get; set; }

        public TelnetService()
        {
        }

        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public async Task<string> ConnectAsync(string hostname, int port)
        {
            Timeout = 5000;
            Hostname = hostname;
            Port = port;

            if (App.Mock)
            {
                await Task.Delay(1);
                Connected = true;
                return "Forcing mock connection";
            }

            using (await BusyObject.LockAsync())
            {
                IsBusy = true;
                using (var client = new TelnetClient(hostname, port, Timeout))
                {
                    if (await client.Connect())
                    {
                        var suppressDebug = false;
                        await client.WriteLineAsync(":*IDN?", suppressDebug);
                        var s = await client.ReadStringAsync(suppressDebug);
                        Connected = true;
                        return s;
                    }
                    Connected = false;
                    IsBusy = false;
                    return string.Empty;
                }
            }

        }

        private bool IsQuery(string command)
        {
            return command.EndsWith("?");
        }

        public async Task<string> SendCommandAsync(string command, bool getResponse)
        {
            if (App.Mock)
            {
                await Task.Delay(1);
                if (getResponse)
                    throw new Exception($"Cannot determine command response when mocking (command: '{command}'");
                else return "";
            }

            using (await BusyObject.LockAsync())
            {
                IsBusy = true;
                using (var client = new TelnetClient(Hostname, Port, Timeout))
                {
                    if (!await client.Connect())
                    {
                        return null;
                    }
                    var suppressDebug = command == ":TRIG:STAT?";
                    await client.WriteLineAsync(command, suppressDebug);
                    IsBusy = false;
                    if (getResponse)
                    {
                        return await client.ReadStringAsync(suppressDebug);
                    }
                    else
                    {
                        if (AutoGetScreenshotAfterCommand && !IsQuery(command))
                        {
                            Task.Run(async () => 
                            {
                                await Task.Delay(100);
                                await GetScreenshot();
                            }).NoAwait();
                        }
                        return "";
                    }
                }
            }
            

        }

        public async Task<byte[]> GetScreenshot()
        {
            if (App.Mock)
            {
                await Task.Delay(1);
                string resourceID = string.Format("Skippy.Resources.mock.png");
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                System.IO.Stream stream = assembly.GetManifestResourceStream(resourceID);
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    var bytes = memoryStream.ToArray();
                    return bytes;
                }
            }

            // skip if busy
            //if (IsBusy)
            //{
            //    return null;
            //}

            using (await BusyObject.LockAsync())
            using (var client = new TelnetClient(Hostname, Port, Timeout))
            {
                IsBusy = true;
                client.DebugEnabled = false;
                if (!await client.Connect())
                {
                    return null;
                }
                var suppressDebug = true;
                await client.WriteLineAsync(":DISP:DATA? ON,OFF,PNG", suppressDebug);

                // read header start character '#'
                var char1 = await client.ReadCharAsync();
                if (char1 == '#')
                {
                    // read header length
                    var char2 = await client.ReadCharAsync();
                    if (int.TryParse(char2.ToString(), out var headerLength))
                    {
                        // read length of data
                        var dataLengthString = await client.ReadStringAsync(headerLength);
                        if (int.TryParse(dataLengthString, out var dataLength))
                        {
                            var data = await client.ReadBytesAsync(dataLength);
                            //Debug.WriteLine("Got Screenshot");
                            IsBusy = false;
                            return data;
                        }
                        else
                        {
                            Debug.WriteLine("Screenshot response data length not recognized");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Screenshot response missing header length");
                    }
                }
                else
                {
                    Debug.WriteLine("Screenshot response missing header character");
                }
                client.DebugEnabled = true;
                IsBusy = false;
                return null;
            }
        
        }

    }
}
