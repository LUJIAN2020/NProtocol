using System;
using NProtocol.Base;

namespace NProtocol.Exceptions
{
    public class DriverTimeoutException : Exception
    {
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
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
                $"{message}, Start:{result.StartTime:yyyy-MM-dd HH:mm:ss.fff}, End:{endTime:yyyy-MM-dd HH:mm:ss.fff}, SetTimeout:{setTimeout}ms, DriverId:{driverId}, [TX]:{result.SendDataHexString}, [RX]:{result.ReceivedDataHexString}"
            )
        {
            ExecuteResult = result;
            StartTime = result.StartTime;
            EndTime = endTime;
            SetTimeout = setTimeout;
            DriverId = driverId;
        }
    }
}
