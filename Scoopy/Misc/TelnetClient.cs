using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Scoopy
{
    public class TelnetClient : IDisposable
    {
        private bool disposedValue;

        #region Properties

        public string Hostname { get; set; }

        public int Port { get; set; }

        public int Timeout
        {
            get { return (int)_timeout.TotalMilliseconds; }
            set { _timeout = TimeSpan.FromMilliseconds(value); }
        }
        private TimeSpan _timeout;
        private TcpClient TcpClient { get; set; }

        private CancellationToken CancellationToken { get; set; }

        #endregion Properties

        #region Constructor

        public TelnetClient(string hostname, int port, int timeout)
        {
            Hostname = hostname;
            Port = port;
        }

        #endregion

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
            TcpClient = new TcpClient();
            await TcpClient.ConnectAsync(Hostname, Port);
            return TcpClient.Connected;
        }

        #region Read

        public async Task<string> ReadStringAsync()
        {
            return await ReadStringAsync(TcpClient.GetStream());
        }

        public async Task<string> ReadStringAsync(int length)
        {
            var buffer = await ReadBytesAsync(TcpClient.GetStream(), length);
            return Encoding.ASCII.GetString(buffer);
        }

        private async Task<string> ReadStringAsync(NetworkStream stream)
        {
            if (!stream.CanRead) return string.Empty;

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
            return str;
        }

        public async Task<byte> ReadByteAsync()
        {
            return await ReadByteAsync(TcpClient.GetStream());
        }
        public async Task<char> ReadCharAsync()
        {
            return (char)(await ReadByteAsync(TcpClient.GetStream()));
        }

        public async Task<byte[]> ReadBytesAsync(int length)
        {
            return await ReadBytesAsync(TcpClient.GetStream(), length);
        }

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

        private async Task WriteAsync(NetworkStream stream, string message)
        {
            var bytes = Encoding.ASCII.GetBytes(message);
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }

        public async Task WriteLineAsync(string message)
        {
            await WriteLineAsync(TcpClient.GetStream(), message);
        }

        private async Task WriteLineAsync(NetworkStream stream, string message)
        {
            var bytes = Encoding.ASCII.GetBytes(message + "\r\n");
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }

        #endregion // Write
    }

}
