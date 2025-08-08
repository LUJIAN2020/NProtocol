using System;
using System.Text;

namespace NProtocol.Exceptions
{
    public class LoopTimeoutException : TimeoutException
    {
        public string? DriverId { get; set; }

        public LoopTimeoutException(string? message = default, string? driverId = default)
            : base(GetMessage(message, driverId))
        {
            DriverId = driverId;
        }

        private static string GetMessage(string? message = default, string? driverId = default)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(message))
            {
                sb.Append(message);
                sb.Append(";");
            }
            if (driverId is not null)
            {
                sb.Append($"DriverId:{driverId};");
            }
            return sb.ToString();
        }
    }
}
