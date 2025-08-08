using System;
using System.Text;
using NProtocol.Extensions;

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
            sb.AppendLine(
                    $"------------ {StartTime:yyyy-MM-dd HH:mm:ss.fff} [Elapsed={Elapsed.TotalMilliseconds}ms] ------------"
                )
                .AppendLine($"TX-HEX [{SendData.Length}] : {SendDataHexString}")
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
