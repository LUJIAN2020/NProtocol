using System;

namespace NProtocol.Base
{
    public interface IDriver : IDisposable
    {
        LogReadWriteRawHandler? LogReadWriteRaw { get; set; }
        int ReadTimeout { get; set; }
        int WriteTimeout { get; set; }
        bool Connected { get; }
        string DriverId { get; }
        void Connect();
        void Close();
        int Write(byte[] writeData);
        ReadOnlySpan<byte> Read();
        void DiscardBuffer(int timeout);
    }
}
