using System;

namespace NProtocol.Connectors
{
    public interface IConnector : IDisposable
    {
        bool Connected { get; }
        int ReadTimeout { get; set; }
        int WriteTimeout { get; set; }
        string DriverId { get; }
        void Connect();
        void Close();
        ReadOnlySpan<byte> Read();
        int Write(byte[] buffer);
        void DiscardBuffer(int timeout);
    }
}
