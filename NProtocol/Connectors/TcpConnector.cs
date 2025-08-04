using NProtocol.Communication.Extensions;
using System;
using System.Net;
using System.Net.Sockets;

namespace NProtocol.Communication.Connectors
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
                throw new ArgumentException("Type error. The parameter type is not the tcp network parameter type", nameof(parameter));
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
        public byte[] Read()
        {
            ValidateConnected();
            var buffer = new byte[1024];
            int len = client.Receive(buffer);
            return buffer.Slice(0, len);
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
        public string DriverId => $"LocalEndPoint:{client.LocalEndPoint},RemoteEndPoint:{client.RemoteEndPoint}";
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