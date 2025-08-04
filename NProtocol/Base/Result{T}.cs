using NProtocol.Communication.Extensions;
using System;

namespace NProtocol.Communication.Base
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
            string value = Value?.ToString() ?? string.Empty;
            if (Value is Array array)
            {
                value = array.ToString() ?? string.Empty;
            }
            return string.Concat($"SendData[{SendData.Length}]:{SendDataHexString};",
            $"ReceivedData[{ReceivedData.Length}]:{ReceivedDataHexString};",
            $"SendDataAscii:{SendDataAsciiString};",
            $"ReceivedDataAscii:{ReceivedDataAsciiString};",
            $"Payload[{Payload.Length}]:{Payload.ToHexString()};",
            $"Value:{value};",
            $"StartTime:{StartTime:yyyy-MM-dd HH:mm:ss.fff};",
            $"EndTime:{EndTime:yyyy-MM-dd HH:mm:ss.fff};",
            $"Elapsed:{Elapsed.TotalMilliseconds}ms;");
        }
    }
}