using System;
using System.Linq;
using System.Text;
using NProtocol.Base;
using NProtocol.Extensions;
using NProtocol.Protocols.S7.Enums;
using NProtocol.Protocols.S7.Extensions;
using NProtocol.Protocols.S7.StructType;

namespace NProtocol.Protocols.S7
{
    /// <summary>
    /// Method for reading S7 protocol
    /// </summary>
    public partial class S7Client
    {
        /// <summary>
        /// Get the read variable message packet
        /// </summary>
        /// <param name="areaType">Memory area type</param>
        /// <param name="varType">Variable type</param>
        /// <param name="db">DB block</param>
        /// <param name="wordAddress">Word address</param>
        /// <param name="bitAddress">Bit address</param>
        /// <param name="count">Total count</param>
        /// <returns></returns>
        private byte[] GetReadVarPacket(
            S7MemoryAreaType areaType,
            S7VarType varType,
            ushort db,
            int wordAddress,
            byte bitAddress,
            ushort count
        )
        {
            var tpkt = CreateTpktPacket();
            var cotp = CreateCotpFuctionPacket(CotpPduType.Data);
            var s7Comm = CreateS7CommPacket(
                S7CommPduType.Job,
                S7CommFuncCode.ReadVar,
                areaType,
                varType,
                db,
                wordAddress,
                bitAddress,
                count,
                null
            );
            byte[] len = ((ushort)(tpkt.Length + cotp.Length + s7Comm.Length)).ToBytes();
            tpkt[2] = len[1];
            tpkt[3] = len[0];
            return tpkt.Combine(cotp, s7Comm);
        }

        /// <summary>
        /// Read byte array
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private Result<byte[]> ReadBytes(
            S7MemoryAreaType areaType,
            ushort db,
            int wordAddress,
            ushort count,
            byte bitAddress = 0,
            S7VarType varType = S7VarType.Byte
        )
        {
            return EnqueueExecute(() =>
            {
                var sendData = GetReadVarPacket(
                    areaType,
                    varType,
                    db,
                    wordAddress,
                    bitAddress,
                    count
                );
                var result = NoLockExecute(sendData);
                var receivedData = result.ReceivedData;
                int recLen = receivedData[23] * 256 + receivedData[24];
                recLen = GetReceivePaloadDataLength(receivedData[22], recLen);
                var data = receivedData.Slice(25);
                return result.ToResult(data);
            });
        }

        /// <summary>
        /// Read multiple consecutive bytes
        /// </summary>
        /// <param name="item">S7 address parameter</param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private Result<byte[]> ReadBytes(S7Addresss item, ushort count)
        {
            return ReadBytes(item.AreaType, item.DbNumber, item.WordAddress, count);
        }

        /// <summary>
        /// Read data, returns byte array with a generic result
        /// </summary>
        /// <param name="address">Starting address in string format</param>
        /// <param name="count">Length of data to read</param>
        /// <returns>Execution result in generic form</returns>
        public Result<byte[]> ReadBytes(string address, ushort count)
        {
            var item = new S7Addresss(address);
            return ReadBytes(item, count);
        }

        /// <summary>
        /// Read a single byte
        /// </summary>
        /// <param name="address">Address to read from</param>
        /// <returns>Byte value</returns>
        /// <exception cref="ArgumentNullException">Thrown when address is null</exception>
        public Result<byte> ReadByte(string address)
        {
            var result = ReadBytes(address, 1);
            var first = result.Value.FirstOrDefault();
            return result.ToResult(first);
        }

        /// <summary>
        /// Batch read bits, read by byte
        /// </summary>
        /// <param name="address">String address</param>
        /// <param name="count">Total number of bits to read</param>
        /// <returns>Result of the read operation</returns>
        public Result<bool[]> ReadBooleans(string address, ushort count)
        {
            var item = new S7Addresss(address);
            int fistBitLen = 8 - item.BitAddress;
            int lastBitLen = count - fistBitLen;
            int byteLength = lastBitLen % 8 > 0 ? 1 + lastBitLen / 8 + 1 : 1 + lastBitLen / 8;
            //为了减少读的频次，全部按字节进行读数据
            var result = ReadBytes(item, (ushort)byteLength);
            var bs = result.Value.ToBooleans().Slice(item.BitAddress, count);
            return result.ToResult(bs);
        }

        /// <summary>
        /// Read a single bit
        /// </summary>
        /// <param name="address">String address</param>
        /// <returns>Execution result</returns>
        public Result<bool> ReadBoolean(string address)
        {
            var result = ReadBooleans(address, 1);
            var b = result.Value.FirstOrDefault();
            return result.ToResult(b);
        }

        private Result<bool> ReadBoolean(
            S7MemoryAreaType areaType,
            ushort db,
            int wordAddress,
            byte bitAddress = 0
        )
        {
            return EnqueueExecute(() =>
            {
                if (bitAddress > 7)
                    throw new ArgumentOutOfRangeException(
                        nameof(bitAddress),
                        bitAddress,
                        "Position must be less than 8"
                    );

                var sendData = GetReadVarPacket(
                    areaType,
                    S7VarType.Bit,
                    db,
                    wordAddress,
                    bitAddress,
                    1
                );
                var result = NoLockExecute(sendData);
                var receivedData = result.ReceivedData;
                var payload = receivedData.Slice(25);
                bool b = ((payload.FirstOrDefault() >> bitAddress) & 1) == 1;
                return result.ToResult(b);
            });
        }

        /// <summary>
        /// Read multiple consecutive words
        /// </summary>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public Result<ushort[]> ReadWords(string address, byte count)
        {
            const byte byteLength = 2;
            var result = ReadBytes(address, (ushort)(count * byteLength));
            var values = result.Value.ToUInt16Array();
            return result.ToResult(values);
        }

        /// <summary>
        /// Read a single word
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public Result<ushort> ReadWord(string address)
        {
            var result = ReadWords(address, 1);
            var value = result.Value.FirstOrDefault();
            return result.ToResult(value);
        }

        /// <summary>
        /// Read multiple double words
        /// </summary>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public Result<uint[]> ReadDWords(string address, byte count)
        {
            const byte byteLength = 4;
            var result = ReadBytes(address, (ushort)(count * byteLength));
            var value = result.Value.ToUInt32Array();
            return result.ToResult(value);
        }

        /// <summary>
        /// Read a single double word
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public Result<uint> ReadDWord(string address)
        {
            var result = ReadDWords(address, 1);
            var value = result.Value.FirstOrDefault();
            return result.ToResult(value);
        }

        /// <summary>
        /// Read multiple continuous S7Int data
        /// </summary>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public Result<short[]> ReadInts(string address, byte count)
        {
            const byte byteLength = 2;
            var result = ReadBytes(address, (ushort)(count * byteLength));
            var values = result.Value.ToInt16Array();
            return result.ToResult(values);
        }

        /// <summary>
        /// 读单个S7Int数据
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<short> ReadInt(string address)
        {
            var result = ReadInts(address, 1);
            var value = result.Value.FirstOrDefault();
            return result.ToResult(value);
        }

        /// <summary>
        /// 读多个连续的S7DInt数据
        /// </summary>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<int[]> ReadDInts(string address, byte count)
        {
            const byte byteLength = 4;
            var result = ReadBytes(address, (ushort)(count * byteLength));
            var values = result.Value.ToInt32Array();
            return result.ToResult(values);
        }

        /// <summary>
        /// 读单个S7DInt数据
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<int> ReadDInt(string address)
        {
            var result = ReadDInts(address, 1);
            var value = result.Value.FirstOrDefault();
            return result.ToResult(value);
        }

        /// <summary>
        /// 读连续的多个实数
        /// </summary>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<float[]> ReadReals(string address, byte count)
        {
            const byte byteLength = 4;
            var result = ReadBytes(address, (ushort)(count * byteLength));
            var values = result.Value.ToFloatArray();
            return result.ToResult(values);
        }

        /// <summary>
        /// 读单个实数
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<float> ReadReal(string address)
        {
            var result = ReadReals(address, 1);
            var value = result.Value.FirstOrDefault();
            return result.ToResult(value);
        }

        /// <summary>
        /// 读连续的多个长实数 不支持直接寻址读取
        /// </summary>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<double[]> ReadLReals(
            S7MemoryAreaType areaType,
            ushort db,
            int wordAddress,
            byte count
        )
        {
            const byte byteLength = 8;
            var result = ReadBytes(areaType, db, wordAddress, (ushort)(count * byteLength));
            var values = result.Value.ToDoubleArray();
            return result.ToResult(values);
        }

        /// <summary>
        /// 读单个长实数 不支持直接寻址读取
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<double> ReadLReal(S7MemoryAreaType areaType, ushort db, int wordAddress)
        {
            var result = ReadLReals(areaType, db, wordAddress, 1);
            var value = result.Value.FirstOrDefault();
            return result.ToResult(value);
        }

        /// <summary>
        /// 读字符串 返回S7String
        /// </summary>
        /// <param name="dbNumber">DB块</param>
        /// <param name="address">地址</param>
        /// <returns>类型：S7String</returns>
        /// <exception cref="LpException"></exception>
        public Result<string> ReadS7StringFromDataBlock(
            ushort dbNumber,
            ushort address,
            byte stringLength
        )
        {
            if (stringLength > S7StringExtension.S7StringMaximumLength)
                throw new ArgumentOutOfRangeException(
                    nameof(stringLength),
                    stringLength,
                    $"Read the maximum string length of {S7StringExtension.S7StringMaximumLength}"
                );
            var result = ReadStringFromDataBlock(dbNumber, address, (ushort)(stringLength + 2));
            string str = result.Value.ToS7String();
            return result.ToResult(str);
        }

        /// <summary>
        /// 读字符串 返回S7WString
        /// </summary>
        /// <param name="dbNumber">DB块</param>
        /// <param name="address">地址</param>
        /// <returns>类型：S7WString</returns>
        /// <exception cref="LpException"></exception>
        public Result<string> ReadS7WStringFromDataBlock(
            ushort dbNumber,
            ushort address,
            ushort stringLength
        )
        {
            if (stringLength > S7StringExtension.S7WStringMaximumLength)
                throw new ArgumentOutOfRangeException(
                    nameof(stringLength),
                    stringLength,
                    $"Read the maximum string length of {S7StringExtension.S7WStringMaximumLength}"
                );
            var result = ReadStringFromDataBlock(dbNumber, address, (ushort)(stringLength * 2 + 4));
            string str = result.Value.ToS7WString();
            return result.ToResult(str);
        }

        /// <summary>
        /// 读S7Char
        /// </summary>
        /// <param name="dbNumber"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public Result<char> ReadS7CharFromDataBlock(ushort dbNumber, ushort address)
        {
            var result = ReadStringFromDataBlock(dbNumber, address, 1);
            var payload = result.Value;
            var str = Encoding.ASCII.GetString(payload);
            char c = str.Length > 0 ? str[0] : char.MinValue;
            return result.ToResult(c);
        }

        /// <summary>
        /// 读S7WChar
        /// </summary>
        /// <param name="dbNumber"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        /// <returns></returns>
        public Result<string> ReadS7WCharFromDataBlock(ushort dbNumber, ushort address)
        {
            var result = ReadStringFromDataBlock(dbNumber, address, 2);
            var payload = result.Value;
            var str = Encoding.BigEndianUnicode.GetString(payload);
            return result.ToResult(str);
        }

        /// <summary>
        /// 读字符串
        /// </summary>
        /// <param name="dbNumber"></param>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="LpException"></exception>
        private Result<byte[]> ReadStringFromDataBlock(
            ushort dbNumber,
            ushort address,
            ushort count
        )
        {
            return EnqueueExecute(() =>
            {
                var sendData = GetReadVarPacket(
                    S7MemoryAreaType.DataBlock,
                    S7VarType.Byte,
                    dbNumber,
                    address,
                    0,
                    count
                );
                var result = NoLockExecute(sendData);
                var receivedData = result.ReceivedData;
                var values = receivedData.Slice(25);
                return result.ToResult(values);
            });
        }

        /// <summary>
        /// 读多个结构体
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="dbNumber">DB块</param>
        /// <param name="startAddress">开始地址</param>
        /// <param name="count">结构体总数</param>
        /// <returns>结构体数组</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public T[] ReadStructs<T>(ushort dbNumber, ushort startAddress, int count)
            where T : struct
        {
            var type = typeof(T);
            int len = S7StructType.GetStructSize(type);
            var values = new T[count];
            for (int i = 0; i < count; i++)
            {
                var result = ReadBytes(
                    S7MemoryAreaType.DataBlock,
                    dbNumber,
                    startAddress,
                    (ushort)len
                );
                var payload = result.Value;
                var obj = S7StructType.ToStruct(type, payload);
                if (obj is null)
                    throw new ArgumentNullException(nameof(obj));
                values[i] = (T)obj;
            }
            return values;
        }

        /// <summary>
        /// 读单个结构体
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="dbNumber">DB块</param>
        /// <param name="startAddress">开始地址</param>
        /// <returns></returns>
        public T ReadStruct<T>(ushort dbNumber, ushort startAddress)
            where T : struct
        {
            return ReadStructs<T>(dbNumber, startAddress, 1).FirstOrDefault();
        }

        /// <summary>
        /// 读DB块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbNumber"></param>
        /// <param name="wordAddr"></param>
        /// <param name="count"></param>
        /// <param name="bitAddr"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Result<T[]> ReadDataBlock<T>(
            ushort dbNumber,
            ushort wordAddr,
            ushort count,
            byte bitAddr = 0
        )
            where T : struct
        {
            T t = default;
            switch (t)
            {
                case bool:
                {
                    int byteLength =
                        (bitAddr + count) % 8 > 0
                            ? (bitAddr + count) / 8 + 1
                            : (bitAddr + count) / 8;
                    var result = ReadBytes(
                        S7MemoryAreaType.DataBlock,
                        dbNumber,
                        wordAddr,
                        (ushort)byteLength
                    );
                    var values = result.Value;
                    var bs = values.ToBooleans().Slice(bitAddr, count);
                    if (bs is T[] val)
                        return result.ToResult(val);
                    break;
                }
                case short:
                {
                    const byte byteLength = 2;
                    var result = ReadBytes(
                        S7MemoryAreaType.DataBlock,
                        dbNumber,
                        wordAddr,
                        (ushort)(count * byteLength)
                    );
                    var buffer = result.Value.ToInt16Array();
                    if (buffer is T[] val)
                        return result.ToResult(val);
                    break;
                }
                case ushort:
                {
                    const byte byteLength = 2;
                    var result = ReadBytes(
                        S7MemoryAreaType.DataBlock,
                        dbNumber,
                        wordAddr,
                        (ushort)(count * byteLength)
                    );
                    var buffer = result.Value.ToUInt16Array();
                    if (buffer is T[] val)
                        return result.ToResult(val);
                    break;
                }
                case int:
                {
                    const byte byteLength = 4;
                    var result = ReadBytes(
                        S7MemoryAreaType.DataBlock,
                        dbNumber,
                        wordAddr,
                        (ushort)(count * byteLength)
                    );
                    var buffer = result.Value.ToInt32Array();
                    if (buffer is T[] val)
                        return result.ToResult(val);
                    break;
                }
                case uint:
                {
                    const byte byteLength = 4;
                    var result = ReadBytes(
                        S7MemoryAreaType.DataBlock,
                        dbNumber,
                        wordAddr,
                        (ushort)(count * byteLength)
                    );
                    var buffer = result.Value.ToUInt32Array();
                    if (buffer is T[] val)
                        return result.ToResult(val);
                    break;
                }
                case long:
                {
                    const byte byteLength = 8;
                    var result = ReadBytes(
                        S7MemoryAreaType.DataBlock,
                        dbNumber,
                        wordAddr,
                        (ushort)(count * byteLength)
                    );
                    var buffer = result.Value.ToInt64Array();
                    if (buffer is T[] val)
                        return result.ToResult(val);
                    break;
                }
                case ulong:
                {
                    const byte byteLength = 8;
                    var result = ReadBytes(
                        S7MemoryAreaType.DataBlock,
                        dbNumber,
                        wordAddr,
                        (ushort)(count * byteLength)
                    );
                    var buffer = result.Value.ToUInt64Array();
                    if (buffer is T[] val)
                        return result.ToResult(val);
                    break;
                }
                case float:
                {
                    const byte byteLength = 4;
                    var result = ReadBytes(
                        S7MemoryAreaType.DataBlock,
                        dbNumber,
                        wordAddr,
                        (ushort)(count * byteLength)
                    );
                    var buffer = result.Value.ToFloatArray();
                    if (buffer is T[] val)
                        return result.ToResult(val);
                    break;
                }
                case double:
                {
                    const byte byteLength = 8;
                    var result = ReadBytes(
                        S7MemoryAreaType.DataBlock,
                        dbNumber,
                        wordAddr,
                        (ushort)(count * byteLength)
                    );
                    var buffer = result.Value.ToDoubleArray();
                    if (buffer is T[] val)
                        return result.ToResult(val);
                    break;
                }
                default:
                    var type = t.GetType();
                    if (!type.IsPrimitive && !type.IsEnum && type.IsValueType)
                    {
                        var result = ReadStructs<T>(dbNumber, wordAddr, count);
                    }

                    break;
            }

            throw new ArgumentException($"Type is not supported,{t.GetType().Name}");
        }

        /// <summary>
        /// 读DB块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbNumber"></param>
        /// <param name="wordAddr"></param>
        /// <param name="bitAddr"></param>
        /// <returns></returns>
        public Result<T> ReadDataBlock<T>(ushort dbNumber, ushort wordAddr, byte bitAddr = 0)
            where T : struct
        {
            var result = ReadDataBlock<T>(dbNumber, wordAddr, 1, bitAddr);
            var value = result.Value.FirstOrDefault();
            return result.ToResult(value);
        }

        /// <summary>
        /// 读多个时间 单位：ms
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <param name="count"></param>
        /// <returns>
        /// 范围：T#-24d_20h_31m_23s_648ms 到 T#24d_20h_31m_23s_647ms
        /// 存储形式： -2,147,483,648 ms 到 +2,147,483,647 ms
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<TimeSpan[]> ReadTimes(
            S7MemoryAreaType areaType,
            ushort db,
            int wordAddress,
            byte count
        )
        {
            const byte byteLength = 4;
            var result = ReadBytes(areaType, db, wordAddress, (ushort)(count * byteLength));
            var value = result.Value.ToTimes();
            return result.ToResult(value);
        }

        /// <summary>
        /// 读单个时间 单位：ms
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <returns>
        /// 范围：T#-24d_20h_31m_23s_648ms 到 T#24d_20h_31m_23s_647ms
        /// 存储形式： -2,147,483,648 ms 到 +2,147,483,647 ms
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<TimeSpan> ReadTime(S7MemoryAreaType areaType, ushort db, int wordAddress)
        {
            var result = ReadTimes(areaType, db, wordAddress, 1);
            var value = result.Value.FirstOrDefault();
            return result.ToResult(value);
        }

        /// <summary>
        /// 读连续的LTime 单位：纳秒
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<TimeSpan[]> ReadLTimes(
            S7MemoryAreaType areaType,
            ushort db,
            int wordAddress,
            byte count
        )
        {
            const byte byteLength = 8;
            var result = ReadBytes(areaType, db, wordAddress, (ushort)(count * byteLength));
            var value = result.Value.ToLTimes();
            return result.ToResult(value);
        }

        /// <summary>
        /// 读LTime 单位：纳秒
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<TimeSpan> ReadLTime(S7MemoryAreaType areaType, ushort db, int wordAddress)
        {
            var result = ReadLTimes(areaType, db, wordAddress, 1);
            var value = result.Value.FirstOrDefault();
            return result.ToResult(value);
        }

        /// <summary>
        /// 读连续的DateAndTime
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<DateTime[]> ReadDateAndTimes(
            S7MemoryAreaType areaType,
            ushort db,
            int wordAddress,
            byte count
        )
        {
            const byte byteLength = 8;
            var result = ReadBytes(areaType, db, wordAddress, (ushort)(count * byteLength));
            var payload = result.Value;
            var dts = new DateTime[payload.Length / byteLength];
            for (int i = 0; i < payload.Length; i += 8)
            {
                dts[i / byteLength] = payload.Slice(i, byteLength).ToDateTime();
            }
            return result.ToResult(dts);
        }

        /// <summary>
        /// 读单个DateAndTime
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<DateTime> ReadDateAndTime(
            S7MemoryAreaType areaType,
            ushort db,
            int wordAddress
        )
        {
            var resul = ReadDateAndTimes(areaType, db, wordAddress, 1);
            var value = resul.Value.FirstOrDefault();
            return resul.ToResult(value);
        }

        /// <summary>
        /// 读连续的TimeOfDay TOD
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <param name="count"></param>
        /// <returns>范围：TOD#0:0:0.0 到 TOD#23:59:59.999</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<TimeSpan[]> ReadTimeOfDays(
            S7MemoryAreaType areaType,
            ushort db,
            int wordAddress,
            byte count
        )
        {
            const byte byteLength = 4;
            var result = ReadBytes(areaType, db, wordAddress, (ushort)(count * byteLength));
            var payload = result.Value;
            var ts = new TimeSpan[count];
            for (int i = 0; i < payload.Length; i += byteLength)
            {
                ts[i / byteLength] = payload.Slice(i, byteLength).ToTimeOfDay();
            }
            return result.ToResult(ts);
        }

        /// <summary>
        /// 读连续的LTimeOfDay LTOD
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<TimeSpan[]> ReadLTimeOfDays(
            S7MemoryAreaType areaType,
            ushort db,
            int wordAddress,
            byte count
        )
        {
            const byte byteLength = 8;
            var result = ReadBytes(areaType, db, wordAddress, (ushort)(count * byteLength));
            var payload = result.Value;
            var ts = new TimeSpan[count];
            for (int i = 0; i < payload.Length; i += byteLength)
            {
                ts[i / byteLength] = payload.Slice(i, byteLength).ToLTimeOfDay();
            }
            return result.ToResult(ts);
        }

        /// <summary>
        /// 读单个LTimeOfDay
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<TimeSpan> ReadLTimeOfDay(
            S7MemoryAreaType areaType,
            ushort db,
            int wordAddress
        )
        {
            var result = ReadLTimeOfDays(areaType, db, wordAddress, 1);
            var value = result.Value.FirstOrDefault();
            return result.ToResult(value);
        }

        /// <summary>
        /// 读连续的DateTimeLong
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <param name="count"></param>
        /// <returns>范围：DTL#1970-01-01-00:00:00.0 到 DTL#2262-04-11-23:47:16.854775807</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<DateTime[]> ReadDtls(
            S7MemoryAreaType areaType,
            ushort db,
            int wordAddress,
            byte count
        )
        {
            const byte byteLength = 12;
            var result = ReadBytes(areaType, db, wordAddress, (ushort)(count * byteLength));
            var payload = result.Value;
            var dts = new DateTime[payload.Length / byteLength];
            for (int i = 0; i < payload.Length; i += byteLength)
            {
                dts[i / byteLength] = payload.Slice(i, byteLength).ToDtl();
            }
            return result.ToResult(dts);
        }

        /// <summary>
        /// 读单个DateTimeLong
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<DateTime> ReadDtl(S7MemoryAreaType areaType, ushort db, int wordAddress)
        {
            var result = ReadDtls(areaType, db, wordAddress, 1);
            var value = result.Value.FirstOrDefault();
            return result.ToResult(value);
        }

        /// <summary>
        /// Read continuous Date data
        /// </summary>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public Result<DateTime[]> ReadDates(
            S7MemoryAreaType areaType,
            ushort db,
            int wordAddress,
            byte count
        )
        {
            const byte byteLength = 2;
            var result = ReadBytes(areaType, db, wordAddress, (ushort)(count * byteLength));
            var payload = result.Value;
            var dts = new DateTime[payload.Length / byteLength];
            for (int i = 0; i < payload.Length; i += byteLength)
            {
                dts[i / byteLength] = payload.Slice(i, byteLength).ToDate();
            }
            return result.ToResult(dts);
        }

        /// <summary>
        /// Read date
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <returns></returns>
        public Result<DateTime> ReadDate(S7MemoryAreaType areaType, ushort db, int wordAddress)
        {
            var result = ReadDates(areaType, db, wordAddress, 1);
            var value = result.Value.FirstOrDefault();
            return result.ToResult(value);
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<object> Read(string address, byte count = 1)
        {
            if (count == 0)
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    count,
                    "Read length must be > 0"
                );

            var item = new S7Addresss(address);
            if (item.VarType == S7VarType.Bit)
            {
                if (count == 1)
                {
                    var result = ReadBoolean(item.Address);
                    object val = result.Value;
                    return result.ToResult(val);
                }
                else
                {
                    var result = ReadBooleans(item.Address, count);
                    object val = result.Value;
                    return result.ToResult(val);
                }
            }
            else
            {
                var result = ReadBytes(item, (ushort)(item.ByteLength * count));
                var payload = result.Value;
                object value;
                switch (item.VarType)
                {
                    case S7VarType.Byte:
                    {
                        value = count == 1 ? payload[0] : payload;
                        break;
                    }
                    case S7VarType.Word:
                    {
                        value = count == 1 ? payload.ToUInt16() : payload.ToUInt16Array();
                        break;
                    }
                    case S7VarType.DWord:
                    {
                        value = count == 1 ? payload.ToUInt32() : payload.ToUInt32Array();
                        break;
                    }
                    case S7VarType.Int:
                    {
                        value = count == 1 ? payload.ToInt16() : payload.ToInt16Array();
                        break;
                    }
                    case S7VarType.DInt:
                    {
                        value = count == 1 ? payload.ToInt32() : payload.ToInt32Array();
                        break;
                    }
                    case S7VarType.Real:
                    {
                        value = count == 1 ? payload.ToFloat() : payload.ToFloatArray();
                        break;
                    }
                    case S7VarType.LReal:
                        throw new InvalidOperationException(
                            $"Please call {nameof(ReadLReals)} or {nameof(ReadLReal)}."
                        );
                    case S7VarType.String:
                        throw new InvalidOperationException(
                            $"Please call {nameof(ReadS7CharFromDataBlock)} or {nameof(ReadS7WCharFromDataBlock)}."
                        );
                    case S7VarType.S7String:
                        throw new InvalidOperationException(
                            $"Please call {nameof(ReadS7StringFromDataBlock)}."
                        );
                    case S7VarType.S7WString:
                        throw new InvalidOperationException(
                            $"Please call {nameof(ReadS7WStringFromDataBlock)}."
                        );
                    case S7VarType.Timer:
                    {
                        if (count == 1)
                        {
                            value = payload.ToTimer();
                        }
                        else
                        {
                            var buff = new double[payload.Length / 4];
                            for (int i = 0; i < payload.Length; i += 4)
                            {
                                buff[i / 4] = payload.Slice(i, 4).ToTimer();
                            }
                            value = buff;
                        }
                        break;
                    }
                    case S7VarType.Counter:
                    {
                        value = count == 1 ? payload.ToUInt16() : payload.ToUInt16Array();
                        break;
                    }
                    case S7VarType.DateTime:
                    {
                        if (count == 1)
                        {
                            value = payload.ToDateTime();
                        }
                        else
                        {
                            var buff = new DateTime[payload.Length / 8];
                            for (int i = 0; i < payload.Length; i += 8)
                            {
                                buff[i / 8] = payload.Slice(i, 8).ToDateTime();
                            }
                            value = buff;
                        }
                        break;
                    }
                    case S7VarType.Date:
                    {
                        if (count == 1)
                        {
                            value = payload.ToDate();
                        }
                        else
                        {
                            var buff = new DateTime[payload.Length / 2];
                            for (int i = 0; i < payload.Length; i += 2)
                            {
                                buff[i / 2] = payload.Slice(i, 2).ToDate();
                            }
                            value = buff;
                        }
                        break;
                    }
                    case S7VarType.DateTimeLong:
                    {
                        if (count == 1)
                        {
                            value = payload.ToDtl();
                        }
                        else
                        {
                            var buff = new DateTime[payload.Length / 12];
                            for (int i = 0; i < payload.Length; i += 12)
                            {
                                buff[i / 12] = payload.Slice(i, 12).ToDtl();
                            }
                            value = buff;
                        }
                        break;
                    }
                    case S7VarType.Time:
                    {
                        value =
                            count == 1
                                ? TimeSpan.FromMilliseconds(payload.ToInt32())
                                : payload
                                    .ToInt32Array()
                                    .Select(c => TimeSpan.FromMilliseconds(c))
                                    .ToArray();
                        break;
                    }
                    default:
                        throw new Exception($"Not supped type `{item.VarType}`");
                }
                return result.ToResult(value);
            }
        }
    }
}
