using System;

namespace NProtocol.Exceptions
{
    public class ReceivedException : ExecuteException
    {
        public ReceivedException(string? message = default, byte[]? sendData = default, byte[]? receivedData = default, string? driverId = default)
            : base(message, sendData, receivedData, driverId) { }
        public ReceivedException(string? message = default, byte[]? sendData = default, byte[]? receivedData = default, string? driverId = default, Exception? innerException = default)
            : base(message, sendData, receivedData, driverId, innerException) { }
    }
}
