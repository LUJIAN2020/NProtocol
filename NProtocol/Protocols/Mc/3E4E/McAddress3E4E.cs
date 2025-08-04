using NProtocol.Communication.Extensions;
using NProtocol.Exceptions;
using NProtocol.Protocols.Mc;
using System;
using System.Linq;

namespace NProtocol.Protocols.Mc
{
    public readonly struct McAddress3E4E
    {
        public McAddress3E4E(string address)
        {
            Address = address;
            ParseMcAddress(address, out PlcMemory3E4E memory, out uint startAddress, out bool isHexAddress);
            Memory = memory;
            StartAddress = startAddress;
            IsHexAddress = isHexAddress;
        }
        public McAddress3E4E(PlcMemory3E4E memory, uint startAddress)
        {
            Memory = memory;
            StartAddress = startAddress;
            IsHexAddress = HasHexAddress(memory);
            Address = this.ToString();
        }
        public string Address { get; }
        public PlcMemory3E4E Memory { get; }
        public bool IsHexAddress { get; }
        public uint StartAddress { get; }
        public static void ParseMcAddress(string address, out PlcMemory3E4E memory, out uint startAddress, out bool isHexAddress)
        {
            if (address.Length < 2)
                throw new ArgumentOutOfRangeException(nameof(address.Length), address.Length, "The length of the address format is incorrect");

            var addr = address.ToUpper();
            int index;
            switch (addr[0])
            {
                case 'X':
                case 'Y':
                case 'B':
                case 'M':
                case 'L':
                case 'F':
                case 'V':
                case 'W':
                case 'R':
                    {
                        index = 1;
                        break;
                    }
                case 'S':
                    {
                        index = addr[1] switch
                        {
                            //SB SM SC SN SD SW
                            'B' or 'M' or 'C' or 'N' or 'D' or 'W' => 2,
                            //STS
                            'T' => 3,
                            //S
                            _ => 1,
                        };
                        break;
                    }
                case 'D':
                    {
                        index = addr[1] switch
                        {

                            'X' or 'Y' => 2,//DX DY
                            _ => 1,//D
                        };
                        break;
                    }
                case 'Z':
                    {
                        index = addr[1] switch
                        {
                            'R' => 2,//ZR
                            _ => 1,//R
                        };
                        break;
                    }
                case 'T':
                    {
                        index = addr[1] switch
                        {
                            //TS TC TN
                            'S' or 'C' or 'N' => 2,
                            _ => throw new AddressParseException("Address format error", address),
                        };
                        break;
                    }
                case 'C':
                    {
                        index = addr[1] switch
                        {
                            //CS CC CN
                            'S' or 'C' or 'N' => 2,
                            _ => throw new AddressParseException("Address format error", address),
                        };
                        break;
                    }
                default:
                    throw new AddressParseException("Address format error", address);
            }

            Parse(address, index, out memory, out startAddress, out isHexAddress);
        }
        private static void Parse(string address, int index, out PlcMemory3E4E memory, out uint startAddress, out bool isHexAddress)
        {
            string memoryStr = address.Substring(0, index);
            string valueStr = address.Substring(index);
            memory = (PlcMemory3E4E)Enum.Parse(typeof(PlcMemory3E4E), memoryStr);
            isHexAddress = HasHexAddress(memory);
            startAddress = ConvertStartAddress(isHexAddress, valueStr);
        }
        public static bool HasHexAddress(PlcMemory3E4E memory)
        {
            return memory == PlcMemory3E4E.X
                || memory == PlcMemory3E4E.DX
                || memory == PlcMemory3E4E.Y
                || memory == PlcMemory3E4E.DY
                || memory == PlcMemory3E4E.B
                || memory == PlcMemory3E4E.SB
                || memory == PlcMemory3E4E.W
                || memory == PlcMemory3E4E.SW
                || memory == PlcMemory3E4E.ZR;
        }
        public static uint ConvertStartAddress(bool isHexAddress, string value)
        {
            if (isHexAddress)
            {
                if (value.Length % 2 > 0)
                {
                    value = value.PadLeft(value.Length + 1, '0');
                }
                return value.ToUInt32FromHexString("", true);
            }
            else
            {
                return Convert.ToUInt32(value);
            }
        }
        public override string ToString()
        {
            int totalWidth = PadLeftTotalWidth(Memory);
            string valueStr = IsHexAddress
                ? BitConverter.GetBytes(StartAddress).Reverse().ToArray().ToHexString("")
                : StartAddress.ToString();

            string val = valueStr.Length > totalWidth
                ? valueStr.Remove(0, valueStr.Length - totalWidth)
                : valueStr.PadLeft(totalWidth, '0');

            return $"{Memory}{val}";
        }
        private int PadLeftTotalWidth(PlcMemory3E4E memory)
        {
            switch (memory)
            {
                case PlcMemory3E4E.X:
                case PlcMemory3E4E.DX:
                case PlcMemory3E4E.Y:
                case PlcMemory3E4E.DY:
                    return 4;
                case PlcMemory3E4E.B:
                case PlcMemory3E4E.SB:
                case PlcMemory3E4E.TS:
                case PlcMemory3E4E.TC:
                case PlcMemory3E4E.STS:
                case PlcMemory3E4E.SC:
                case PlcMemory3E4E.CS:
                case PlcMemory3E4E.CC:
                case PlcMemory3E4E.TN:
                case PlcMemory3E4E.SN:
                case PlcMemory3E4E.CN:
                case PlcMemory3E4E.D:
                    return 8;
                case PlcMemory3E4E.M:
                    return 10;
                case PlcMemory3E4E.SM:
                case PlcMemory3E4E.L:
                case PlcMemory3E4E.F:
                case PlcMemory3E4E.V:
                case PlcMemory3E4E.S:
                case PlcMemory3E4E.SD:
                case PlcMemory3E4E.R:
                    return 5;
                case PlcMemory3E4E.W:
                case PlcMemory3E4E.SW:
                case PlcMemory3E4E.ZR:
                    return 6;
                case PlcMemory3E4E.Z:
                    return 3;
                default:
                    return 10;
            }
        }
    }
}