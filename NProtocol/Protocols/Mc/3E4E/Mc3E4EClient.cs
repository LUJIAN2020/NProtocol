using NProtocol.Base;
using NProtocol.Connectors;
using NProtocol.Enums;
using NProtocol.Exceptions;
using NProtocol.Extensions;
using System;
using System.Linq;

namespace NProtocol.Protocols.Mc
{
    public class Mc3E4EClient : McProtocolBase
    {
        //MC协议报文格式3E/4E帧
        //请求报文：帧头+副帧头+访问路径+请求数据长度+监视定时器+请求数据
        //正常结束响应报文（有响应数据）：帧头+副帧头+访问路径+响应数据长度+结束代码+响应数据
        //正常结束响应报文（无响应数据）：帧头+副帧头+访问路径+响应数据长度+结束代码
        //异常结束响应报文：帧头+副帧头+访问路径+响应数据长度+结束代码+出错信息

        /// <summary>
        /// 正常结束代码
        /// </summary>
        public new const ushort OkEndCode = 0x0000;

        public Mc3E4EClient(EtherNetParameter parameter, ConnectMode mode)
            : base(parameter, mode)
        {
            if (mode == ConnectMode.SerialPort)
                throw new ArgumentException(
                    "Serial port connection is not supported",
                    nameof(mode)
                );
        }

        public Mc3E4EClient(string ip, ushort port, ConnectMode mode)
            : this(EtherNetParameter.Create(ip, port), mode) { }

        /// <summary>
        /// 报文帧格式
        /// </summary>
        public McFrame McFrame { get; set; } = McFrame.MC3E;

        /// <summary>
        /// 序列号
        /// </summary>
        public ushort SerialNumber { get; set; }

        /// <summary>
        /// 网络编号
        /// </summary>
        public byte NetworkNumber { get; set; }

        /// <summary>
        /// 请求单元I/O号 不同CPU值不一样，一般为管理CPU=0x03FF
        /// </summary>
        public ushort IoNumber { get; set; } = 0x03FF;

        /// <summary>
        /// 请求目标模块站号
        /// </summary>
        public byte StationNumber { get; set; }

        private byte[] CreateCommand(
            Command3E4E main,
            SubCommand3E4E sub,
            PlcMemory3E4E memory,
            uint startAddress,
            byte count,
            byte[]? writeData = default
        )
        {
            var buffer = Array.Empty<byte>();
            switch (McFrame)
            {
                case McFrame.MC3E:
                    {
                        buffer = writeData is not null ? new byte[21 + writeData.Length] : new byte[21];

                        //副头部
                        buffer[0] = 0x50;
                        buffer[1] = 0x00;
                        buffer[2] = NetworkNumber; //网络编号
                        buffer[3] = PlcNumber; //可编程控制器编号

                        //Q头部
                        buffer[4] = (byte)IoNumber; //请求目标模块I/O编号
                        buffer[5] = (byte)(IoNumber >> 8);
                        buffer[6] = StationNumber; //请求目标模块站号
                        buffer[7] = 0x00; //请求数据长度
                        buffer[8] = 0x00;
                        buffer[9] = (byte)CpuTimer; //CPU监视定时器
                        buffer[10] = (byte)(CpuTimer >> 8);

                        //主指令
                        buffer[11] = (byte)main;
                        buffer[12] = (byte)(((ushort)main) >> 8);

                        //子指令
                        buffer[13] = (byte)sub;
                        buffer[14] = (byte)(((ushort)sub) >> 8);

                        //请求数据部分
                        buffer[15] = (byte)startAddress; //起始软元件
                        buffer[16] = (byte)(startAddress >> 8);
                        buffer[17] = (byte)(startAddress >> 16);
                        buffer[18] = (byte)memory; //软元件代码
                        buffer[19] = count; //软元件点数
                        buffer[20] = 0; //读取默认为0
                        if (writeData is not null)
                            Array.Copy(writeData, 0, buffer, 21, writeData.Length);

                        //数据长度
                        int len = buffer.Length - 9;
                        buffer[7] = (byte)len;
                        buffer[8] = (byte)(len >> 8);
                        break;
                    }
                case McFrame.MC4E:
                    {
                        buffer = writeData is not null ? new byte[25 + writeData.Length] : new byte[25];

                        //副头部
                        buffer[0] = 0x54;
                        buffer[1] = 0x00;
                        buffer[2] = (byte)SerialNumber; //串行编号
                        buffer[3] = (byte)(SerialNumber >> 8);
                        buffer[4] = 0x00; //固定值
                        buffer[5] = 0x00;

                        //Q头部
                        buffer[6] = NetworkNumber; //网络编号
                        buffer[7] = PlcNumber; //可编程控制器编号
                        buffer[8] = (byte)IoNumber; //请求目标模块I/O编号
                        buffer[9] = (byte)(IoNumber >> 8);
                        buffer[10] = StationNumber; //请求目标模块站号
                        buffer[11] = 0x00; //请求数据长度
                        buffer[12] = 0x00;
                        buffer[13] = (byte)CpuTimer; //CPU监视定时器
                        buffer[14] = (byte)(CpuTimer >> 8);

                        //主指令
                        buffer[15] = (byte)main;
                        buffer[16] = (byte)(((ushort)main) >> 8);

                        //子指令
                        buffer[17] = (byte)sub;
                        buffer[18] = (byte)(((ushort)sub) >> 8);

                        //请求数据部分
                        buffer[19] = (byte)startAddress; //起始软元件
                        buffer[20] = (byte)(startAddress >> 8);
                        buffer[21] = (byte)(startAddress >> 16);
                        buffer[22] = (byte)memory; //软元件代码
                        buffer[23] = count; //软元件点数
                        buffer[24] = 0; //读取默认为0
                        if (writeData is not null)
                            Array.Copy(writeData, 0, buffer, 25, writeData.Length);

                        //数据长度
                        int len = buffer.Length - 13;
                        buffer[11] = (byte)len;
                        buffer[12] = (byte)(len >> 8);
                        break;
                    }
            }
            return buffer;
        }

        private void ValidateDataType<T>(T t, uint start, PlcMemory3E4E memory)
            where T : struct
        {
            switch (memory)
            {
                case PlcMemory3E4E.X:
                case PlcMemory3E4E.DX:
                case PlcMemory3E4E.Y:
                case PlcMemory3E4E.DY:
                case PlcMemory3E4E.B:
                case PlcMemory3E4E.SB:
                case PlcMemory3E4E.M:
                case PlcMemory3E4E.SM:
                case PlcMemory3E4E.L:
                case PlcMemory3E4E.F:
                case PlcMemory3E4E.V:
                case PlcMemory3E4E.S:
                case PlcMemory3E4E.TS:
                case PlcMemory3E4E.TC:
                case PlcMemory3E4E.STS:
                case PlcMemory3E4E.SC:
                case PlcMemory3E4E.CS:
                case PlcMemory3E4E.CC:
                    switch (t)
                    {
                        case bool:
                            break;
                        case short:
                        case ushort:
                        case int:
                        case uint:
                            if (start % 16 > 0)
                                throw new ArgumentOutOfRangeException(
                                    nameof(start),
                                    start,
                                    $"This data type {t.GetType()}, the start address value must be a multiple of 16"
                                );
                            break;
                        default:
                            throw new ArgumentException(
                                "Data type not supported",
                                t.GetType().ToString()
                            );
                    }
                    break;
                case PlcMemory3E4E.TN:
                case PlcMemory3E4E.SN:
                case PlcMemory3E4E.CN:
                    if (t is not short && t is not ushort)
                        throw new ArgumentException(
                            "Data type not supported",
                            t.GetType().ToString()
                        );
                    break;
                case PlcMemory3E4E.D:
                    break;
                case PlcMemory3E4E.SD:
                    break;
                case PlcMemory3E4E.W:
                    break;
                case PlcMemory3E4E.SW:
                    break;
                case PlcMemory3E4E.R:
                    break;
                case PlcMemory3E4E.ZR:
                    break;
                case PlcMemory3E4E.Z:
                    break;
                default:
                    break;
            }
        }

        private int ReceivedDataSliceLength() =>
            McFrame switch
            {
                McFrame.MC3E => 11,
                McFrame.MC4E => 15,
                _ => 0,
            };

        private Result ReadBytes(
            Command3E4E main,
            SubCommand3E4E sub,
            PlcMemory3E4E memory,
            uint startAddress,
            byte count
        )
        {
            return EnqueueExecute(() =>
            {
                var sendData = CreateCommand(main, sub, memory, startAddress, count, default);
                return NoLockExecute(sendData);
            });
        }

        public Result ReadBytes(string address, ushort count)
        {
            var addr = new McAddress3E4E(address);
            return ReadBytes(addr, (byte)count);
        }

        public Result ReadBytes(McAddress3E4E address, byte count)
        {
            return ReadBytes(
                Command3E4E.BatchRead,
                SubCommand3E4E.Word,
                address.Memory,
                address.StartAddress,
                count
            );
        }

        public Result<T[]> ReadValues<T>(string address, byte count)
            where T : struct
        {
            var addr = new McAddress3E4E(address);
            return ReadValues<T>(addr, count);
        }

        public Result<T[]> ReadValues<T>(McAddress3E4E address, byte count)
            where T : struct
        {
            T t = default;
            ValidateDataType(t, address.StartAddress, address.Memory);
            int sliceStart = ReceivedDataSliceLength();
            switch (t)
            {
                case bool:
                    {
                        uint start =
                            address.StartAddress % 16 > 0
                                ? address.StartAddress - (address.StartAddress % 16)
                                : address.StartAddress;

                        byte _count =
                            (address.StartAddress + count - start) % 16 > 0
                                ? (byte)((address.StartAddress + count - start) / 16 + 1)
                                : (byte)((address.StartAddress + count - start) / 16);

                        var addr = new McAddress3E4E(address.Memory, start);
                        var result = ReadBytes(addr, _count);
                        result.Payload = result.ReceivedData.Slice(sliceStart);
                        var values = result.Payload.ToBooleansFromWord(IsLittleEndian);
                        var bs = values.Slice((int)(address.StartAddress - start), count);
                        if (bs is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case byte:
                    {
                        var result = ReadBytes(address, count);
                        result.Payload = result.ReceivedData.Slice(sliceStart);
                        var values = result.Payload;
                        if (values is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case short:
                    {
                        var result = ReadBytes(address, count);
                        result.Payload = result.ReceivedData.Slice(sliceStart);
                        var values = result.Payload.ToInt16Array(IsLittleEndian);
                        if (values is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case ushort:
                    {
                        var result = ReadBytes(address, count);
                        result.Payload = result.ReceivedData.Slice(sliceStart);
                        var values = result.Payload.ToUInt16Array(IsLittleEndian);
                        if (values is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case int:
                    {
                        const int byteSize = 2;
                        var result = ReadBytes(address, (byte)(count * byteSize));
                        result.Payload = result.ReceivedData.Slice(sliceStart);
                        var values = result.Payload.ToInt32Array(IsLittleEndian);
                        if (values is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case uint:
                    {
                        const int byteSize = 2;
                        var result = ReadBytes(address, (byte)(count * byteSize));
                        result.Payload = result.ReceivedData.Slice(sliceStart);
                        var values = result.Payload.ToUInt32Array(IsLittleEndian);
                        if (values is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case float:
                    {
                        const int byteSize = 2;
                        var result = ReadBytes(address, (byte)(count * byteSize));
                        result.Payload = result.ReceivedData.Slice(sliceStart);
                        var values = result.Payload.ToFloatArray(IsLittleEndian);
                        if (values is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case long:
                    {
                        const int byteSize = 4;
                        var result = ReadBytes(address, (byte)(count * byteSize));
                        result.Payload = result.ReceivedData.Slice(sliceStart);
                        var values = result.Payload.ToInt64Array(IsLittleEndian);
                        if (values is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case ulong:
                    {
                        const int byteSize = 4;
                        var result = ReadBytes(address, (byte)(count * byteSize));
                        result.Payload = result.ReceivedData.Slice(sliceStart);
                        var values = result.Payload.ToUInt64Array(IsLittleEndian);
                        if (values is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case double:
                    {
                        const int byteSize = 4;
                        var result = ReadBytes(address, (byte)(count * byteSize));
                        result.Payload = result.ReceivedData.Slice(sliceStart);
                        var values = result.Payload.ToDoubleArray(IsLittleEndian);
                        if (values is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                default:
                    break;
            }
            throw new ArgumentException("Type not supported", nameof(T));
        }

        public Result<T> ReadValue<T>(string address)
            where T : struct
        {
            var addr = new McAddress3E4E(address);
            return ReadValue<T>(addr);
        }

        public Result<T> ReadValue<T>(McAddress3E4E address)
            where T : struct
        {
            var result = ReadValues<T>(address, 1);
            var value = result.Value.FirstOrDefault();
            return result.ToResult(value);
        }

        private Result WriteBytes(
            Command3E4E main,
            SubCommand3E4E sub,
            PlcMemory3E4E memory,
            uint startAddress,
            byte count,
            byte[] writeData
        )
        {
            return EnqueueExecute(() =>
            {
                var sendData = CreateCommand(main, sub, memory, startAddress, count, writeData);
                return NoLockExecute(sendData);
            });
        }

        public Result WriteBytes(string address, byte[] writeData)
        {
            var addr = new McAddress3E4E(address);
            return WriteBytes(addr, writeData);
        }

        public Result WriteBytes(McAddress3E4E address, byte[] writeData)
        {
            return WriteBytes(
                Command3E4E.BatchWrite,
                SubCommand3E4E.Word,
                address.Memory,
                address.StartAddress,
                (byte)writeData.Length,
                writeData
            );
        }

        public Result WriteValues<T>(string address, T[] value)
            where T : struct
        {
            var addr = new McAddress3E4E(address);
            return WriteValues(addr, value);
        }

        public Result WriteValues<T>(McAddress3E4E address, T[] writeData)
            where T : struct
        {
            T t = default;
            ValidateDataType(t, address.StartAddress, address.Memory);

            switch (writeData)
            {
                case bool[] bs:
                    {
                        if (writeData.Length % 2 > 0)
                            throw new ArgumentOutOfRangeException(
                                nameof(writeData.Length),
                                writeData.Length,
                                "Write length is a multiple of 2"
                            );

                        int count = writeData.Length;
                        var bytes = new byte[count / 2];
                        for (int i = 0; i < bs.Length; i += 2)
                        {
                            byte left = bs[i] ? (byte)0x10 : byte.MinValue;
                            byte right = bs[i + 1] ? (byte)0x01 : byte.MinValue;
                            bytes[i / 2] = (byte)(left + right);
                        }
                        return WriteBytes(
                            Command3E4E.BatchWrite,
                            SubCommand3E4E.Bit,
                            address.Memory,
                            address.StartAddress,
                            (byte)count,
                            bytes
                        );
                    }
                case short[] values:
                    {
                        int count = writeData.Length;
                        var data = values.ToBytes(!IsLittleEndian);
                        return WriteBytes(address, data);
                    }
                case ushort[] values:
                    {
                        int count = writeData.Length;
                        var data = values.ToBytes(!IsLittleEndian);
                        return WriteBytes(address, data);
                    }
                case int[] values:
                    {
                        int count = writeData.Length / 2;
                        var data = values.ToBytes(!IsLittleEndian);
                        return WriteBytes(address, data);
                    }
                case uint[] values:
                    {
                        int count = writeData.Length / 2;
                        var data = values.ToBytes(!IsLittleEndian);
                        return WriteBytes(address, data);
                    }
                case float[] values:
                    {
                        int count = writeData.Length / 2;
                        var data = values.ToBytes(!IsLittleEndian);
                        return WriteBytes(address, data);
                    }
                case long[] values:
                    {
                        int count = writeData.Length / 4;
                        var data = values.ToBytes(!IsLittleEndian);
                        return WriteBytes(address, data);
                    }
                case ulong[] values:
                    {
                        int count = writeData.Length / 4;
                        var data = values.ToBytes(!IsLittleEndian);
                        return WriteBytes(address, data);
                    }
                case double[] values:
                    {
                        int count = writeData.Length / 4;
                        var data = values.ToBytes(!IsLittleEndian);
                        return WriteBytes(address, data);
                    }
                default:
                    throw new ArgumentException("Type not supported", t.GetType().ToString());
            }
        }

        public Result WriteValue<T>(string address, T value)
            where T : struct
        {
            return WriteValues(address, new T[] { value });
        }

        public Result WriteValue<T>(McAddress3E4E address, T value)
            where T : struct
        {
            return WriteValues(address, new T[] { value });
        }

        protected override ReadOnlySpan<byte> ExtractPayload(ReadOnlySpan<byte> writeData, ReadOnlySpan<byte> readData)
        {
            if (ValidateReceivedData(writeData, readData))
            {
                return readData;
            }
            return ReadOnlySpan<byte>.Empty;
        }

        private bool ValidateReceivedData(ReadOnlySpan<byte> sendData, ReadOnlySpan<byte> receivedData)
        {
            if (receivedData.Length < 11)
                return false;
            switch (McFrame)
            {
                case McFrame.MC3E:
                    {
                        int dataLen = receivedData[7] + receivedData[8] * 256;
                        int endCode = receivedData[9] + receivedData[10] * 256;
                        if (
                            sendData[0] + 0x80 == receivedData[0]
                            && sendData[1] == receivedData[1]
                            && sendData[2] == receivedData[2]
                            && sendData[3] == receivedData[3]
                            && sendData[4] == receivedData[4]
                            && sendData[5] == receivedData[5]
                            && sendData[6] == receivedData[6]
                            && dataLen == receivedData.Length - 9
                            && endCode == 0
                        )
                        {
                            //写入只会有返回数据
                            //读取会有数据负载在后面
                            return true;
                        }
                        else
                        {
                            var errorCode = receivedData.Slice(9, 2).ToArray().ToHexString();
                            var errorData = receivedData.Slice(11).ToArray().ToHexString();
                            throw new ReceivedException(
                                $"Response data error, error code:{errorCode},error data:{errorData}",
                                sendData.ToArray(),
                                receivedData.ToArray(),
                                DriverId
                            );
                        }
                    }
                case McFrame.MC4E:
                    {
                        int sn = receivedData[2] + receivedData[3] * 256;
                        int dataLen = receivedData[11] + receivedData[12] * 256;
                        int endCode = receivedData[13] + receivedData[14] * 256;
                        if (
                            sendData[0] + 0x80 == receivedData[0]
                            && sendData[1] == receivedData[1]
                            && sn == SerialNumber
                            && sendData[4] == receivedData[4]
                            && sendData[5] == receivedData[5]
                            && sendData[6] == receivedData[6]
                            && sendData[7] == receivedData[7]
                            && sendData[8] == receivedData[8]
                            && sendData[9] == receivedData[9]
                            && sendData[10] == receivedData[10]
                            && dataLen == receivedData.Length - 13
                            && endCode == 0
                        )
                        {
                            SerialNumber++;
                            //写入只会有返回数据
                            //读取会有数据负载在后面
                            return true;
                        }
                        else
                        {
                            var errorCode = receivedData.Slice(13, 2).ToArray();
                            var errorData = receivedData.Slice(15).ToArray().ToHexString();
                            throw new ReceivedException(
                                $"Response data error, error code:{string.Join("", errorCode.Select(c => c.ToString("X2")))},error data:{errorData}",
                                sendData.ToArray(),
                                receivedData.ToArray(),
                                DriverId
                            );
                        }
                    }
                default:
                    return false;
            }
        }
    }
}
