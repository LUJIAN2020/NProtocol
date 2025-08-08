using System;
using System.Net;
using System.Net.Sockets;
using NProtocol.Extensions;

namespace NProtocol.Connectors
{
    public class UdpConnector : IConnector
    {
        private int readTimeout = 1000;
        private int writeTimeout = 1000;
        private Socket client = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public EtherNetParameter Parameter { get; set; }
        public EndPoint? Local { get; private set; }
        public EndPoint? Remote { get; private set; }

        public UdpConnector(IParameter parameter)
        {
            if (parameter is EtherNetParameter para)
            {
                Parameter = para;
                Remote = new IPEndPoint(IPAddress.Parse(para.IP), para.Port);
            }
            else
            {
                throw new Exception(
                    "Type error. The parameter type is not the udp network parameter type"
                );
            }
        }

        public void Connect()
        {
            client.SendTimeout = WriteTimeout;
            client.ReceiveTimeout = ReadTimeout;
            Remote = new IPEndPoint(IPAddress.Parse(Parameter.IP), Parameter.Port);
        }

        public void Close() { }

        public void Dispose()
        {
            client.SafeClose();
        }

        public byte[] Read()
        {
            var buffer = new byte[1024];
            EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            int len = client.ReceiveFrom(buffer, ref remote);
            Local = client.LocalEndPoint;
            return buffer.Slice(0, len);
        }

        public int Write(byte[] buffer)
        {
            int len = 0;
            if (buffer.Length > 1024)
            {
                var segments = buffer.ChunkBy(1024);
                foreach (var segment in segments)
                {
                    len += client.SendTo(segment, Remote);
                }
            }
            else
            {
                len = client.SendTo(buffer, SocketFlags.None, Remote);
            }
            Local = client.LocalEndPoint;
            return len;
        }

        public void DiscardBuffer(int timeout = 100) { }

        public bool Connected => false;
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
    }
}
