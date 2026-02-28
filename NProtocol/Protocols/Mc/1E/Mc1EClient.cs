using NProtocol.Base;
using NProtocol.Connectors;
using NProtocol.Enums;
using NProtocol.Exceptions;
using NProtocol.Extensions;
using System;
using System.Linq;

namespace NProtocol.Protocols.Mc
{
    /// <summary>
    /// Mitsubishi MC protocol is not tested and is temporarily unavailable
    /// </summary>
    internal class Mc1EClient : McProtocolBase
    {
        //MC协议报文格式1E帧
        //请求报文：帧头+副帧头+可编程控制器编号+ACPU监视定时器+请求数据
        //正常结束响应报文（有响应数据）：帧头+副帧头+结束代码+响应数据
        //正常结束响应报文（无响应数据）：帧头+副帧头+结束代码+响应数据
        //异常结束响应报文：帧头+副帧头+结束代码+异常代码
        public Mc1EClient(EtherNetParameter parameter, ConnectMode mode)
            : base(parameter, mode)
        {
            if (mode == ConnectMode.SerialPort)
                throw new ArgumentException(
                    "Serial port connection is not supported",
                    nameof(mode)
                );
        }

        public Mc1EClient(string ip, ushort port, ConnectMode mode)
            : this(EtherNetParameter.Create(ip, port), mode) { }

        private byte[] CreateCommand(
            Command1E cmd,
            PlcMemory1E plcMemory1E,
            uint address,
            ushort count,
            byte[]? writeData
        )
        {
            byte[] buffer = writeData is not null ? new byte[12 + writeData.Length] : new byte[12];
            //固定头部
            buffer[0] = (byte)cmd; //副头部 对应为功能码
            buffer[1] = PlcNumber; //可编程控制器编号
            buffer[2] = (byte)CpuTimer; //CPU监视定时器
            buffer[3] = (byte)(CpuTimer >> 8);

            //软元件+数据部分
            buffer[4] = (byte)address;
            buffer[5] = (byte)(address >> 8);
            buffer[6] = (byte)(address >> 16);
            buffer[7] = (byte)(address >> 24);

            //软元件代码
            ushort memory = (ushort)plcMemory1E;
            buffer[8] = (byte)memory;
            buffer[9] = (byte)(memory >> 8);

            //软元件点数
            buffer[10] = (byte)count;
            buffer[11] = (byte)(count >> 8);

            //负载数据
            if (writeData is not null)
                Array.Copy(writeData, 0, buffer, 12, writeData.Length);
            return buffer;
        }

        private void ValidateDataType<T>(T t, uint start, PlcMemory1E memory)
            where T : struct
        {
            switch (memory)
            {
                case PlcMemory1E.X:
                case PlcMemory1E.Y:
                case PlcMemory1E.M:
                case PlcMemory1E.S:
                case PlcMemory1E.TS:
                case PlcMemory1E.CS:
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
                case PlcMemory1E.TN:
                    if (t is not short && t is not ushort)
                        throw new ArgumentException(
                            "Data type not supported",
                            t.GetType().ToString()
                        );
                    break;
                case PlcMemory1E.CN:
                    if (t is not short && t is not ushort && t is not int && t is not uint)
                        throw new ArgumentException(
                            "Data type not supported",
                            t.GetType().ToString()
                        );
                    break;
                case PlcMemory1E.D:
                    break;
                case PlcMemory1E.R:
                    break;
                default:
                    break;
            }
        }

        private Result<byte[]> ReadBytes(Command1E cmd, PlcMemory1E plcMemory1E, uint address, ushort count)
        {
            return EnqueueExecute(() =>
            {
                var sendData = CreateCommand(cmd, plcMemory1E, address, count, default);
                var result = NoLockExecute(sendData);
                var payload = result.ReceivedData.Slice(2);
                return result.ToResult(payload);
            });
        }

        public Result ReadBytes(string address, ushort count)
        {
            var addr = new McAddress1E(address);
            return ReadBytes(Command1E.BatchReadWord, addr.Memory, addr.StartAddress, count);
        }

        public Result<byte[]> ReadBytes(McAddress1E address, ushort count)
        {
            return ReadBytes(Command1E.BatchReadWord, address.Memory, address.StartAddress, count);
        }

        public Result<T[]> ReadValues<T>(string address, byte count)
            where T : struct
        {
            var addr = new McAddress1E(address);
            return ReadValues<T>(addr, count);
        }

        public Result<T[]> ReadValues<T>(McAddress1E address, byte count) where T : struct
        {
            T t = default;
            ValidateDataType(t, address.StartAddress, address.Memory);
            switch (t)
            {
                case bool:
                    {
                        uint start = address.StartAddress % 16 > 0
                            ? address.StartAddress - (address.StartAddress % 16)
                            : address.StartAddress;

                        byte _count = (address.StartAddress + count - start) % 16 > 0
                            ? (byte)((address.StartAddress + count - start) / 16 + 1)
                            : (byte)((address.StartAddress + count - start) / 16);

                        var addr = new McAddress1E(address.Memory, start);
                        var result = ReadBytes(addr, _count);
                        var bs = result.Value.ToBooleansFromWord(IsLittleEndian)
                            .Slice((int)(address.StartAddress - start), count);
                        if (bs is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case byte:
                    {
                        var result = ReadBytes(address, count);
                        if (result.Value is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case short:
                    {
                        var result = ReadBytes(address, count);
                        var values = result.Value.ToInt16Array(IsLittleEndian);
                        if (values is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case ushort:
                    {
                        var result = ReadBytes(address, count);
                        var values = result.Value.ToUInt16Array(IsLittleEndian);
                        if (values is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case int:
                    {
                        const int byteSize = 2;
                        var result = ReadBytes(address, (byte)(count * byteSize));
                        var values = result.Value.ToInt32Array(IsLittleEndian);
                        if (values is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case uint:
                    {
                        const int byteSize = 2;
                        var result = ReadBytes(address, (byte)(count * byteSize));
                        var values = result.Value.ToUInt32Array(IsLittleEndian);
                        if (values is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case float:
                    {
                        const int byteSize = 2;
                        var result = ReadBytes(address, (byte)(count * byteSize));
                        var values = result.Value.ToFloatArray(IsLittleEndian);
                        if (values is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case long:
                    {
                        const int byteSize = 4;
                        var result = ReadBytes(address, (byte)(count * byteSize));
                        var values = result.Value.ToInt64Array(IsLittleEndian);
                        if (values is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case ulong:
                    {
                        const int byteSize = 4;
                        var result = ReadBytes(address, (byte)(count * byteSize));
                        var values = result.Value.ToUInt64Array(IsLittleEndian);
                        if (values is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                case double:
                    {
                        const int byteSize = 4;
                        var result = ReadBytes(address, (byte)(count * byteSize));
                        var values = result.Value.ToDoubleArray(IsLittleEndian);
                        if (values is T[] val)
                            return result.ToResult(val);
                        break;
                    }
                default:
                    break;
            }
            throw new ArgumentException("Type not supported", nameof(T));
        }

        public Result<T> ReadValue<T>(string address) where T : struct
        {
            var addr = new McAddress1E(address);
            return ReadValue<T>(addr);
        }

        public Result<T> ReadValue<T>(McAddress1E address) where T : struct
        {
            var result = ReadValues<T>(address, 1);
            var value = result.Value.FirstOrDefault();
            return result.ToResult(value);
        }

        private Result WriteBytes(
            Command1E cmd,
            PlcMemory1E plcMemory1E,
            uint address,
            ushort count,
            byte[] writeData
        )
        {
            return EnqueueExecute(() =>
            {
                var sendData = CreateCommand(cmd, plcMemory1E, address, count, writeData);
                return NoLockExecute(sendData);
            });
        }

        public Result WriteBytes(string address, byte[] writeData)
        {
            var addr = new McAddress1E(address);
            return WriteBytes(
                Command1E.BatchWriteWord,
                addr.Memory,
                addr.StartAddress,
                (ushort)writeData.Length,
                writeData
            );
        }

        public Result WriteBytes(McAddress1E address, byte[] writeData)
        {
            return WriteBytes(
                Command1E.BatchWriteWord,
                address.Memory,
                address.StartAddress,
                (ushort)writeData.Length,
                writeData
            );
        }

        public Result WriteValues<T>(string address, T[] value)
            where T : struct
        {
            var addr = new McAddress1E(address);
            return WriteValues(addr, value);
        }

        public Result WriteValues<T>(McAddress1E address, T[] writeData)
            where T : struct
        {
            T t = default;
            ValidateDataType(t, address.StartAddress, address.Memory);

            switch (writeData)
            {
                case bool[] bs:
                    {
                        //写入单位为字，比较危险，不支持连续位写入
                        if (address.Memory == PlcMemory1E.D || address.Memory == PlcMemory1E.R)
                            throw new Exception(
                                $"The `{address.Memory}` address does not support write bits"
                            );

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
                            Command1E.BatchWriteBit,
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

        public Result WriteValue<T>(McAddress1E address, T value)
            where T : struct
        {
            return WriteValues(address, new T[] { value });
        }

        protected override bool ValidateReceivedData(ReadOnlySpan<byte> sendData, ReadOnlySpan<byte> writeData)
        {
            if (writeData.Length == 4 && writeData[0] == sendData[0] + 0x80 && writeData[1] != OkEndCode)
            {
                //错误结束
                var errorCode = writeData.Slice(2, 2).ToArray().ToHexString();
                var errorData = writeData.Slice(3).ToArray().ToHexString();
                throw new ReceivedException($"Response data error, error code:{errorCode},error data:{errorData}", sendData.ToArray(), writeData.ToArray(), DriverId);
            }
            else if (writeData.Length == writeData.Length && writeData[0] == sendData[0] + 0x80 && writeData[1] == OkEndCode)
            {
                //正确数据返回
                return true;
            }
            return false;
        }
    }
}
