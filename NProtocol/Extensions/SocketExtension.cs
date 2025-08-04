using System;
using System.Net.Sockets;

namespace NProtocol.Communication.Extensions
{
    public static class SocketExtension
    {
        /// <summary>
        /// Securely close the extension method
        /// </summary>
        /// <param name="socket"></param>
        public static void SafeClose(this Socket? socket)
        {
            try
            {
                if (socket is not null && socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Disconnect(false);
                }
            }
            catch { }
            finally
            {
                try
                {
                    socket?.Close();
                    socket?.Dispose();
                    socket = null;
                }
                catch { }
            }
        }
        /// <summary>
        /// Discard and return the data cache
        /// </summary>
        /// <param name="socket"></param>
        public static void DiscardBuffer(this Socket socket, int timeout = 100)
        {
            int preTimeout = socket.ReceiveTimeout;
            try
            {
                socket.ReceiveTimeout = timeout;
                var now = DateTime.UtcNow;
                byte[] discardBuffer = new byte[1024];
                while (true)
                {
                    int bytesRead = socket.Receive(discardBuffer, SocketFlags.None);
                    if (bytesRead == 0)
                    {
                        return;
                    }
                    if ((DateTime.Now - now).TotalSeconds > 1)
                    {
                        return;
                    }
                }
            }
            catch { }
            finally
            {
                socket.ReceiveTimeout = preTimeout;
            }
        }
    }
}
