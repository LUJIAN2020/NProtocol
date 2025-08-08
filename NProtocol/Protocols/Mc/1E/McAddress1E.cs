using System;
using System.Linq;
using NProtocol.Exceptions;
using NProtocol.Extensions;

namespace NProtocol.Protocols.Mc
{
    public readonly struct McAddress1E
    {
        public McAddress1E(string address)
        {
            Address = address;
            ParseMcAddress(
                address,
                out PlcMemory1E memory,
                out uint startAddress,
                out bool isOctAddress
            );
            Memory = memory;
            StartAddress = startAddress;
            IsOctAddress = isOctAddress;
        }

        public McAddress1E(PlcMemory1E memory, uint startAddress)
        {
            Memory = memory;
            StartAddress = startAddress;
            IsOctAddress = HasOctAddress(memory);
            Address = this.ToString();
        }

        public string Address { get; }
        public PlcMemory1E Memory { get; }
        public bool IsOctAddress { get; }
        public uint StartAddress { get; }

        public static void ParseMcAddress(
            string address,
            out PlcMemory1E memory,
            out uint startAddress,
            out bool isOctAddress
        )
        {
            if (address.Length < 2)
                throw new ArgumentOutOfRangeException(
                    nameof(address.Length),
                    address.Length,
                    "The length of the address format is incorrect"
                );

            var addr = address.ToUpper();
            int index = 0;
            switch (addr[0])
            {
                case 'X':
                case 'Y':
                case 'M':
                case 'S':
                case 'D':
                case 'R':
                    index = 1;
                    break;
                case 'C':
                    if (addr[1] == 'S' || addr[1] == 'N')
                        index = 2;
                    break;
                case 'T':
                    if (addr[1] == 'S' || addr[1] == 'N')
                        index = 2;
                    break;
                default:
                    throw new AddressParseException("Address format error", address);
            }

            Parse(addr, index, out memory, out startAddress, out isOctAddress);
        }

        private static void Parse(
            string address,
            int index,
            out PlcMemory1E memory,
            out uint startAddress,
            out bool isOctAddress
        )
        {
            string memoryStr = address.Substring(0, index);
            string valueStr = address.Substring(index);
            memory = (PlcMemory1E)Enum.Parse(typeof(PlcMemory1E), memoryStr);
            isOctAddress = HasOctAddress(memory);
            startAddress = ConvertStartAddress(isOctAddress, valueStr);
        }

        public static bool HasOctAddress(PlcMemory1E memory)
        {
            return memory == PlcMemory1E.X || memory == PlcMemory1E.Y;
        }

        public static uint ConvertStartAddress(bool isOctAddress, string value)
        {
            if (isOctAddress)
            {
                if (value.Length % 2 > 0)
                {
                    value = value.PadLeft(value.Length + 1, '0');
                }
                return value.ToUInt32FromOctString("");
            }
            else
            {
                return Convert.ToUInt32(value);
            }
        }

        public override string ToString()
        {
            int totalWidth = PadLeftTotalWidth(Memory);
            string valueStr = IsOctAddress
                ? BitConverter.GetBytes(StartAddress).Reverse().ToArray().ToOctString("")
                : StartAddress.ToString();

            string val =
                valueStr.Length > totalWidth
                    ? valueStr.Remove(0, valueStr.Length - totalWidth)
                    : valueStr.PadLeft(totalWidth, '0');

            return $"{Memory}{val}";
        }

        private int PadLeftTotalWidth(PlcMemory1E memory) =>
            memory switch
            {
                PlcMemory1E.X or PlcMemory1E.Y => 3,
                PlcMemory1E.M or PlcMemory1E.S => 4,
                PlcMemory1E.TS or PlcMemory1E.CS or PlcMemory1E.TN or PlcMemory1E.CN => 3,
                PlcMemory1E.D => 4,
                PlcMemory1E.R => 5,
                _ => 6,
            };
    }
}
