using NProtocol.Exceptions;
using NProtocol.Extensions;
using System;
using System.Linq;

namespace NProtocol.Protocols.Fins
{
    public readonly struct FinsAddress
    {
        public FinsAddress(PlcMemory memory, DataType dataType, ushort addressWord, byte addressBit = 0, byte? bank = default, ushort stringLength = 0, StringFormatType? stringFormat = default)
        {
            PlcMemory = memory;
            DataType = dataType;
            AddressWord = addressWord;
            AddressBit = addressBit;
            Bank = bank;
            StringLength = stringLength;
            StringFormat = stringFormat;
            Address = ToFinsAddress(memory, dataType, addressWord, addressBit, bank, stringLength, stringFormat);
        }
        public FinsAddress(string address)
        {
            Address = address;
            ParseFinsAddress(address, out PlcMemory memory, out DataType dataType,
                  out ushort addressWord, out byte addressBit,
                  out byte? bank,
                  out ushort stringLength, out StringFormatType? stringFormatType);
            PlcMemory = memory;
            DataType = dataType;
            AddressWord = addressWord;
            AddressBit = addressBit;
            Bank = bank;
            StringLength = stringLength;
            StringFormat = stringFormatType;
        }
        public string Address { get; }
        public PlcMemory PlcMemory { get; }
        public DataType DataType { get; }
        public ushort AddressWord { get; }
        public byte AddressBit { get; }
        public byte? Bank { get; }
        public bool IsBank => Bank is not null;
        public ushort StringLength { get; }
        public StringFormatType? StringFormat { get; }
        public static void ParseFinsAddress(string address, out PlcMemory memory, out DataType dataType,
            out ushort addressWord, out byte addressBit,
            out byte? bank,
            out ushort stringLength, out StringFormatType? stringFormatType)
        {
            address.ThrowIsNullOrWhiteSpace(nameof(address));

            if (!Enum.GetNames(typeof(PlcMemory)).Any(c => address.Contains(c)))
                throw new AddressParseException("Address format error", address);

            if (address.Length < 2)
                throw new AddressParseException("Address format error", address);

            memory = default;
            dataType = default;
            addressWord = default;
            addressBit = default;
            stringLength = default;
            bank = default;
            stringFormatType = default;

            address = address.Trim().ToUpper();

            //E08:00104.06
            //E08:00104.06H
            //D00104.06H
            string leftAddr = string.Empty;
            if (address.Contains('.'))
            {
                var splitDot = address.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (splitDot.Length != 2)
                    throw new AddressParseException("Address format error", address);

                leftAddr = splitDot[0];
                string rightAddr = splitDot[1];

                char lastChar = rightAddr.LastOrDefault();
                if (lastChar == 'H' || lastChar == 'L' || lastChar == 'D' || lastChar == 'E')
                {
                    //字符串
                    dataType = DataType.String;

                    string lenStr = rightAddr.Remove(rightAddr.Length - 1, 1);
                    int bytesLength = Convert.ToUInt16(lenStr);
                    if (bytesLength % 2 > 0)
                        throw new ArgumentOutOfRangeException(nameof(bytesLength), bytesLength,
                            $"E Read the address. The value ranges from 1 to 256 characters. The address format must range from 2H to 512H,{address}");

                    if (lastChar == 'H' || lastChar == 'L')
                    {
                        stringLength = (ushort)bytesLength;
                    }
                    else
                    {
                        stringLength = (ushort)(bytesLength / 2);
                    }
                    stringFormatType = ConvertToStringFormatType(lastChar);
                }
                else
                {
                    //位地址
                    dataType = DataType.Bit;
                    addressBit = Convert.ToByte(splitDot[1]);
                    if (addressBit > 15)
                        throw new ArgumentOutOfRangeException(nameof(addressBit), addressBit,
                            $"The bit address format is incorrect, ranging from 0 to 15,{address}");
                }
            }
            else
            {
                dataType = DataType.Word;
                leftAddr = address;
            }

            var splitColon = leftAddr.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (address.Contains(':') && splitColon.Length != 2)
                throw new AddressParseException("Address format error", address);

            if (splitColon.Length == 1)
            {
                //非EB地址
                switch (leftAddr)
                {
                    case string adr when adr.Contains("CIO"):
                        memory = (PlcMemory)Enum.Parse(typeof(PlcMemory), leftAddr.Substring(0, 3));
                        addressWord = Convert.ToUInt16(leftAddr.Substring(3));
                        break;
                    case string adr when adr.Contains("W") || 
                                         adr.Contains("H") ||
                                         adr.Contains("A") ||
                                         adr.Contains("D") ||
                                         adr.Contains("T") ||
                                         adr.Contains("C") ||
                                         adr.Contains("E"):
                        memory = (PlcMemory)Enum.Parse(typeof(PlcMemory), leftAddr.Substring(0, 1));
                        addressWord = Convert.ToUInt16(leftAddr.Substring(1));
                        break;
                    default:
                        throw new AddressParseException("Notsupported memory region type", address);
                }
            }
            else if (splitColon.Length == 2)
            {
                //EB地址
                memory = PlcMemory.EB;
                var leftColon = splitColon[0];
                var rightColon = splitColon[1];

                bank = Convert.ToByte(leftColon.Substring(1));
                if (bank > 12)
                    throw new ArgumentOutOfRangeException(nameof(bank), bank, $"Bank range: 0-12 {address}");
                addressWord = Convert.ToUInt16(rightColon);
            }
            else
            {
                throw new AddressParseException("Address format error", address);
            }
        }
        private static StringFormatType ConvertToStringFormatType(char lastChar) =>
            (StringFormatType)Enum.Parse(typeof(StringFormatType), lastChar.ToString());
        public override string ToString() => Address;
        private static string ToFinsAddress(PlcMemory memory, DataType dataType, ushort addressWord, byte addressBit, byte? bank, ushort stringLength, StringFormatType? stringFormat)
        {
            string address = string.Empty;
            if (addressBit > 15)
                throw new ArgumentOutOfRangeException(nameof(addressBit), addressBit, "The bit address must 0 - 15");

            switch (dataType)
            {
                case DataType.Bit:
                    {
                        if (memory == PlcMemory.T || memory == PlcMemory.C)
                            throw new ArgumentException("Timer(T), Counter(C) do not support bit operation", nameof(memory));

                        if (memory == PlcMemory.EB)
                        {
                            if (bank is null && bank > 12)
                                throw new ArgumentOutOfRangeException(nameof(bank), bank, "Bank ranges of 0-12");

                            address = $"E{bank:00}:{addressWord:00000}.{addressBit:00}";
                        }
                        else
                        {
                            address = $"{memory}{addressWord:00000}.{addressBit:00}";
                        }
                        break;
                    }
                case DataType.Word:
                    {
                        if (memory == PlcMemory.EB)
                        {
                            if (bank is null && bank > 12)
                                throw new ArgumentOutOfRangeException(nameof(bank), bank, "Bank ranges of 0-12");
                            address = $"E{bank:00}:{addressWord:00000}";
                        }
                        else
                        {
                            address = $"{memory}{addressWord:00000}";
                        }
                        break;
                    }
                case DataType.String:
                    {
                        if (stringFormat is null)
                            throw new ArgumentNullException(nameof(stringFormat), "StringFormat cannot be empty");
                        if (stringFormat == StringFormatType.H || stringFormat == StringFormatType.L)
                        {
                            if (stringLength % 2 > 0 && stringLength < 2 && stringLength > 512)
                                throw new ArgumentOutOfRangeException(nameof(stringLength), stringLength,
                                    "The H and L string are 2 to 512 in length and must be even");
                        }
                        else if (stringFormat == StringFormatType.D || stringFormat == StringFormatType.E)
                        {
                            if (stringLength < 1 && stringLength > 256)
                                throw new ArgumentOutOfRangeException(nameof(stringLength), stringLength,
                                    "The H and L string are 1 to 256 in length");
                        }
                        if (memory == PlcMemory.EB)
                        {
                            if (bank is null && bank > 12)
                                throw new ArgumentOutOfRangeException(nameof(bank), bank, "Bank ranges of 0-12");
                            address = $"E{bank:00}:{addressWord:00000}.{stringLength:000}{stringFormat}";
                        }
                        else
                        {
                            address = $"{memory}{addressWord:00000}.{stringLength:000}{stringFormat}";
                        }
                        break;
                    }
            }
            return address;
        }
    }
}
