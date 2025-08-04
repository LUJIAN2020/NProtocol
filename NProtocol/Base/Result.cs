using NProtocol.Communication.Extensions;
using System;

namespace NProtocol.Communication.Base
{
    public class Result
    {
        public byte[] SendData { get; set; } = Array.Empty<byte>();
        public byte[] ReceivedData { get; set; } = Array.Empty<byte>();
        public byte[] Payload { get; set; } = Array.Empty<byte>();
        public string SendDataHexString => SendData.ToHexString();
        public string ReceivedDataHexString => ReceivedData.ToHexString();
        public string SendDataAsciiString => SendData.ToAsciiString();
        public string ReceivedDataAsciiString => ReceivedData.ToAsciiString();
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; }
        public TimeSpan Elapsed => EndTime - StartTime;
        public override string ToString()
        {
            return string.Concat($"SendDataHex[{SendData.Length}]:{SendDataHexString};",
             $"ReceivedDataHex[{ReceivedData.Length}]:{ReceivedDataHexString};",
             $"SendDataAscii:{SendDataAsciiString};",
             $"ReceivedDataAscii:{ReceivedDataAsciiString};",
             $"Payload[{Payload.Length}]:{Payload.ToHexString()};",
             $"StartTime:{StartTime:yyyy-MM-dd HH:mm:ss.fff};",
             $"EndTime:{EndTime:yyyy-MM-dd HH:mm:ss.fff};",
             $"Elapsed:{Elapsed.TotalMilliseconds}ms;");
        }
    }
}