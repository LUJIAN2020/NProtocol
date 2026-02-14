using System;
using NProtocol.Base;

namespace NProtocol.Exceptions
{
    public class DriverTimeoutException : Exception
    {
        public int SetTimeout { get; }
        public Result? ExecuteResult { get; }
        public string? DriverId { get; }

        public DriverTimeoutException(string message)
            : base(message) { }

        public DriverTimeoutException(
            string message,
            DateTime endTime,
            int setTimeout,
            string driverId,
            Result result
        )
            : base(
                $"{message}, SetTimeout:{setTimeout}ms, DriverId:{driverId}, [TX]:{result.SendDataHexString}, [RX]:{result.ReceivedDataHexString}"
            )
        {
            ExecuteResult = result;
            SetTimeout = setTimeout;
            DriverId = driverId;
        }
    }
}
