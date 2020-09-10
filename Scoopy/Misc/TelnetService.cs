using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Scoopy
{
    public class TelnetService : ReactiveObject, IDisposable
    {
        public string Hostname { get; set; }

        public int Port { get; set; }

        public int Timeout
        {
            get { return (int)_timeout.TotalMilliseconds; }
            set { _timeout = TimeSpan.FromMilliseconds(value); }
        }
        private TimeSpan _timeout;
        private bool disposedValue;

        [Reactive] public bool Connected { get; set; }

        public TelnetService() { }

        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //Stream.Dispose();
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

            Connected = true;
            return "Forcing mock connection";

            using (var client = new TelnetClient(hostname, port, Timeout))
            {
                if (await client.Connect())
                {
                    await client.WriteLineAsync(":*IDN?");
                    var s = await client.ReadStringAsync();
                    Connected = true;
                    return s;
                }
            }
            Connected = false;
            return string.Empty;
        }

        public async Task<string> SendCommandAsync(string command, bool getResponse)
        {
            using (var client = new TelnetClient(Hostname, Port, Timeout))
            {
                if (!await client.Connect())
                {
                    return null;
                }
                await client.WriteLineAsync(command);
                return getResponse ? await client.ReadStringAsync() : "";
            }
        }

        public async Task<byte[]> GetScreenshot()
        {
            using (var client = new TelnetClient(Hostname, Port, Timeout))
            {
                if (!await client.Connect())
                {
                    return null;
                }
                await client.WriteLineAsync(":DISP:DATA? ON,OFF,PNG");

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
                            return data;
                        }
                    }
                }
            }
            return null;
        }

    }
}
