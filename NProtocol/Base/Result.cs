using System;
using System.Text;
using NProtocol.Extensions;

namespace NProtocol.Base
{
    public class Result
    {
        public byte[] SendData { get; internal set; } = Array.Empty<byte>();
        public byte[] ReceivedData { get; internal set; } = Array.Empty<byte>();
        public byte[] Payload { get; internal set; } = Array.Empty<byte>();
        public string SendDataHexString => SendData.ToHexString();
        public string ReceivedDataHexString => ReceivedData.ToHexString();
        public string SendDataAsciiString => SendData.ToAsciiString();
        public string ReceivedDataAsciiString => ReceivedData.ToAsciiString();
        public DateTime StartTime { get; internal set; } = DateTime.Now;
        public DateTime EndTime { get; internal set; }
        public TimeSpan Elapsed => EndTime - StartTime;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(
                    $"------------ {StartTime:yyyy-MM-dd HH:mm:ss.fff} [Elapsed={Elapsed.TotalMilliseconds}ms] ------------"
                )
                .AppendLine($"TX-HEX [{SendData.Length}] : {SendDataHexString}")
                .AppendLine($"RX-HEX [{ReceivedData.Length}] : {ReceivedDataHexString}")
                .AppendLine($"TX-ASCII : {SendDataAsciiString}")
                .AppendLine($"RX-ASCII : {ReceivedDataAsciiString}");
            return sb.ToString();
        }
    }
}
