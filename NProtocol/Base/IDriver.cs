using System;
using NProtocol.Enums;

namespace NProtocol.Base
{
    public interface IDriver : IDisposable
    {
        event Action<string, RW, byte[]>? LogReadWriteRaw;
        int ReadTimeout { get; set; }
        int WriteTimeout { get; set; }
        bool Connected { get; }
        string DriverId { get; }
        void Connect();
        void Close();
        int Write(byte[] writeData);
        byte[] Read();
        void DiscardBuffer(int timeout);
    }
}
