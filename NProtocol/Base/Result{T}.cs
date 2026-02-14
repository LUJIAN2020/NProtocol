using NProtocol.Extensions;
using System;
using System.Text;

namespace NProtocol.Base
{
    public class Result<T> : Result
    {
        public Result(T value)
        {
            Value = value;
        }

        public T Value { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"TX-HEX [{SendData.Length}] : {SendDataHexString}")
                .AppendLine($"RX-HEX [{ReceivedData.Length}] : {ReceivedDataHexString}")
                .AppendLine($"TX-ASCII : {SendDataAsciiString}")
                .AppendLine($"RX-ASCII : {ReceivedDataAsciiString}");
            if (Value is Array array)
            {
                sb.AppendLine($"VALUE : {array.ToFlattenString()}");
            }
            else
            {
                sb.AppendLine($"VALUE : {Value}");
            }
            return sb.ToString();
        }
    }
}
