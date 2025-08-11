using System;
using System.Collections.Generic;
using System.Linq;
using NProtocol.Base;
using NProtocol.Extensions;
using NProtocol.Protocols.S7.Enums;

namespace NProtocol.Protocols.S7
{
    public partial class S7Client
    {
        /// <summary>
        /// Get multiple read data packets
        /// </summary>
        /// <param name="items"></param>
        /// <param name="isRead"></param>
        /// <returns></returns>
        private byte[] GetMultipleVarsPacket(IEnumerable<MultipleItem> items, bool isRead = true)
        {
            var tpkt = CreateTpktPacket();
            var cotp = CreateCotpFuctionPacket(CotpPduType.Data);
            var s7Comm = Array.Empty<byte>();
            if (isRead)
                s7Comm = CreateMultipleS7CommPacket(
                    S7CommPduType.Job,
                    S7CommFuncCode.ReadVar,
                    items
                );
            else
                s7Comm = CreateMultipleS7CommPacket(
                    S7CommPduType.Job,
                    S7CommFuncCode.WriteVar,
                    items
                );
            byte[] len = ((ushort)(tpkt.Length + cotp.Length + s7Comm.Length)).ToBytes();
            tpkt[2] = len[1];
            tpkt[3] = len[0];
            return tpkt.Combine(cotp, s7Comm);
        }

        /// <summary>
        /// Create multiple S7comm data packets
        /// </summary>
        /// <param name="s7CommPduType">pdu type</param>
        /// <param name="s7CommFuncCode"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        private byte[] CreateMultipleS7CommPacket(
            S7CommPduType s7CommPduType,
            S7CommFuncCode s7CommFuncCode,
            IEnumerable<MultipleItem> items
        )
        {
            var header = CreateS7CommHeaderPacket(s7CommPduType);
            var parameter = CreateMultipleS7CommParameterPacket(s7CommFuncCode, items);
            var plens = ((ushort)parameter.Length).ToBytes();
            header[6] = plens[1];
            header[7] = plens[0];
            if (items.Any())
            {
                var data = CreateMultipleS7CommDataPacket(items);
                var dlens = ((ushort)data.Length).ToBytes();
                header[8] = dlens[1];
                header[9] = dlens[0];
                return header.Combine(parameter, data);
            }
            else
            {
                return header.Combine(parameter);
            }
        }

        /// <summary>
        /// Create multiple S7comm parameter data packets
        /// </summary>
        /// <param name="s7CommFuncCode"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        private byte[] CreateMultipleS7CommParameterPacket(
            S7CommFuncCode s7CommFuncCode,
            IEnumerable<MultipleItem> items
        )
        {
            if (
                s7CommFuncCode != S7CommFuncCode.ReadVar
                && s7CommFuncCode != S7CommFuncCode.WriteVar
            )
                throw new NotSupportedException($"Function code is temporarily not supported, {s7CommFuncCode}.");
            int itemCount = items.Count();
            var buffer = new byte[12 * itemCount + 2];
            buffer[0] = (byte)s7CommFuncCode; //作业请求（Job）和确认数据响应（Ack_Data）
            buffer[1] = (byte)itemCount;
            int no = 0;
            foreach (var item in items)
            {
                S7VarType varType = S7VarType.Byte;
                ushort db = 0;
                if (item.DbNumber is not null)
                {
                    db = (ushort)item.DbNumber;
                }
                else
                {
                    if (item.MemoryAreaType == S7MemoryAreaType.S7Timer)
                    {
                        varType = S7VarType.Timer;
                    }
                    else if (item.MemoryAreaType == S7MemoryAreaType.S7Counter)
                    {
                        varType = S7VarType.Counter;
                    }
                }
                var itemPacket = CreateParameterItemPacket(
                    varType,
                    item.MemoryAreaType,
                    db,
                    item.StartAddress,
                    item.BitAddress,
                    item.Count
                );
                Buffer.BlockCopy(itemPacket, 0, buffer, 2 + (12 * no), itemPacket.Length);
                no++;
            }
            return buffer;
        }

        /// <summary>
        /// Create multiple S7comm payload data packets
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private byte[] CreateMultipleS7CommDataPacket(IEnumerable<MultipleItem> items)
        {
            int sum = items.Sum(m => m.WriteData != null ? m.WriteData.Length + 5 : 0);
            byte[] buffer = new byte[sum];
            int offset = 0;
            foreach (var item in items)
            {
                S7VarType varType = S7VarType.Byte;
                if (item.MemoryAreaType == S7MemoryAreaType.S7Timer)
                {
                    varType = S7VarType.Timer;
                }
                else if (item.MemoryAreaType == S7MemoryAreaType.S7Counter)
                {
                    varType = S7VarType.Counter;
                }
                var data = CreateS7CommDataPacket(varType, item.WriteData);
                Buffer.BlockCopy(data, 0, buffer, offset, data.Length);
                offset += data.Length;
            }
            return buffer;
        }

        /// <summary>
        /// Multiple consecutive address read
        /// </summary>
        /// <param name="items">Address collection, key = address, value = read length</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<IEnumerable<MultipleItem>> ReadMultipleVars(IEnumerable<MultipleItem> items)
        {
            return EnqueueExecute(() =>
            {
                ValidateWriteMultipleItems(items);
                var sendData = GetMultipleVarsPacket(items);
                var result = NoLockExecute(sendData);
                var receivedData = result.ReceivedData;
                byte itemCount = receivedData[20];
                var payload = receivedData.Slice(21);
                int offset = 0;
                foreach (var item in items)
                {
                    item.ReturnCode = (S7CommReturnCode)payload[offset];
                    if (payload[offset] == (byte)S7CommReturnCode.Success)
                    {
                        var tst = (S7CommTransportSizeType)payload[offset + 1];
                        int len = payload[offset + 2] * 256 + payload[offset + 3];
                        int size = GetTransportSizeLength(tst);
                        len /= size;
                        offset += 4;
                        item.ReadValue = payload.Slice(offset, len);
                        int residue = len % 2;
                        if (residue > 0)
                        {
                            //长度如果为奇数，最后一个字节fill byte 0x00
                            offset += len;
                            offset += 1;
                        }
                        else
                        {
                            offset += len;
                        }
                    }
                    else
                    {
                        offset += 4;
                    }
                }
                return result.ToResult(items);
            });
        }

        /// <summary>
        /// Multiple consecutive address write
        /// </summary>
        /// <param name="items">Parameters</param>
        /// <returns>Returns multiple defined parameters</returns>
        /// <exception cref="LpException"></exception>
        /// <exception cref="ArgumentNullException">Parameters cannot be null</exception>
        public Result<IEnumerable<MultipleItem>> WriteMultipleVars(IEnumerable<MultipleItem> items)
        {
            return EnqueueExecute(() =>
            {
                ValidateWriteMultipleItems(items, true);
                var sendData = GetMultipleVarsPacket(items, false);
                var result = NoLockExecute(sendData);
                var receivedData = result.ReceivedData;
                byte itemCount = receivedData[20];
                int no = 1;
                foreach (var parameter in items)
                {
                    parameter.ReturnCode = (S7CommReturnCode)receivedData[20 + no];
                    no++;
                }
                return result.ToResult(items);
            });
        }

        /// <summary>
        /// Validate write data parameter items
        /// </summary>
        /// <param name="items"></param>
        /// <param name="isWrite"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        private void ValidateWriteMultipleItems(
            IEnumerable<MultipleItem> items,
            bool isWrite = false
        )
        {
            EnqueueExecute(() =>
            {
                if (items.Count() > 20)
                    throw new ArgumentOutOfRangeException(
                        nameof(items),
                        items.Count(),
                        "The number of parameters cannot exceed 20"
                    );
                if (isWrite)
                {
                    foreach (var item in items)
                    {
                        if (item.WriteData is null)
                            throw new ArgumentNullException(nameof(item.WriteData));
                        switch (item.MemoryAreaType)
                        {
                            case S7MemoryAreaType.DataBlock:
                            case S7MemoryAreaType.Output:
                            case S7MemoryAreaType.Memory:
                            case S7MemoryAreaType.Input:
                                if (item.WriteData.Length < 1)
                                    throw new ArgumentOutOfRangeException(
                                        nameof(item.WriteData.Length),
                                        item.WriteData.Length,
                                        "Data length must be equal to 1"
                                    );
                                break;
                            case S7MemoryAreaType.S7Counter:
                            case S7MemoryAreaType.S7Timer:
                                if (item.WriteData.Length < 2)
                                    throw new ArgumentOutOfRangeException(
                                        nameof(item.WriteData.Length),
                                        item.WriteData.Length,
                                        "Data length must be equal to 2"
                                    );
                                break;
                            default:
                                break;
                        }
                        if (item.WriteData.Length % 2 > 0)
                            throw new ArgumentOutOfRangeException(
                                nameof(item.WriteData.Length),
                                item.WriteData.Length,
                                "The data length to be written must be even"
                            );
                    }
                }
            });
        }
    }
}
