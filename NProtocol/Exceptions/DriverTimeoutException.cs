using NProtocol.Base;
using System;

namespace NProtocol.Exceptions
{
    public class DriverTimeoutException : Exception
    {
        public int SetTimeout { get; }
        public Result? ExecuteResult { get; }
        public string? DriverId { get; }
        public DriverTimeoutException(string message) : base(message) { }
        public DriverTimeoutException(string message, DateTime endTime, int setTimeout, string driverId, Result result)
            : base($"{message}, SetTimeout:{setTimeout}ms, DriverId:{driverId}, {result}")
        {
            ExecuteResult = result;
            SetTimeout = setTimeout;
            DriverId = driverId;
        }
    }
}
