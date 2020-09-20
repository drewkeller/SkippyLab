//#define MOCK

using Nito.AsyncEx;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Scoopy.Services
{
    public class TelnetService : ReactiveObject, IDisposable
    {
        private readonly AsyncLock BusyObject = new AsyncLock();

        public bool IsBusy { get; set; }

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

#if MOCK
            await Task.Delay(1);
            Connected = true;
            return "Forcing mock connection";
#else
            using (await BusyObject.LockAsync())
            {
                IsBusy = true;
                using (var client = new TelnetClient(hostname, port, Timeout))
                {
                    if (await client.Connect())
                    {
                        await client.WriteLineAsync(":*IDN?");
                        var s = await client.ReadStringAsync();
                        Connected = true;
                        return s;
                    }
                    Connected = false;
                    IsBusy = false;
                    return string.Empty;
                }
            }
#endif
        }

        public async Task<string> SendCommandAsync(string command, bool getResponse)
        {
#if MOCK
            await Task.Delay(1);
            if (getResponse)
                throw new Exception($"Cannot determine command response when mocking (command: '{command}'");
            else return "";
#else
            using (await BusyObject.LockAsync())
            {
                IsBusy = true;
                using (var client = new TelnetClient(Hostname, Port, Timeout))
                {
                    if (!await client.Connect())
                    {
                        return null;
                    }
                    await client.WriteLineAsync(command);
                    IsBusy = false;
                    if (getResponse)
                    {
                        return await client.ReadStringAsync();
                    } else
                    {
                        return "";
                    }
                }
            }
#endif
        }

        public async Task<byte[]> GetScreenshot()
        {
#if MOCK
            await Task.Delay(1);
            return null;
#else
            // skip if busy
            if (IsBusy)
            {
                return null;
            }

            using (await BusyObject.LockAsync())
            using (var client = new TelnetClient(Hostname, Port, Timeout))
            {
                IsBusy = true;
                client.DebugEnabled = false;
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
#endif
        }

    }
}
