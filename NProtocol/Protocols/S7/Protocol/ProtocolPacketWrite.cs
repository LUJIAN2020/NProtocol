using NProtocol.Communication.Base;
using NProtocol.Communication.Extensions;
using NProtocol.Protocols.S7.Enums;
using NProtocol.Protocols.S7.Extensions;
using NProtocol.Protocols.S7.StructType;
using Robot.Communication.Extensions;
using System;
using System.Linq;
using System.Text;

namespace NProtocol.Protocols.S7
{
    /// <summary>
    /// S7协议写的方法
    /// </summary>
    public partial class S7Client
    {
        /// <summary>
        /// 获取写入变量报文包
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="varType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <param name="count"></param>
        /// <param name="writeData"></param>
        /// <returns></returns>
        /// <exception cref="LpException"></exception>
        private byte[] GetWriteVarPacket(S7MemoryAreaType areaType, S7VarType varType, ushort db, int wordAddress, ushort count, byte[] writeData)
        {
            var tpkt = CreateTpktPacket();
            var cotp = CreateCotpFuctionPacket(CotpPduType.Data);
            var s7Comm = CreateS7CommPacket(S7CommPduType.Job, S7CommFuncCode.WriteVar, areaType, varType, db, wordAddress, 0, count, writeData);
            byte[] len = ((ushort)(tpkt.Length + cotp.Length + s7Comm.Length)).ToBytes();
            tpkt[2] = len[1];
            tpkt[3] = len[0];
            return tpkt.Combine(cotp, s7Comm);
        }
        /// <summary>
        /// 写入字节数组
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="varType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <param name="writeData"></param>
        /// <returns></returns>
        private Result WriteBytes(S7MemoryAreaType areaType, S7VarType varType, ushort db, ushort wordAddress, byte[] writeData)
        {
            return EnqueueExecute(() =>
            {
                var packet = GetWriteVarPacket(areaType, varType, db, wordAddress, (ushort)writeData.Length, writeData);
                return NoLockExecute(packet);
            });
        }
        /// <summary>
        /// 写入连续位
        /// </summary>
        /// <param name="address"></param>
        /// <param name="states"></param>
        /// <returns></returns>
        public Result WriteBoolean(string address, bool state)
        {
            return EnqueueExecute(() =>
            {
                var item = new S7Addresss(address);
                var writeData = new byte[] { state ? (byte)1 : (byte)0 };
                var packet = GetWriteVarPacket(item.AreaType, S7VarType.Bit, item.DbNumber, item.WordAddress * 8 + item.BitAddress, (ushort)writeData.Length, writeData);
                return NoLockExecute(packet);
            });
        }
        /// <summary>
        /// 写入连续的字节数组
        /// </summary>
        /// <param name="address">字符串格式开始地址</param>
        /// <param name="values">写入值字节数组</param>
        /// <returns>执行结果</returns>
        public Result WriteBytes(string address, params byte[] values)
        {
            if (values.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(values), values.Length, "The write data length must be > 0");
            var item = new S7Addresss(address);
            ValidateWriteValueLength(item.VarType, values.Length);
            return WriteBytes(item.AreaType, S7VarType.Byte, item.DbNumber, item.WordAddress, values);
        }
        /// <summary>
        /// 验证写入数据长度
        /// </summary>
        /// <param name="varType">变量类型</param>
        /// <param name="length">数据长度</param>
        /// <exception cref="ArgumentException"></exception>
        private void ValidateWriteValueLength(S7VarType varType, int length)
        {
            switch (varType)
            {
                case S7VarType.Bit:
                case S7VarType.Byte:
                    break;
                case S7VarType.Word:
                case S7VarType.Int:
                    if (length % 2 > 0)
                        throw new ArgumentException("The length of the data written must be a multiple of 2");
                    break;
                case S7VarType.DWord:
                case S7VarType.DInt:
                case S7VarType.Real:
                    if (length % 4 > 0)
                        throw new ArgumentException("The length of the data written must be a multiple of 4");
                    break;
                case S7VarType.Counter:
                case S7VarType.Timer:
                    throw new ArgumentException("Writing is not supported for the time being");
            }
        }
        public Result WriteS7CharFromDataBlock(ushort db, ushort address, char value)
        {
            return WriteBytes(S7MemoryAreaType.DataBlock, S7VarType.Byte, db, address, new byte[] { (byte)value, 0x30 });
        }
        public Result WriteS7WCharFromDataBlock(ushort db, ushort address, string value)
        {
            if (value.Length != 1)
                throw new ArgumentOutOfRangeException(nameof(value), value.Length, "The character length must be 1");
            var bytes = Encoding.Unicode.GetBytes(value).Reverse().ToArray();
            return WriteBytes(S7MemoryAreaType.DataBlock, S7VarType.Byte, db, address, bytes);
        }
        /// <summary>
        /// 写入字符串到DB块 仅支持ASCII写入
        /// </summary>
        /// <param name="dbNumber">DB块编号</param>
        /// <param name="address">地址</param>
        /// <param name="content">写入内容，非中文</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Result WriteS7StringToDataBlock(ushort dbNumber, ushort address, string content)
        {
            return EnqueueExecute(() =>
            {
                const byte MaxLength = S7StringExtension.S7StringMaximumLength;
                if (content.Length > MaxLength)
                    throw new ArgumentOutOfRangeException(nameof(content.Length), content.Length, $"Write the maximum string length to {MaxLength}.");
                var writeData = Encoding.ASCII.GetBytes(content);
                var stringPerfix = new byte[] { (byte)content.Length, (byte)content.Length };
                writeData = stringPerfix.Combine(writeData);
                var packet = GetWriteVarPacket(S7MemoryAreaType.DataBlock, S7VarType.Byte, dbNumber, address, (ushort)writeData.Length, writeData);
                return NoLockExecute(packet);
            });
        }
        /// <summary>
        /// 写入长字符串，中文字符
        /// </summary>
        /// <param name="dbNumber"></param>
        /// <param name="address"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Result WriteS7WStringToDataBlock(ushort dbNumber, ushort address, string content)
        {
            return EnqueueExecute(() =>
            {
                const ushort MaxLength = S7StringExtension.S7WStringMaximumLength;
                if (content.Length > MaxLength)
                    throw new ArgumentOutOfRangeException(nameof(content.Length), content.Length, $"Write the maximum string length to {MaxLength}.");
                var writeData = Encoding.BigEndianUnicode.GetBytes(content);
                var stringPerfix = new byte[4];
                stringPerfix[0] = (byte)((content.Length >> 8) & 0xFF);
                stringPerfix[1] = (byte)(content.Length & 0xFF);
                stringPerfix[2] = (byte)((content.Length >> 8) & 0xFF);
                stringPerfix[3] = (byte)(content.Length & 0xFF);
                writeData = stringPerfix.Combine(writeData);
                var packet = GetWriteVarPacket(S7MemoryAreaType.DataBlock, S7VarType.Byte, dbNumber, address, (ushort)writeData.Length, writeData);
                return NoLockExecute(packet);
            });
        }
        /// <summary>
        /// 批量写入字
        /// </summary>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public Result WriteWords(string address, params ushort[] values)
        {
            var data = values.ToBytes(false);
            return WriteBytes(address, data);
        }
        /// <summary>
        /// 批量写入双字
        /// </summary>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public Result WriteDWords(string address, params uint[] values)
        {
            var data = values.ToBytes(false);
            return WriteBytes(address, data);
        }
        /// <summary>
        /// 写入多个Int16
        /// </summary>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public Result WriteInts(string address, params short[] values)
        {
            var data = values.ToBytes(false);
            return WriteBytes(address, data);
        }
        /// <summary>
        /// 写入连续多个Int32
        /// </summary>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public Result WriteDInts(string address, params int[] values)
        {
            var data = values.ToBytes(false);
            return WriteBytes(address, data);
        }
        /// <summary>
        /// 写入连续多个实数
        /// </summary>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public Result WriteReals(string address, params float[] values)
        {
            var data = values.ToBytes(false);
            return WriteBytes(address, data);
        }
        /// <summary>
        /// 写入多个双精度实数
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public Result WriteLReals(S7MemoryAreaType areaType, ushort db, ushort wordAddress, params double[] values)
        {
            var data = values.ToBytes(false);
            return WriteBytes(areaType, S7VarType.Byte, db, wordAddress, data);
        }
        /// <summary>
        /// 写入结构体
        /// </summary>
        /// <param name="structValue"></param>
        /// <param name="db"></param>
        /// <param name="startByteAdr"></param>
        /// <returns></returns>
        public Result WriteStruct(object structValue, ushort db, ushort startByteAdr = 0)
        {
            //01 0B 16 00 EF 02 00 00 06 06 65 61 71 33 33 33 00 00 01 9E 7B
            var writeData = S7StructType.ToBytes(structValue);
            return WriteBytes(S7MemoryAreaType.DataBlock, S7VarType.Byte, db, startByteAdr, writeData);
        }
        /// <summary>
        /// 写入连续的毫秒
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Result WriteTimes(S7MemoryAreaType areaType, ushort db, ushort wordAddress, params TimeSpan[] milliseconds)
        {
            var data = milliseconds.SelectMany(c => c.ToBytesFromTime(false)).ToArray();
            return WriteBytes(areaType, S7VarType.Byte, db, wordAddress, data);
        }
        /// <summary>
        /// 写入连续的LTime 最小单位：纳秒
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <param name="nanoseconds"></param>
        /// <returns></returns>
        public Result WriteLTimes(S7MemoryAreaType areaType, ushort db, ushort wordAddress, params TimeSpan[] nanoseconds)
        {
            var data = nanoseconds.SelectMany(c => c.ToBytesFromLTime(false)).ToArray();
            return WriteBytes(areaType, S7VarType.Byte, db, wordAddress, data);
        }
        /// <summary>
        /// 写入连续的DT 日期+时间
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <param name="dateTimes"></param>
        /// <returns></returns>
        public Result WriteDateAndTimes(S7MemoryAreaType areaType, ushort db, ushort wordAddress, params DateTime[] dateTimes)
        {
            var data = dateTimes.SelectMany(c => c.ToBytesFromDateAndTime()).ToArray();
            return WriteBytes(areaType, S7VarType.Byte, db, wordAddress, data);
        }
        /// <summary>
        /// 写入连续的日期 
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <param name="dateTimes"></param>
        /// <returns></returns>
        public Result WriteDates(S7MemoryAreaType areaType, ushort db, ushort wordAddress, params DateTime[] dates)
        {
            var data = dates.SelectMany(c => c.ToBytesFromDate(false)).ToArray();
            return WriteBytes(areaType, S7VarType.Byte, db, wordAddress, data);
        }
        /// <summary>
        /// 写入连续的LTOD
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <param name="ltods"></param>
        /// <returns></returns>
        public Result WriteLTimeOfDays(S7MemoryAreaType areaType, ushort db, ushort wordAddress, params TimeSpan[] ltods)
        {
            var data = ltods.SelectMany(c => c.ToBytesFromLTimeOfDay(false)).ToArray();
            return WriteBytes(areaType, S7VarType.Byte, db, wordAddress, data);
        }
        /// <summary>
        /// 写入连续的LTOD
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <param name="tods"></param>
        /// <returns></returns>
        public Result WriteTimeOfDays(S7MemoryAreaType areaType, ushort db, ushort wordAddress, params TimeSpan[] tods)
        {
            var data = tods.SelectMany(c => c.ToBytesFromTimeOfDay(false)).ToArray();
            return WriteBytes(areaType, S7VarType.Byte, db, wordAddress, data);
        }
        /// <summary>
        /// 写入连续的DTL
        /// </summary>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="wordAddress"></param>
        /// <param name="dateTimes"></param>
        /// <returns></returns>
        public Result WriteDtls(S7MemoryAreaType areaType, ushort db, ushort wordAddress, params DateTime[] dateTimes)
        {
            var data = dateTimes.SelectMany(c => c.ToBytesFromDtl(false)).ToArray();
            return WriteBytes(areaType, S7VarType.Byte, db, wordAddress, data);
        }
        /// <summary>
        /// 写入值
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result Write<T>(string address, params T[] values) where T : struct
        {
            var item = new S7Addresss(address);
            if (values is bool[])
                ValidateS7VarType(item.VarType, S7VarType.Bit);
            else
                ValidateS7VarType(item.VarType, S7VarType.Byte);
            switch (values)
            {
                case bool[] bs:
                    if (bs.Length != 1)
                        throw new ArgumentOutOfRangeException(nameof(bs.Length), bs.Length, "Boolean writes currently support single operations only");
                    return WriteBoolean(address, bs.FirstOrDefault());
                case byte[] bytes:
                    return WriteBytes(address, bytes);
                case short[] int16Values:
                    return WriteInts(address, int16Values);
                case ushort[] uint16Values:
                    return WriteWords(address, uint16Values);
                case int int32Value:
                    return WriteDInts(address, int32Value);
                case uint[] uint32Values:
                    return WriteDWords(address, uint32Values);
                case float[] floatValues:
                    return WriteReals(address, floatValues);
                case double[] doubleValues:
                    return WriteLReals(item.AreaType, item.DbNumber, item.WordAddress, doubleValues);
                default:
                    break;
            }
            throw new ArgumentException("Type is not supported", nameof(values));
        }
        /// <summary>
        /// 校验S7变量类型
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <exception cref="ArgumentException"></exception>
        private static void ValidateS7VarType(S7VarType t1, S7VarType t2)
        {
            if (t1 != t2) throw new ArgumentException("Type error", nameof(S7VarType));
        }

    }
}
