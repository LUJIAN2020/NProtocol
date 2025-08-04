using NProtocol.Communication.Base;
using NProtocol.Communication.Connectors;
using NProtocol.Communication.Enums;
using NProtocol.Communication.Exceptions;
using NProtocol.Communication.Extensions;
using Robot.Communication.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NProtocol.Protocols.Fins
{
    public class FinsClient : DriverBase
    {
        public FinsClient(EtherNetParameter parameter, FinsConnectMode mode) : base(parameter, GetConnectMode(mode))
        {
            FinsConnectMode = mode;
        }
        public FinsClient(string ip, ushort port, FinsConnectMode mode) : this(EtherNetParameter.Create(ip, port), mode)
        {
        }
        public FinsClient(string ip, FinsConnectMode mode) : this(ip, 9600, mode)
        {
        }
        private static ConnectMode GetConnectMode(FinsConnectMode mode) => mode switch
        {
            FinsConnectMode.FinsTcp => ConnectMode.Tcp,
            FinsConnectMode.FinsUdp => ConnectMode.Udp,
            _ => throw new ArgumentException($"Unsupported modbus connect mode `{mode}`"),
        };
        public const string FINS = "FINS";
        public const byte ICF_COMMAND = 0x80;
        public const byte ICF_RESPONSE = 0xC0;
        public const byte RSV = 0;
        public const byte GCT = 2;
        public byte DNA { get; set; } = 0x00;
        public byte DA1 { get; set; }
        public byte DA2 { get; set; } = 0x00;
        public byte SNA { get; set; } = 0x00;
        public byte SA1 { get; set; }
        public byte SA2 { get; set; }
        public byte SID { get; internal set; } = byte.MaxValue;
        public bool IsLittleEndian { get; } = true;
        public FinsConnectMode FinsConnectMode { get; }
        private byte[] CreateCommand(FinsCommand finsCommand, FinsAddress finsAddress, ushort length = 1, byte[]? writeData = null)
        {
            return CreateCommand(finsCommand, finsAddress.PlcMemory, finsAddress.DataType,
                finsAddress.AddressWord, finsAddress.AddressBit,
                finsAddress.Bank,
                finsAddress.StringLength, finsAddress.StringFormat,
                length, writeData);
        }
        private byte[] CreateCommand(FinsCommand finsCommand, PlcMemory memory, DataType dataType,
            ushort addressWord, byte addressBit = 0,
            byte? bank = null,
            ushort stringLength = 0, StringFormatType? stringFormat = null,
            ushort length = 1, byte[]? writeData = null)
        {
            byte[] body = new byte[18];
            body[0] = ICF_COMMAND;//ICF 10000000:command 11000000:response；
            body[1] = RSV;//RSV；
            body[2] = GCT;//GCT；
            body[3] = DNA;//DNA PLC网络号；
            body[4] = DA1;//DA1 PLC节点号；
            body[5] = DA2;//DA2 PLC单元号；PLC 侧直接对 CPU 操作，与以太网模块实际单元号没有关系，固定为 0。
            body[6] = SNA;//SNA PC网络号；
            body[7] = SA1;//SA1 PC节点地址；
            body[8] = SA2;//SA2 PC单元地址；
            body[9] = SID;//SID ServerId；
            bool isWrite = finsCommand == FinsCommand.WriteMemoryArea;
            //功能码 0x01 0x01读 0x01 0x02写
            ushort func = (ushort)finsCommand;
            body[10] = (byte)(func >> 8);
            body[11] = (byte)func;

            //地址码
            if (bank is not null)
            {
                byte mCode = GetMemoryCode(memory, dataType, isWrite);
                body[12] = (byte)(mCode + bank);
            }
            else
            {
                //读地址区(D位:02,D字:82,W位:31,C位:30,W字:B1,C字:B0)；
                body[12] = GetMemoryCode(memory, dataType, isWrite);
            }

            int word = addressWord;
            if (memory == PlcMemory.C)
            {
                word += 0x8000;
            }

            //起始字地址
            body[13] = (byte)(word >> 8);
            body[14] = (byte)word;
            int dataLength = 0;
            if (isWrite)
            {
                //位按位写入 字按字写入
                body[15] = dataType == DataType.Bit
                    ? addressBit
                    : byte.MinValue;
                dataLength = length;
            }
            else
            {
                //读位和字全部使用读byte
                //读取长度 最小读取长度1Word=2Bytes
                body[15] = byte.MinValue;//位地址
                switch (dataType)
                {
                    case DataType.Bit:
                        {
                            int bitLen = addressBit + length;
                            dataLength = bitLen % 16 == 0
                                ? bitLen / 16
                                : bitLen / 16 + 1;
                            break;
                        }
                    case DataType.Word:
                        dataLength = length;
                        break;
                    case DataType.String:
                        {
                            dataLength = stringFormat is StringFormatType.H or StringFormatType.L
                                ? stringLength / 2
                                : stringLength * 2;
                            break;
                        }
                }
            }
            body[16] = (byte)(dataLength >> 8);
            body[17] = (byte)dataLength;

            switch (FinsConnectMode)
            {
                case FinsConnectMode.FinsTcp:
                    {

                        var header = new byte[16] {
                                    70, 73, 78, 83,//FINS
                                    0, 0, 0, 0,//指后面跟的字节长度；
                                    0, 0, 0, 0x02,//固定命令；
                                    0, 0, 0, 0 };//错误代码；
                        //读 固定长度0x1A
                        if (!isWrite)//读
                        {
                            header[6] = 0;
                            header[7] = 0x1A;
                        }
                        else//写
                        {
                            int byteLength = dataType == DataType.Bit
                                 ? length + 0x1A
                                 : length * 2 + 0x1A;
                            header[6] = (byte)(byteLength >> 8);
                            header[7] = (byte)byteLength;
                        }
                        if (writeData is not null)
                        {
                            return header.Combine(body, writeData);
                        }
                        return header.Combine(body);
                    }
                case FinsConnectMode.FinsUdp:
                    {
                        if (writeData is not null)
                        {
                            return body.Combine(writeData);
                        }
                        return body;
                    }
            }
            return Array.Empty<byte>();
        }
        private bool ValidateFrameCode(byte main, byte sub)
        {
            switch (main)
            {
                case 0:
                    switch (sub)
                    {
                        case 0:
                        case 0x40://错误码64，是因为PLC中产生了报警，但是数据还是能正常读到的，屏蔽64报警或清除plc错误可解决
                            return true;
                        case 1:
                            return false;
                    }
                    break;
                case 1:
                    switch (sub)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                            return false;
                    }
                    break;
                case 2:
                    switch (sub)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                            return false;
                    }
                    break;
                case 3:
                    switch (sub)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            return false;
                    }
                    break;
                case 4:
                    switch (sub)
                    {
                        case 1:
                        case 2:
                            return false;
                    }
                    break;
                case 5:
                    switch (sub)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            return false;
                    }
                    break;
                case 16:
                    switch (sub)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                            return false;
                    }
                    break;
                case 17:
                    switch (sub)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 6:
                        case 9:
                        case 10:
                        case 11:
                        case 12:
                            return false;
                    }
                    break;
                case 32:
                    switch (sub)
                    {
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                            return false;
                    }
                    break;
                case 33:
                    switch (sub)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                            return false;
                    }
                    break;
                case 34:
                    switch (sub)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                            return false;
                    }
                    break;
                case 35:
                    switch (sub)
                    {
                        case 1:
                        case 2:
                        case 3:
                            return false;
                    }
                    break;
                case 36:
                    {
                        byte b3 = sub;
                        if (b3 != 1)
                        {
                            break;
                        }
                        return false;
                    }
                case 37:
                    switch (sub)
                    {
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 9:
                        case 10:
                        case 13:
                        case 15:
                        case 16:
                            return false;
                    }
                    break;
                case 38:
                    switch (sub)
                    {
                        case 1:
                        case 2:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                            return false;
                    }
                    break;
                case 48:
                    {
                        byte b2 = sub;
                        if (b2 != 1)
                        {
                            break;
                        }
                        return false;
                    }
                case 64:
                    {
                        byte b = sub;
                        if (b != 1)
                        {
                            break;
                        }
                        return false;
                    }
            }
            return false;
        }
        private byte GetMemoryCode(PlcMemory plcMemory, DataType dataType, bool isWrite)
        {
            if (dataType == DataType.Bit && isWrite)
            {
                return plcMemory switch
                {
                    PlcMemory.CIO => 0x30,
                    PlcMemory.W => 0x31,
                    PlcMemory.H => 0x32,
                    PlcMemory.A => 0x33,
                    PlcMemory.D => 0x02,
                    PlcMemory.T => 0x09,
                    PlcMemory.C => 0x09,
                    PlcMemory.EB => 0x20,//EB地址需要加上bank号码
                    _ => throw new NotSupportedException($"寄存器类型:{plcMemory},数据类型:{dataType}不支持"),
                };
            }
            return plcMemory switch
            {
                PlcMemory.CIO => 0xB0,
                PlcMemory.W => 0xB1,
                PlcMemory.H => 0xB2,
                PlcMemory.A => 0xB3,
                PlcMemory.D => 0x82,
                PlcMemory.T => 0x89,
                PlcMemory.C => 0x89,
                PlcMemory.E => 0x98,//E地址只支持word
                PlcMemory.EB => 0xA0,//EB地址需要加上bank号码
                _ => throw new NotSupportedException($"寄存器类型:{plcMemory},数据类型:{dataType}不支持"),
            };
        }
        protected override byte[]? ExtractPayload(byte[] writeData, byte[] readData)
        {
            if (ValidateReceiveData(readData))
            {
                SID--;
                return readData;
            }
            return default;
        }
        private byte[] ValidateReceiveDataToPayload(byte[] sendData, byte[] receiveData)
            => FinsConnectMode switch
            {
                FinsConnectMode.FinsTcp => ValidateReceiveDataToPayloadFinsTcp(sendData, receiveData),
                FinsConnectMode.FinsUdp => ValidateReceiveDataToPayloadFinsUdp(sendData, receiveData),
                _ => Array.Empty<byte>(),
            };
        private byte[] ValidateReceiveDataToPayloadFinsTcp(byte[] sendData, byte[] receiveData)
        {
            if (!ValidateFrameCode(receiveData[28], receiveData[29]))
            {
                var errorCode = receiveData.Slice(28, 2).ToUInt16();
                if (errorCode != 0)
                {
                    throw new FinsErrorCodeException(errorCode, sendData, receiveData, DriverId);
                }
            }
            if (receiveData.Length < 29)
                throw new ReceivedException($"The response data length error,length:{receiveData.Length}", sendData, receiveData, DriverId);

            return receiveData.Slice(30);
        }
        private byte[] ValidateReceiveDataToPayloadFinsUdp(byte[] sendData, byte[] receiveData)
        {
            if (!ValidateFrameCode(receiveData[12], receiveData[13]))
            {
                var errorCode = receiveData.Slice(12, 2).ToUInt16();
                if (errorCode != 0)
                {
                    throw new FinsErrorCodeException(errorCode, sendData, receiveData, DriverId);
                }
            }
            if (receiveData.Length < 14)
                throw new ReceivedException($"The response data length error,length:{receiveData.Length}", sendData, receiveData, DriverId);

            return receiveData.Slice(14);
        }
        private bool ValidateReceiveData(byte[] receiveData)
            => FinsConnectMode switch
            {
                FinsConnectMode.FinsTcp => ValidateReceiveDataFinsTcp(receiveData),
                FinsConnectMode.FinsUdp => ValidateReceiveDataFinsUdp(receiveData),
                _ => false,
            };
        private bool ValidateReceiveDataFinsTcp(byte[] receiveData)
        {
            int readLength = receiveData.Length;
            int len = receiveData.Slice(4, 4).ToInt32();
            return readLength >= 30
                && receiveData[0] == FINS[0]
                && receiveData[1] == FINS[1]
                && receiveData[2] == FINS[2]
                && receiveData[3] == FINS[3]
                && readLength == len + 8
                && receiveData[16] == ICF_RESPONSE
                && receiveData[17] == RSV
                && receiveData[18] == GCT
                && receiveData[19] == SNA
                && receiveData[20] == SA1
                && receiveData[21] == SA2
                && receiveData[22] == DNA
                && receiveData[23] == DA1
                && receiveData[24] == DA2
                && receiveData[25] == SID;
        }
        private bool ValidateReceiveDataFinsUdp(byte[] receiveData)
        {
            return receiveData.Length >= 14
                 && receiveData[0] == ICF_RESPONSE
                 && receiveData[1] == RSV
                 && receiveData[2] == GCT
                 && receiveData[3] == SNA
                 && receiveData[4] == SA1
                 && receiveData[5] == SA2
                 && receiveData[6] == DNA
                 && receiveData[7] == DA1
                 && receiveData[8] == DA2
                 && receiveData[9] == SID;
        }
        /// <summary>
        /// 前置头部错误码字典
        /// </summary>
        private static readonly Dictionary<uint, string> headerErrorCodeValues = new()
        {
            {0x00000000, "正常" },
            {0x00000001, "头不是‘FINS’ (ASCII code)" },
            {0x00000002, "数据太长" },
            {0x00000003, "不支持的命令" },
            {0x00000020, "所有的连接被占用" },
            {0x00000021, "制定的节点已经连接" },
            {0x00000022, "未被指定的IP地址试图访问一个被保护的节点" },
            {0x00000023, "客户端FINS节点地址超范围" },
            {0x00000024, "相同的FINS节点地址已经被使用" },
            {0x00000025, "所有可用的节点地址都已使用" }
        };
        public Encoding FinsStringEncoding { get; set; } = Encoding.ASCII;
        internal static string GetPayloadToString(byte[] payload, StringFormatType? format, Encoding encoding)
        {
            switch (format)
            {
                case StringFormatType.H:
                    //字节排序由高到低
                    return encoding.GetString(payload);
                case StringFormatType.L:
                    //采用由低到高字节排序的字符串
                    return encoding.GetString(payload.Reverse().ToArray());
                case StringFormatType.D:
                    {
                        //仅使用每个字的高位字节
                        byte[] buffer = new byte[payload.Length / 2];
                        for (int i = 0; i < payload.Length; i += 2)
                        {
                            buffer[i / 2] = payload[i];
                        }
                        return encoding.GetString(buffer);
                    }
                case StringFormatType.E:
                    {
                        //仅使用每个字的低位字节
                        byte[] buffer = new byte[payload.Length / 2];
                        for (int i = 0; i < payload.Length; i += 2)
                        {
                            buffer[i / 2] = payload[i + 1];
                        }
                        return encoding.GetString(buffer);
                    }
                default:
                    throw new ArgumentNullException(nameof(format));
            }
        }
        public Result ReadBytes(FinsAddress finsAddress, ushort count)
        {
            if (count == 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "The number of reads must be > 0");

            return EnqueueExecute(() =>
            {
                var sendData = CreateCommand(FinsCommand.ReadMemoryArea, finsAddress, count);
                var result = NoLockExecute(sendData);
                result.Payload = ValidateReceiveDataToPayload(result.SendData, result.ReceivedData);
                return result;
            });
        }
        public Result ReadBytes(string address, ushort count)
        {
            var addr = new FinsAddress(address);
            return ReadBytes(addr, count);
        }
        public string ReadToString(string address)
        {
            var addr = new FinsAddress(address);
            return ReadToString(addr);
        }
        public string ReadToString(FinsAddress finsAddress)
        {
            if (finsAddress.DataType != DataType.String)
                throw new ArgumentException(nameof(finsAddress.DataType), "The data type must be `String`");

            return EnqueueExecute(() =>
            {
                //读字符串
                //E10:10.10H
                //80 00 02 00 2C 00 00 2C 00 03 01 01 AA 00 0A 00 00 05 

                //E10:100.10L
                //80 00 02 00 2C 00 00 2C 00 02 01 01 AA 00 64 00 00 05

                //E10:150:10D
                //80 00 02 00 2C 00 00 2C 00 03 01 01 AA 00 96 00 00 0A

                //E10:200.10E
                //80 00 02 00 2C 00 00 2C 00 03 01 01 AA 00 C8 00 00 0A
                var sendData = CreateCommand(FinsCommand.ReadMemoryArea, finsAddress.PlcMemory, finsAddress.DataType,
                    finsAddress.AddressWord, 0,
                    finsAddress.Bank,
                    finsAddress.StringLength, finsAddress.StringFormat);
                var result = NoLockExecute(sendData);
                result.Payload = ValidateReceiveDataToPayload(result.SendData, result.ReceivedData);
                return GetPayloadToString(result.Payload, finsAddress.StringFormat, FinsStringEncoding);
            });
        }
        public Result<T[]> Read<T>(FinsAddress finsAddress, ushort count) where T : struct
        {
            T t = default;
            switch (t)
            {
                case bool:
                    {
                        if (finsAddress.PlcMemory == PlcMemory.C || finsAddress.PlcMemory == PlcMemory.T)
                            throw new ArgumentException("Timer (T), Counter (C) do not support bit operation", nameof(finsAddress.PlcMemory));
                        var result = ReadBytes(finsAddress, count);
                        var bs = result.Payload.ToBooleansFromWord().Slice(finsAddress.AddressBit, count);
                        if (bs is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case byte:
                    {
                        const int wordSize = 1;
                        var result = ReadBytes(finsAddress, (ushort)(count * wordSize));
                        if (result.Payload is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case short:
                    {
                        const int wordSize = 1;
                        var result = ReadBytes(finsAddress, (ushort)(count * wordSize));
                        var payload = result.Payload;
                        if (payload.ToInt16Array() is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case ushort:
                    {
                        const int wordSize = 1;
                        var result = ReadBytes(finsAddress, (ushort)(count * wordSize));
                        var payload = result.Payload;
                        if (payload.ToUInt16Array() is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case int:
                    {
                        const int wordSize = 2;
                        var result = ReadBytes(finsAddress, (ushort)(count * wordSize));
                        var payload = result.Payload;
                        if (payload.ToInt32Array() is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case uint:
                    {
                        const int wordSize = 2;
                        var result = ReadBytes(finsAddress, (ushort)(count * wordSize));
                        var payload = result.Payload;
                        if (payload.ToUInt32Array() is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case float:
                    {
                        const int wordSize = 2;
                        var result = ReadBytes(finsAddress, (ushort)(count * wordSize));
                        var payload = result.Payload;
                        if (payload.ToFloatArray() is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case double:
                    {
                        const int wordSize = 4;
                        var result = ReadBytes(finsAddress, (ushort)(count * wordSize));
                        var payload = result.Payload;
                        if (payload.ToDoubleArray() is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case long:
                    {
                        const int wordSize = 4;
                        var result = ReadBytes(finsAddress, (ushort)(count * wordSize));
                        var payload = result.Payload;
                        if (payload.ToInt64Array() is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case ulong:
                    {
                        const int wordSize = 4;
                        var result = ReadBytes(finsAddress, (ushort)(count * wordSize));
                        var payload = result.Payload;
                        if (payload.ToUInt64Array() is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                default:
                    break;
            }
            throw new ArgumentException("Type not supported", nameof(T));
        }
        public Result<T[]> Read<T>(string address, ushort count) where T : struct
        {
            var addr = new FinsAddress(address);
            return Read<T>(addr, count);
        }
        public Result<T> Read<T>(string address) where T : struct
        {
            var result = Read<T>(address, 1);
            var first = result.Value.FirstOrDefault();
            return result.ToResult(first);
        }
        public Result<T> Read<T>(FinsAddress finsAddress) where T : struct
        {
            var result = Read<T>(finsAddress, 1);
            var first = result.Value.FirstOrDefault();
            return result.ToResult(first);
        }
        public Result WriteBytes(FinsAddress finsAddress, params byte[] values)
        {
            return EnqueueExecute(() =>
            {
                if (values.Length % 2 > 0)
                    throw new ArgumentOutOfRangeException(nameof(values.Length), values.Length, "The minimum write data unit is 1Word=2Byte");

                if (finsAddress.DataType == DataType.Bit)
                    throw new ArgumentException($"This method does not support bit write,{finsAddress}", nameof(finsAddress));

                var sendData = CreateCommand(FinsCommand.WriteMemoryArea, finsAddress, (ushort)(values.Length / 2), values);
                var result = NoLockExecute(sendData);
                result.Payload = ValidateReceiveDataToPayload(result.SendData, result.ReceivedData);
                return result;
            });
        }
        public Result WriteBytes(string address, params byte[] values)
        {
            var addr = new FinsAddress(address);
            return WriteBytes(addr, values);
        }
        public Result WriteBooleans(FinsAddress finsAddress, params bool[] values)
        {
            return EnqueueExecute(() =>
            {
                if (finsAddress.DataType != DataType.Bit)
                    throw new ArgumentException($"The write address must be a bit address,{finsAddress}", nameof(finsAddress));

                var writeData = values.Select(c => c ? (byte)1 : byte.MinValue).ToArray();
                var sendData = CreateCommand(FinsCommand.WriteMemoryArea, finsAddress, (ushort)values.Length, writeData);
                var result = NoLockExecute(sendData);
                result.Payload = ValidateReceiveDataToPayload(result.SendData, result.ReceivedData);
                return result;
            });
        }
        public Result WriteBooleans(string address, params bool[] values)
        {
            var addr = new FinsAddress(address);
            return WriteBooleans(addr, values);
        }
        public Result Write<T>(FinsAddress finsAddress, params T[] values)
        {
            return values switch
            {
                bool[] vals => WriteBooleans(finsAddress, vals),
                byte[] vals => WriteBytes(finsAddress, vals),
                short[] vals => WriteBytes(finsAddress, vals.ToBytes(false)),
                ushort[] vals => WriteBytes(finsAddress, vals.ToBytes(false)),
                int[] vals => WriteBytes(finsAddress, vals.ToBytes(false)),
                uint[] vals => WriteBytes(finsAddress, vals.ToBytes(false)),
                float[] vals => WriteBytes(finsAddress, vals.ToBytes(false)),
                double[] vals => WriteBytes(finsAddress, vals.ToBytes(false)),
                long[] vals => WriteBytes(finsAddress, vals.ToBytes(false)),
                ulong[] vals => WriteBytes(finsAddress, vals.ToBytes(false)),
                _ => throw new ArgumentException("Type not supported", nameof(T)),
            };
        }
        public Result Write<T>(string address, params T[] values) where T : struct
        {
            var addr = new FinsAddress(address);
            return Write(addr, values);
        }

        private readonly byte[] handshakeCommand =
        {
            0x46, 0x49, 0x4E, 0x53, // 'F' 'I' 'N' 'S'
			0x00, 0x00, 0x00, 0x0C,	// 指后面跟的字节长度；
			0x00, 0x00, 0x00, 0x00,	// 固定命令； (0 client to server, 1 server to client)
			0x00, 0x00, 0x00, 0x00,	// 错误代码；
			0x00, 0x00, 0x00, 0x00  // PC节点IP，当设置为0时，会自动获取节点IP。
        };
        public void SendHandshakeCommand(int timeout = 1000)
        {
            if (FinsConnectMode == FinsConnectMode.FinsUdp)
                throw new Exception("FinsUdp does not support Handshake and does not need to be called");

            Write(handshakeCommand);
            Thread.Sleep(10);
            int offset = 0;
            var receivedData = new byte[24];
            var now = DateTime.Now;
            while (true)
            {
                var received = Read();
                Array.Copy(received, 0, receivedData, offset, received.Length);
                offset += received.Length;
                if (offset == 24)
                {
                    byte[] error = received.Slice(12, 4);
                    int errorCode = BitConverter.ToInt32(error, 0);
                    if (errorCode > 0)
                        throw new ExecuteException($"Handshake exception,error code:{error.ToHexString()}", handshakeCommand, received, DriverId);
                    SA1 = received[19];
                    DA1 = received[23];
                    break;
                }

                if ((DateTime.Now - now).TotalMilliseconds > timeout)
                {
                    DiscardBuffer();
                    throw new Exception("Handshake timeout");
                }
            }
        }

#if DEBUG

        #region HandshakeCommand

        /// <summary>
        /// 握手指令
        /// </summary>
        private byte[] handshakeCmd = new byte[]
        {
            0x46, 0x49, 0x4E, 0x53, // 'F' 'I' 'N' 'S'
			0x00, 0x00, 0x00, 0x0C,	// 指后面跟的字节长度；
			0x00, 0x00, 0x00, 0x00,	// 固定命令； (0 client to server, 1 server to client)
			0x00, 0x00, 0x00, 0x00,	// 错误代码；
			0x00, 0x00, 0x00, 0x00  // PC节点IP，当设置为0时，会自动获取节点IP。
        };

        /// <summary>
        /// 握手返回数据（示例）
        /// </summary>
        private byte[] handshakeResponse = new byte[]
        {
            0x46, 0x49, 0x4E, 0x53, // 'F' 'I' 'N' 'S'
			0x00, 0x00, 0x00, 0x10,	// 指后面跟的字节长度；
			0x00, 0x00, 0x00, 0x01,	// 固定命令； (0 client to server, 1 server to client)
			0x00, 0x00, 0x00, 0x00,	// 错误代码；
			0x00, 0x00, 0x00, 0x18, // 本机电脑节点IP；
			0x00, 0x00, 0x00, 0x17  // PLC节点IP。
        };

        #endregion

        #region ReadCommand

        /// <summary>
        /// 读指令（示例）
        /// </summary>
        private byte[] readFinsTcpCmd = new byte[]
        {
            70, 73, 78, 83, //FINS
            0, 0, 0, 0, //指后面跟的字节长度；
            0, 0, 0, 0, //固定命令；
            0, 0, 0, 0, //错误代码；
            0, //ICF；
            0, //RSV；
            0, //GCT；
            0, //PLC网络地址；
            0, //PLC节点地址；
            0, //PLC单元地址； 
            0, //PC网络地址；
            0, //PC节点地址；
            0, //PC单元地址；
            0, //SID；
            01, 01, //读指令；
            0, //读地址区(D位:02,D字:82,W位:31,C位:30,W字:B1,C字:B0)；
            0, 0, 0, //起始地址；
            0, 0 //读个数。最多100个地址
        };

        /// <summary>
        /// 读后返回数据（示例）
        /// </summary>
        private byte[] readFinsTcpResponse = new byte[]
        {
            70, 73, 78, 83, //FINS
            0, 0, 0, 0, //指后面跟的字节长度；
            0, 0, 0, 0, //固定命令；
            0, 0, 0, 0, //错误代码；
            0, //ICF；
            0, //RSV；
            0, //GCT；
            0, //PC网络地址；
            0, //PC节点地址；
            0, //PC单元地址； 
            0, //PLC网络地址；
            0, //PLC节点地址；
            0, //PLC单元地址；
            0, //SID；
            01, 01, //读指令；
            0, 0, //读取成功标识；
            0, 0, //读到的数据，最小单位一个字，2个字节
        };

        /// <summary>
        /// 读指令（示例）
        /// PC：10.110.59.192
        /// PLC：10.110.59.3
        /// 读D100
        /// </summary>
        private byte[] readFinsUdpCmd = new byte[]
        {
            0x80, //ICF 10000000:command 11000000:response；
            0, //RSV；
            2, //GCT；
            0, //DNA PLC网络号；
            3, //DA1 PLC节点号；
            0, //DA2 PLC单元号；PLC 侧直接对 CPU 操作，与以太网模块实际单元号没有关系，固定为 0。
            0, //SNA PC网络号；
            0xC0, //SA1 PC节点地址；
            0, //SA2 PC单元地址；
            0, //SID；
            01, 01, //读指令；
            0x82, //读地址区(D位:02,D字:82,W位:31,C位:30,W字:B1,C字:B0)；
            0, 0x64, 0, //起始地址；
            0, 1 //读个数。最多100个地址
        };

        /// <summary>
        /// 读后返回数据（示例）
        /// PC：10.110.59.192
        /// PLC：10.110.59.3
        /// 读D100返回数据
        /// </summary>
        private byte[] readFinsUdpResponse = new byte[]
        {
            0xC0, //ICF 10000000:command 11000000:response；
            0, //RSV；
            2, //GCT；
            0, //SNA PC网络号；
            0xC0, //SA1 PC节点地址；
            0, //SA2 PC单元地址； 
            0, //DNA PLC网络号；
            3, //DA1 PLC节点号；
            0, //DA2 PLC单元号；PLC 侧直接对 CPU 操作，与以太网模块实际单元号没有关系，固定为 0。
            0, //SID；
            01, 01, //读指令；
            0, 0, //读取成功标识；
            0x01, 0x23, //读到的数据，最小单位一个字，2个字节
        };

        #endregion

        #region WriteCommand

        /// <summary>
        /// 写指令，长度不固定，根据写入内容（示例）
        /// </summary>
        private byte[] writeFinsTcpCmd = new byte[]
        {
            70, 73, 78, 83, //FINS
            0, 0, 0, 0, //指后面跟的字节长度；
            0, 0, 0, 0, //固定命令；
            0, 0, 0, 0, //错误代码；
            0, //ICF；
            0, //RSV；
            0, //GCT；
            0, //PLC网络地址；
            0, //PLC节点地址；
            0, //PLC单元地址； 
            0, //PC网络地址；
            0, //PC节点地址；
            0, //PC单元地址；
            0, //SID；
            01, 02, //写指令；
            0, //读地址区(D位:02,D字:82,W位:31,C位:30,W字:B1,C字:B0)；
            0, 0, 0, //起始地址；
            0, 1, //写个数。一个字=2个字节
            0XFF,0XFF
        };

        /// <summary>
        /// 写指令，长度不固定，根据写入内容（示例）
        /// </summary>
        private byte[] writeFinsTcpResponse = new byte[]
        {
            70, 73, 78, 83, //FINS
            0, 0, 0, 0, //指后面跟的字节长度；
            0, 0, 0, 0, //固定命令；
            0, 0, 0, 0, //错误代码；
            0, //ICF；
            0, //RSV；
            0, //GCT；
            0, //PC网络地址；
            0, //PC节点地址；
            0, //PC单元地址； 
            0, //PLC网络地址；
            0, //PLC节点地址；
            0, //PLC单元地址；
            0, //SID；
            01, 02, //写指令；
            0, 0 //写入成功指令
        };

        /// <summary>
        /// 写指令，长度不固定，根据写入内容（示例）
        /// PC：10.110.59.192
        /// PLC：10.110.59.3
        /// 写W0.05
        /// </summary>
        private byte[] writeFinsUdpCmd = new byte[]
        {
            0x80, //ICF；
            0, //RSV；
            2, //GCT；
            0, //DNA PLC网络号；
            3, //DA1 PLC节点号；
            0, //DA2 PLC单元号；PLC 侧直接对 CPU 操作，与以太网模块实际单元号没有关系，固定为 0。
            0, //SNA PC网络号；
            0xC0, //SA1 PC节点地址；
            0, //SA2 PC单元地址；
            0, //SID；
            01, 02, //写指令；
            0x31, //读地址区(D位:02,D字:82,W位:31,C位:30,W字:B1,C字:B0)；
            0, 0, 5, //起始地址；
            0, 1, //写个数。一个Word=2个字节；单个位的话一个字节
            1
        };

        /// <summary>
        /// 写指令，长度不固定，根据写入内容（示例）
        /// PC：10.110.59.192
        /// PLC：10.110.59.3
        /// 写W0.05 返回数据
        /// </summary>
        private byte[] writeFinsUdpResponse = new byte[]
        {
            0xC0, //ICF 10000000:command 11000000:response；
            0, //RSV；
            2, //GCT；
            0, //SNA PC网络号；
            0xC0, //SA1 PC节点地址；
            0, //SA2 PC单元地址； 
            0, //DNA PLC网络号；
            3, //DA1 PLC节点号；
            0, //DA2 PLC单元号；PLC 侧直接对 CPU 操作，与以太网模块实际单元号没有关系，固定为 0。
            0, //SID；
            01, 02, //写指令；
            0, 0 //写入成功指令
        };

        #endregion

#endif

    }
}