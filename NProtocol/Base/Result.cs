using NProtocol.Extensions;
using System;
using System.Text;

namespace NProtocol.Base
{
    public class Result
    {
        public byte[] SendData { get; internal set; } = Array.Empty<byte>();
        public byte[] ReceivedData { get; internal set; } = Array.Empty<byte>();
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"TX-HEX [{SendData.Length}] : {GetSendDataHexString()}")
                .AppendLine($"RX-HEX [{ReceivedData.Length}] : {GetReceivedDataHexString()}")
                .AppendLine($"TX-ASCII : {GetSendDataAsciiString()}")
                .AppendLine($"RX-ASCII : {GetReceivedDataAsciiString()}");
            return sb.ToString();
        }
        public string GetSendDataAsciiString() => SendData.ToAsciiString();
        public string GetReceivedDataAsciiString() => ReceivedData.ToAsciiString();
        public string GetSendDataHexString() => SendData.ToHexString();
        public string GetReceivedDataHexString() => ReceivedData.ToHexString();
    }
}
