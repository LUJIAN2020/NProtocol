using NProtocol.Communication.Extensions;
using System;
using System.Text;

namespace NProtocol.Communication.Exceptions
{
    public class ExecuteException : Exception
    {
        public byte[]? SendData { get; }
        public byte[]? ReceivedData { get; }
        public string? DriverId { get; set; }
        public ExecuteException(string? message = default, byte[]? sendData = default, byte[]? receivedData = default, string? driverId = default)
            : base(GetMessage(message, sendData, receivedData, driverId))
        {
            SendData = sendData;
            ReceivedData = receivedData;
            DriverId = driverId;
        }
        public ExecuteException(string? message = default, byte[]? sendData = default, byte[]? receivedData = default, string? driverId = default, Exception? innerException = default)
            : base(GetMessage(message, sendData, receivedData, driverId), innerException)
        {
            SendData = sendData;
            ReceivedData = receivedData;
            DriverId = driverId;
        }
        private static string GetMessage(string? message = default, byte[]? sendData = default, byte[]? receivedData = default, string? driverId = default)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(message))
            {
                sb.Append(message);
                sb.Append(";");
            }
            if (sendData is not null)
            {
                sb.Append($"S:{sendData.ToHexString()};");
            }
            if (receivedData is not null)
            {
                sb.Append($"R:{receivedData.ToHexString()};");
            }
            if (driverId is not null)
            {
                sb.Append($"DriverId:{driverId};");
            }
            return sb.ToString();
        }
    }
}
