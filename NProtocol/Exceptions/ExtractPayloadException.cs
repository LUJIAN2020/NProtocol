using NProtocol.Extensions;
using System;

namespace NProtocol.Exceptions
{
    public class ExtractPayloadException : Exception
    {
        public byte[] SendData { get; } = Array.Empty<byte>();
        public byte[] ReceivedData { get; } = Array.Empty<byte>();
        public string DeviceId { get; } = string.Empty;
        public ExtractPayloadException(string message) : base(message) { }
        public ExtractPayloadException(string message, byte[] sendData, byte[] receivedData, string driverId)
            : base($"{message},DriverId:{driverId},[TX]:{sendData.ToHexString()},[RX]:{receivedData.ToHexString()}")
        {
            SendData = sendData;
            ReceivedData = receivedData;
            DeviceId = driverId;
        }
    }
}
