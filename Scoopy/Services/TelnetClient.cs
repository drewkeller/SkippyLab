using Nito.AsyncEx;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Scoopy.Services
{
    public class TelnetClient : IDisposable
    {
        private bool disposedValue;
        private static readonly AsyncLock BusyObject = new AsyncLock();

        #region Properties

        public bool DebugEnabled { get; set; } = true;

        public string Hostname { get; set; }

        public int Port { get; set; }

        public int Timeout
        {
            get { return (int)_timeout.TotalMilliseconds; }
            set { _timeout = TimeSpan.FromMilliseconds(value); }
        }
        private TimeSpan _timeout;
        private TcpClient TcpClient { get; set; }

        public bool IsBusy
        {
            get { return _isBusy; }
            private set {
                if (value) _isBusy = value;
                else
                {
                    Task.Delay(100);
                    _isBusy = value;
                }
            }
        }
        private bool _isBusy;

        //private CancellationToken CancellationToken { get; set; }

        #endregion Properties

        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    TcpClient.Close();
                    TcpClient.Dispose();
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

        public async Task<bool> Connect()
        {
            await WaitForNotBusy();
            using (await BusyObject.LockAsync())
            {
                IsBusy = true;
                TcpClient = new TcpClient();
                await TcpClient.ConnectAsync(Hostname, Port);
                IsBusy = false;
                return TcpClient.Connected;
            }
        }

        public void Disconnect()
        {
            TcpClient.Close();
        }

        public async Task WaitForNotBusy()
        {
            while (IsBusy) await Task.Delay(100);
        }

        #region Read

        public async Task<string> ReadStringAsync()
        {
            var response = await ReadStringAsync(TcpClient.GetStream());
            if (DebugEnabled)
            {
                Debug.WriteLine("Response: " + response?.TrimEnd());
            }
            return response;
        }

        public async Task<string> ReadStringAsync(int length)
        {
            await WaitForNotBusy();
            using (await BusyObject.LockAsync())
            {
                IsBusy = true;
                var buffer = await ReadBytesAsync(TcpClient.GetStream(), length);
                var response = Encoding.ASCII.GetString(buffer);
                if (DebugEnabled)
                {
                    Debug.WriteLine(response);
                }
                IsBusy = false;
                return response;
            }
        }

        private async Task<string> ReadStringAsync(NetworkStream stream)
        {
            if (!stream.CanRead) return string.Empty;

            await WaitForNotBusy();
            using (await BusyObject.LockAsync())
            {
                IsBusy = true;
                string str = null;
                byte[] buffer = new byte[1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    // read into the buffer from the network stream
                    var readTask = stream.ReadAsync(buffer, 0, buffer.Length);
                    var delayTask = Task.Delay(stream.ReadTimeout);
                    var task = await Task.WhenAny(readTask, delayTask);

                    if (task == readTask)
                    {
                        var numBytesRead = await readTask;
                        // write from the buffer to the memory stream
                        ms.Write(buffer, 0, numBytesRead);
                    }
                    str = Encoding.ASCII.GetString(ms.ToArray(), 0, (int)ms.Length);
                }
                IsBusy = false;
                return str;
            }
        }

        public async Task<byte> ReadByteAsync()
        {
            await WaitForNotBusy();
            using (await BusyObject.LockAsync())
            {
                IsBusy = true;
                var result = await ReadByteAsync(TcpClient.GetStream());
                IsBusy = false;
                return result;
            }
        }

        public async Task<char> ReadCharAsync()
        {
            await WaitForNotBusy();
            using (await BusyObject.LockAsync())
            {
                IsBusy = true;
                var result = (char)(await ReadByteAsync(TcpClient.GetStream()));
                IsBusy = false;
                return result;
            }
        }

        public async Task<byte[]> ReadBytesAsync(int length)
        {
            await WaitForNotBusy();
            using (await BusyObject.LockAsync())
            {
                IsBusy = true;
                var result = await ReadBytesAsync(TcpClient.GetStream(), length);
                IsBusy = false;
                return result;
            }
        }

        /// <summary>
        /// Callee must handle IsBusy
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static async Task<byte> ReadByteAsync(NetworkStream stream)
        {
            if (!stream.CanRead) return 0;

            var buffer = new byte[1];
            var readTask = stream.ReadAsync(buffer, 0, 1);
            var delayTask = Task.Delay(stream.ReadTimeout);
            var task = await Task.WhenAny(readTask, delayTask);

            if (task == readTask)
            {
                await readTask;
                return buffer[0];
            }

            return 0;
        }

        /// <summary>
        /// Callee must handle IsBusy
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private async Task<byte[]> ReadBytesAsync(NetworkStream stream, int length)
        {
            if (!stream.CanRead || length == 0) return new byte[0];

            byte[] buffer = new byte[length];
            using (MemoryStream ms = new MemoryStream())
            {
                var numBytesRead = 0;
                int totalBytesRead = 0;
                do
                {
                    numBytesRead = await ReadAsync(stream, buffer, 0, length - totalBytesRead);
                    totalBytesRead += numBytesRead;
                    ms.Write(buffer, 0, numBytesRead);
                } while (numBytesRead > 0 && totalBytesRead < length);

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Callee must handle IsBusy
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static async Task<int> ReadAsync(NetworkStream stream, byte[] buffer, int offset, int count)
        {
            if (!stream.CanRead) return 0;

            var readTask = stream.ReadAsync(buffer, offset, count);
            var delayTask = Task.Delay(stream.ReadTimeout);
            var task = await Task.WhenAny(readTask, delayTask);

            if (task == readTask)
                return await readTask;

            return 0;
        }

        #endregion // Read

        #region Write

        //private async Task WriteAsync(NetworkStream stream, string message)
        //{
        //    await WaitForNotBusy();
        //    using (await BusyObject.LockAsync())
        //    {
        //        IsBusy = true;
        //        var bytes = Encoding.ASCII.GetBytes(message);
        //        await stream.WriteAsync(bytes, 0, bytes.Length);
        //        IsBusy = false;
        //    }
        //}

        public async Task WriteLineAsync(string message)
        {
            await WriteLineAsync(TcpClient.GetStream(), message);
        }

        private async Task WriteLineAsync(NetworkStream stream, string message)
        {
            await WaitForNotBusy();
            using (await BusyObject.LockAsync())
            {
                IsBusy = true;
                if (DebugEnabled)
                {
                    Debug.WriteLine(message);
                }
                var bytes = Encoding.ASCII.GetBytes(message + "\r\n");
                await stream.WriteAsync(bytes, 0, bytes.Length);
                IsBusy = false;
            }
        }

        #endregion // Write

        public override string ToString()
        {
            return $"TelnetClient [${Hostname}: ${Port}]";
        }
    }

}
