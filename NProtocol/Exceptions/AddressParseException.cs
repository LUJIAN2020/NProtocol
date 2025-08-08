using System;

namespace NProtocol.Exceptions
{
    public class AddressParseException : Exception
    {
        public string Address { get; }

        public AddressParseException(string address)
            : base($"Address:{address}")
        {
            Address = address;
        }

        public AddressParseException(string message, string address)
            : base(GetMessage(message, address))
        {
            Address = address;
        }

        public AddressParseException(string message, string address, Exception innerException)
            : base(GetMessage(message, address), innerException)
        {
            Address = address;
        }

        private static string GetMessage(string message, string address) =>
            $"{message};Address:{address};";
    }
}
