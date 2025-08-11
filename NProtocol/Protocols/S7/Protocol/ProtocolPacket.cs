using NProtocol.Extensions;
using NProtocol.Protocols.S7.Enums;
using NProtocol.Protocols.S7.Enums.UserData;
using System;

namespace NProtocol.Protocols.S7
{
    /// <summary>
    /// S7 Protocol Packet Composition
    /// </summary>
    public partial class S7Client
    {
        //complete data packet：TPKT+COTP+S7Comm(Header+Parameter+Data)

        /// <summary>
        /// Creates a TPKT packet, 4 bytes
        /// </summary>
        /// <returns></returns>
        private byte[] CreateTpktPacket()
        {
            //TPKT 4字节
            return
            [
                Version, //版本号 固定报文头0x03
                Reserved, //预留 固定报文头0x00
                0x00,
                0x00, //报文长度,连接所有报文后再进行更改长度
            ];
        }

        /// <summary>
        /// Creates a COTP connection packet, 18 bytes
        /// </summary>
        /// <returns></returns>
        private byte[] CreateCotpConnectionPacket(CotpPduType pduType)
        {
            return
            [
                0x11, //长度 COTP Header Length
                (byte)pduType, //PDU类型 Connect Request
                (byte)(DestinationReference >> 8),
                (byte)DestinationReference, //目标引用 Destination Reference
                (byte)(SourceReference >> 8),
                (byte)SourceReference, //源引用 Source Reference
                0, //前4bit扩展格式=false / 后4bit流控制=false Option 默认为0
                0xC0, //Parameter Code (tpdu-size)
                0x01, //Parameter Length
                DefaultTpduSize, //TPDU Size (2^10 = 1024) 协商TPDU长度
                0xC1, //参数代码 Parameter Code (src-tasp)
                0x02, //参数长度 Parameter Length
                TsapPair.Local.FirstByte,
                TsapPair.Local.SecondByte, //Source TSAP
                0xC2, //Parameter Code (dst-tasp)
                0x02, //Parameter Length
                TsapPair.Remote.FirstByte,
                TsapPair.Remote.SecondByte, //Destination TSAP
            ];
        }

        /// <summary>
        /// Creates a COTP function packet, 3 bytes
        /// </summary>
        /// <returns></returns>
        private byte[] CreateCotpFuctionPacket(CotpPduType pduType)
        {
            return
            [
                0x02, //长度
                (byte)pduType, //PDU类型
                0x80, //PDU类型 传输Data 前4bit扩展格式=true / 后4bit流控制=false 0b1000 0000
            ];
        }

        /// <summary>
        /// Creates an S7 protocol packet
        /// </summary>
        /// <returns></returns>
        private byte[] CreateS7CommPacket(
            S7CommPduType s7CommPduType,
            S7CommFuncCode s7CommFuncCode,
            S7MemoryAreaType areaType = S7MemoryAreaType.DataBlock,
            S7VarType varType = S7VarType.Bit,
            ushort db = 1,
            int byteAddress = 0,
            byte bitAddress = 0,
            ushort count = 1,
            byte[]? writeData = default
        )
        {
            var header = CreateS7CommHeaderPacket(s7CommPduType);
            var parameter = CreateS7CommParameterPacket(
                s7CommFuncCode,
                areaType,
                varType,
                db,
                byteAddress,
                bitAddress,
                count
            );
            var pLens = ((ushort)parameter.Length).ToBytes();
            header[6] = pLens[1];
            header[7] = pLens[0];
            if (writeData is not null)
            {
                var source = CreateS7CommDataPacket(varType, writeData);
                var dLens = ((ushort)source.Length).ToBytes();
                header[8] = dLens[1];
                header[9] = dLens[0];
                return header.Combine(parameter, source);
            }
            else
            {
                return header.Combine(parameter);
            }
        }

        /// <summary>
        /// Creates an S7Comm header packet, fixed length of 10 bytes
        /// </summary>
        /// <param name="s7CommPduType">S7 protocol PDU type</param>
        /// <returns></returns>
        private byte[] CreateS7CommHeaderPacket(S7CommPduType s7CommPduType)
        {
            return
            [
                ProtocolId, //协议ID，通常为0x32；
                (byte)s7CommPduType, //S7CommPduType
                RedundancyIdentification >> 8,
                (byte)RedundancyIdentification, //冗余数据，通常为0x0000；
                (byte)(ProtocolDataUnitReference >> 8),
                (byte)ProtocolDataUnitReference, //协议数据单元参考，通过请求事件增加；设定位0xFFFF
                0,
                8, //Parameter Length参数的总长度；
                0,
                0, //Data length，数据长度。如果读取PLC内部数据，此处为0x0000；对于其他功能，则为Data部分的数据长度；
            ];
        }

        /// <summary>
        /// Creates an S7Comm parameter packet, length is variable and mainly depends on the function code
        /// </summary>
        /// <param name="s7CommFuncCode"></param>
        /// <param name="areaType"></param>
        /// <param name="varType"></param>
        /// <param name="db"></param>
        /// <param name="byteAddress"></param>
        /// <param name="count"></param>
        /// <param name="userDataFunction"></param>
        /// <param name="subFunction"></param>
        /// <returns></returns>
        private byte[] CreateS7CommParameterPacket(
            S7CommFuncCode s7CommFuncCode,
            S7MemoryAreaType areaType = S7MemoryAreaType.DataBlock,
            S7VarType varType = S7VarType.Bit,
            ushort db = 1,
            int byteAddress = 0,
            byte bitAddress = 0,
            ushort count = 1,
            UserDataFunction userDataFunction = UserDataFunction.TimeFunctions,
            byte subFunction = 1
        )
        {
            byte[] buffer = Array.Empty<byte>();
            switch (s7CommFuncCode)
            {
                case S7CommFuncCode.CpuServices:
                    {
                        buffer = new byte[8];
                        buffer[0] = (byte)s7CommFuncCode; //作业请求（Job）和确认数据响应（Ack_Data）
                        buffer[1] = (byte)count;
                        buffer[2] = VariableSpecification;
                        buffer[3] = 0x04; //parameter长度
                        buffer[4] = 0x11; //request=0x11 response=0x12
                        buffer[5] = (byte)(0x40 + (byte)userDataFunction); //前4位 4=request 8=response 后4位 UserDataFunction
                        buffer[6] = subFunction; //子功能码，根据UserDataFunction改变而改变
                        buffer[7] = 0x00; //sequence number 顺序号 不懂什么意思
                        break;
                    }
                case S7CommFuncCode.SetupCommunication: //设置通讯，固定长度8字节
                    buffer = new byte[8];
                    buffer[0] = (byte)s7CommFuncCode; //作业请求（Job）和确认数据响应（Ack_Data）
                    buffer[1] = Reserved; //保留
                    buffer[2] = 0;
                    buffer[3] = 1; //Max AmQ (parallel jobs with ack) calling；
                    buffer[4] = 0;
                    buffer[5] = 1; //Max AmQ (parallel jobs with ack) called；
                    unchecked // CS0221
                    {
                        buffer[6] = (byte)(PduSize >> 8);
                        buffer[7] = (byte)PduSize; //协商PDU长度。 Use 960 PDU size
                    }
                    break;
                case S7CommFuncCode.ReadVar:
                case S7CommFuncCode.WriteVar:
                    {
                        buffer = new byte[12 + 2];
                        buffer[0] = (byte)s7CommFuncCode; //作业请求（Job）和确认数据响应（Ack_Data）
                        buffer[1] = 1;
                        var item = CreateParameterItemPacket(
                            varType,
                            areaType,
                            db,
                            byteAddress,
                            bitAddress,
                            count
                        );
                        Buffer.BlockCopy(item, 0, buffer, 2, item.Length);
                        break;
                    }
                case S7CommFuncCode.RequestDownload:
                    break;
                case S7CommFuncCode.DownloadBlock:
                    break;
                case S7CommFuncCode.DownloadEnded:
                    break;
                case S7CommFuncCode.StartUpload:
                    break;
                case S7CommFuncCode.Upload:
                    break;
                case S7CommFuncCode.EndUpload:
                    break;
                case S7CommFuncCode.PlcControl:
                    return new byte[] { 0x28 };
                case S7CommFuncCode.PlcStop:
                    return new byte[]
                    {
                        0x29, //stop function
                        0x00,
                        0x00,
                        0x00,
                        0x00,
                        0x00,
                        0x09,
                        0x50,
                        0x5F,
                        0x50,
                        0x52,
                        0x4F,
                        0x47,
                        0x52,
                        0x41,
                        0x4D,
                    };
                default:
                    break;
            }
            return buffer;
        }

        /// <summary>
        /// Creates the parameter for each item
        /// </summary>
        /// <param name="varType"></param>
        /// <param name="areaType"></param>
        /// <param name="db"></param>
        /// <param name="byteAddress"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private byte[] CreateParameterItemPacket(
            S7VarType varType,
            S7MemoryAreaType areaType,
            ushort db,
            int byteAddress,
            byte bitAddress,
            ushort count
        )
        {
            byte[] buffer = new byte[12];
            //Item
            buffer[0] = VariableSpecification; //ariable specification，确定项目结构的主要类型，通常为0x12，代表变量规范；
            buffer[1] = 0x0A; //Length of following address specification，本Item其余部分的长度；
            buffer[2] = 0x10; //Syntax Ids of variable specification，确定寻址模式和其余项目结构的格式；

            //Transport sizes in item data，确定变量的类型和长度：
            switch (areaType)
            {
                case S7MemoryAreaType.S7Timer:
                case S7MemoryAreaType.S7Counter:
                    buffer[3] = (byte)areaType;
                    break;
                default:
                    if (varType == S7VarType.Bit)
                    {
                        buffer[3] = 1;
                    }
                    else
                    {
                        buffer[3] = 2;
                    }
                    break;
            }

            //Request data length，请求的数据长度；
            buffer[4] = (byte)(count >> 8);
            buffer[5] = (byte)count;

            //DB number，DB模块的编号，如果访问的不是DB区域，此处为0x0000；
            if (areaType != S7MemoryAreaType.DataBlock)
            {
                buffer[6] = 0;
                buffer[7] = 0;
            }
            else
            {
                buffer[6] = (byte)(db >> 8);
                buffer[7] = (byte)db;
            }

            //Area，区域类型：
            buffer[8] = (byte)areaType;

            //Address，地址。
            //最大8191
            int overflow = byteAddress * 8 / 0xFFFF; // handles words with address bigger than 8191
            buffer[9] = (byte)overflow;
            switch (areaType)
            {
                case S7MemoryAreaType.S7Timer:
                case S7MemoryAreaType.S7Counter:
                    buffer[10] = (byte)((byteAddress) >> 8);
                    buffer[11] = (byte)byteAddress;
                    break;
                default:
                    if (varType == S7VarType.Bit)
                    {
                        int addr = byteAddress + bitAddress;
                        buffer[9] = (byte)(addr >> 16);
                        buffer[10] = (byte)(addr >> 8);
                        buffer[11] = (byte)addr;
                    }
                    else
                    {
                        buffer[10] = (byte)(((ushort)(byteAddress * 8)) >> 8);
                        buffer[11] = (byte)(byteAddress * 8);
                    }
                    break;
            }
            return buffer;
        }

        /// <summary>
        /// Creates the S7Comm payload data packet, typically for written data
        /// </summary>
        /// <returns></returns>
        private byte[] CreateS7CommDataPacket(S7VarType varType, byte[]? writeData)
        {
            if (writeData is null)
                return Array.Empty<byte>();
            int len = writeData.Length;
            var buffer = new byte[writeData.Length + 4];
            buffer[0] = Reserved; //Return code: Reserved (0x00)
            var tst = GetTransportSizeType(varType);
            buffer[1] = (byte)tst; //Transport size: BIT (0x03)
            len *= GetTransportSizeLength(tst);
            var lens = len.ToBytes();
            buffer[2] = lens[1];
            buffer[3] = lens[0]; //Length: 1
            Buffer.BlockCopy(writeData, 0, buffer, 4, writeData.Length);
            return buffer;
        }

        /// <summary>
        /// Gets the transport size type
        /// </summary>
        /// <param name="varType">Variable type</param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        private S7CommTransportSizeType GetTransportSizeType(S7VarType varType)
        {
            //Hex | 值 | 描述
            //| :----- | :----- | :----- |
            //| 0 | NULL |
            //| 3 | BIT | bit access, len is in bits
            //| 4 | BYTE / WORD / DWORD | byte / word / dword access, len is in bits
            //| 5 | INTEGER | integer access, len is in bits
            //| 6 | DINTEGER | integer access, len is in bytes
            //| 7 | REAL | real access, len is in bytes
            //| 9 | OCTET STRING | octet string, len is in bytes
            switch (varType)
            {
                case S7VarType.Bit:
                    return S7CommTransportSizeType.BIT;
                case S7VarType.Byte:
                case S7VarType.Word:
                case S7VarType.DWord:
                    return S7CommTransportSizeType.BYTE_WORD_DWORD;
                case S7VarType.Int:
                    return S7CommTransportSizeType.INTEGER;
                case S7VarType.DInt:
                    return S7CommTransportSizeType.DINTEGER;
                case S7VarType.LReal:
                    return S7CommTransportSizeType.REAL;
                case S7VarType.Counter:
                case S7VarType.Timer:
                    return S7CommTransportSizeType.OCTET_STRING;
                default:
                    return S7CommTransportSizeType.NULL;
            }
        }

        /// <summary>
        /// Gets the transport size based on the transport type
        /// </summary>
        /// <param name="type">Transport type</param>
        /// <returns></returns>
        private int GetTransportSizeLength(S7CommTransportSizeType type)
        {
            switch (type)
            {
                case S7CommTransportSizeType.NULL:
                    return 0;
                case S7CommTransportSizeType.BIT:
                    return 1;
                case S7CommTransportSizeType.BYTE_WORD_DWORD:
                case S7CommTransportSizeType.INTEGER:
                case S7CommTransportSizeType.DINTEGER:
                case S7CommTransportSizeType.REAL:
                    return 8;
                case S7CommTransportSizeType.OCTET_STRING:
                    return 1;
                default:
                    return 0;
            }
        }
    }
}
