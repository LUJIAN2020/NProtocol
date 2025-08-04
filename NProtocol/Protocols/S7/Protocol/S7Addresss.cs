using NProtocol.Exceptions;
using NProtocol.Protocols.S7.Enums;
using System;

namespace NProtocol.Protocols.S7
{
    public readonly struct S7Addresss
    {
        public S7Addresss(string address)
        {
            Address = address;
            ParseS7CommAddress(address, out S7MemoryAreaType areaType, out ushort db, out S7VarType varType, out ushort wordAddress, out byte bitAddress);
            AreaType = areaType;
            DbNumber = db;
            VarType = varType;
            WordAddress = wordAddress;
            BitAddress = bitAddress;
        }
        public string Address { get; }
        public ushort DbNumber { get; }
        public ushort WordAddress { get; }
        public byte BitAddress { get; }
        public int ByteLength => VarTypeToByteLength(VarType);
        public S7MemoryAreaType AreaType { get; }
        public S7VarType VarType { get; }
        internal static int VarTypeToByteLength(S7VarType varType, int varCount = 1)
        {
            return varType switch
            {
                S7VarType.Bit => (varCount + 7) / 8,
                S7VarType.Byte => (varCount < 1) ? 1 : varCount,
                S7VarType.String => varCount,
                S7VarType.S7String => ((varCount + 2) & 1) == 1 ? (varCount + 3) : (varCount + 2),
                S7VarType.S7WString => (varCount * 2) + 4,
                S7VarType.Word or S7VarType.Timer or S7VarType.Int or S7VarType.Counter or S7VarType.Date => varCount * 2,
                S7VarType.DWord or S7VarType.DInt or S7VarType.Real or S7VarType.Time => varCount * 4,
                S7VarType.LReal or S7VarType.DateTime => varCount * 8,
                S7VarType.DateTimeLong => varCount * 12,
                _ => 0,
            };
        }
        /// <summary>
        /// S7Comm地址解析<br/>
        /// 地址示例：<br/>
        /// S7-200没有DB块 V=DB1
        /// DB1.DBX29.0  地址为29.0，类型为布尔<br/>
        /// DB1.DBB22  地址为22，类型为字节型<br/>
        /// DB1.DBW28  地址为28，类型为字<br/>
        /// DB1.DBD0  地址为0，类型为双字或实数<br/>
        /// DB1.DBX18  地址为18类型为字符串<br/>
        /// MB0 地址位0，类型为字节<br/>
        /// MW2 地址为2，类型为字<br/>
        /// MD6 地址为6，类型为双字<br/>
        /// </summary>
        /// <param name="inputAddress">字符串地址</param>
        /// <param name="areaType">寄存器区域类型</param>
        /// <param name="db">DB块</param>
        /// <param name="varType">变量类型</param>
        /// <param name="wordAddress">字地址</param>
        /// <param name="bitAddress">位地址</param>
        /// <exception cref="AddressParseException"></exception>
        public static void ParseS7CommAddress(string inputAddress, out S7MemoryAreaType areaType, out ushort db, out S7VarType varType, out ushort wordAddress, out byte bitAddress)
        {
            bitAddress = 0;
            db = 0;
            switch (inputAddress.Substring(0, 2))
            {
                case "DB":
                    string[] strings = inputAddress.Split('.');
                    if (strings.Length < 2)
                        throw new AddressParseException("Address to few periods for DB address", inputAddress);

                    areaType = S7MemoryAreaType.DataBlock;
                    db = Convert.ToUInt16(strings[0].Substring(2));
                    wordAddress = Convert.ToUInt16(strings[1].Substring(3));
                    string dbType = strings[1].Substring(0, 3);
                    switch (dbType)
                    {
                        case "DBB":
                            varType = S7VarType.Byte;
                            return;
                        case "DBW":
                            varType = S7VarType.Word;
                            return;
                        case "DBD":
                            varType = S7VarType.DWord;
                            return;
                        case "DBX":
                            if (strings.Length == 3 && byte.TryParse(strings[2], out bitAddress))
                            {
                                if (bitAddress > 7)
                                    throw new AddressParseException("Bit can only be 0-7", inputAddress);

                                varType = S7VarType.Bit;
                            }
                            else
                            {
                                varType = S7VarType.Byte;
                            }
                            return;
                        default:
                            throw new AddressParseException("The address cannot be parse", inputAddress);
                    }
                case "IB":
                case "EB"://Input byte
                    areaType = S7MemoryAreaType.Input;
                    wordAddress = Convert.ToUInt16(inputAddress.Substring(2));
                    varType = S7VarType.Byte;
                    return;
                case "IW":
                case "EW"://Input word
                    areaType = S7MemoryAreaType.Input;
                    wordAddress = Convert.ToUInt16(inputAddress.Substring(2));
                    varType = S7VarType.Word;
                    return;
                case "ID":
                case "ED"://Input double-word
                    areaType = S7MemoryAreaType.Input;
                    wordAddress = Convert.ToUInt16(inputAddress.Substring(2));
                    varType = S7VarType.DWord;
                    return;
                case "QB":
                case "AB":
                case "OB"://Output byte
                    areaType = S7MemoryAreaType.Output;
                    wordAddress = Convert.ToUInt16(inputAddress.Substring(2));
                    varType = S7VarType.Byte;
                    return;
                case "QW":
                case "AW":
                case "OW"://Output word
                    areaType = S7MemoryAreaType.Output;
                    wordAddress = Convert.ToUInt16(inputAddress.Substring(2));
                    varType = S7VarType.Word;
                    return;
                case "QD":
                case "AD":
                case "OD"://Output double-word
                    areaType = S7MemoryAreaType.Output;
                    wordAddress = Convert.ToUInt16(inputAddress.Substring(2));
                    varType = S7VarType.DWord;
                    return;
                case "MB"://Memory byte
                    areaType = S7MemoryAreaType.Memory;
                    wordAddress = Convert.ToUInt16(inputAddress.Substring(2));
                    varType = S7VarType.Byte;
                    return;
                case "MW"://Memory word
                    areaType = S7MemoryAreaType.Memory;
                    wordAddress = Convert.ToUInt16(inputAddress.Substring(2));
                    varType = S7VarType.Word;
                    return;
                case "MD"://Memory double-word
                    areaType = S7MemoryAreaType.Memory;
                    wordAddress = Convert.ToUInt16(inputAddress.Substring(2));
                    varType = S7VarType.DWord;
                    return;
                default:
                    switch (inputAddress.Substring(0, 1))
                    {
                        case "E":
                        case "I"://Input
                            areaType = S7MemoryAreaType.Input;
                            varType = S7VarType.Bit;
                            break;
                        case "Q":
                        case "A":
                        case "O"://Output
                            areaType = S7MemoryAreaType.Output;
                            varType = S7VarType.Bit;
                            break;
                        case "M"://Memory
                            areaType = S7MemoryAreaType.Memory;
                            varType = S7VarType.Bit;
                            break;
                        case "T"://Timer
                            areaType = S7MemoryAreaType.S7Timer;
                            wordAddress = Convert.ToUInt16(inputAddress.Substring(1));
                            varType = S7VarType.Timer;
                            return;
                        case "Z":
                        case "C"://Counter
                            areaType = S7MemoryAreaType.S7Counter;
                            wordAddress = Convert.ToUInt16(inputAddress.Substring(1));
                            varType = S7VarType.Counter;
                            return;
                        default:
                            throw new AddressParseException("Address is not a valid address", inputAddress);
                    }
                    string txt2 = inputAddress.Substring(1);
                    if (txt2.IndexOf(".") == -1)
                        throw new AddressParseException("Address to few periods for DB address", inputAddress);

                    var splitDot = txt2.Split('.');
                    wordAddress = Convert.ToUInt16(splitDot[0]);
                    bitAddress = Convert.ToByte(splitDot[1]);
                    if (bitAddress > 7)
                        throw new AddressParseException("Bit can only be 0-7", inputAddress);

                    return;
            }
        }
    }
}
