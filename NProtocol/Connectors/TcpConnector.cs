using NProtocol.Extensions;
using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;

namespace NProtocol.Connectors
{
    public class TcpConnector : IConnector
    {
        private int readTimeout = 1000;
        private int writeTimeout = 1000;
        private Socket client = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public EtherNetParameter Parameter { get; set; }
        public EndPoint? Local => client != null ? client.LocalEndPoint : default;
        public EndPoint? Remote => client != null ? client.RemoteEndPoint : default;

        public TcpConnector(IParameter parameter)
        {
            if (parameter is EtherNetParameter para)
            {
                Parameter = para;
            }
            else
            {
                throw new ArgumentException(
                    "Type error. The parameter type is not the tcp network parameter type",
                    nameof(parameter)
                );
            }
        }

        public void Connect()
        {
            client.SendTimeout = WriteTimeout;
            client.ReceiveTimeout = ReadTimeout;
            client.Connect(Parameter.IP, Parameter.Port);
        }

        public void Close()
        {
            client.Close();
        }

        public void Dispose()
        {
            client.SafeClose();
        }

        public ReadOnlySpan<byte> Read()
        {
            ValidateConnected();
            var rentBuf = ArrayPool<byte>.Shared.Rent(1024);
            try
            {
                int len = client.Receive(rentBuf);
                return rentBuf.AsSpan().Slice(0, len);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rentBuf);
            }
        }

        public int Write(byte[] buffer)
        {
            ValidateConnected();
            if (buffer.Length > 1024)
            {
                int len = 0;
                var segments = buffer.ChunkBy(1024);
                foreach (var segment in segments)
                {
                    len += client.Send(segment);
                }
                return len;
            }
            else
            {
                return client.Send(buffer);
            }
        }

        public bool Connected => client != null && client.Connected;
        public string DriverId => $"LocalEndPoint:{Local},RemoteEndPoint:{Remote}";
        public int ReadTimeout
        {
            get { return readTimeout; }
            set
            {
                readTimeout = value;
                if (client != null)
                {
                    client.ReceiveTimeout = value;
                }
            }
        }
        public int WriteTimeout
        {
            get { return writeTimeout; }
            set
            {
                writeTimeout = value;
                if (client != null)
                {
                    client.SendTimeout = value;
                }
            }
        }

        private void ValidateConnected()
        {
            if (!client.Connected)
            {
                client.SafeClose();
                client = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Connect();
            }
        }

        public void DiscardBuffer(int timeout = 100)
        {
            client.DiscardBuffer(timeout);
        }
    }
}
