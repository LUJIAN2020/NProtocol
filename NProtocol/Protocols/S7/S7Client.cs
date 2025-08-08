using System;
using NProtocol.Base;
using NProtocol.Connectors;
using NProtocol.Enums;
using NProtocol.Exceptions;
using NProtocol.Protocols.S7.Enums;

namespace NProtocol.Protocols.S7
{
    public partial class S7Client : DriverBase
    {
        /// <summary>
        /// PDU尺寸
        /// </summary>
        public const ushort PduSize = 960;

        /// <summary>
        /// 确定项目结构的主要类型，通常为0x12，代表变量规范
        /// </summary>
        public const byte VariableSpecification = 0x12;

        /// <summary>
        /// 冗余数据，通常为0x0000
        /// </summary>
        public const ushort RedundancyIdentification = 0;

        /// <summary>
        /// 默认TPDU长度，(2^10 = 1024)
        /// </summary>
        public const byte DefaultTpduSize = 0x0A;

        /// <summary>
        /// 协议Id，固定值
        /// </summary>
        public const byte ProtocolId = 0x32;

        /// <summary>
        /// 版本号
        /// </summary>
        public const byte Version = 0x03;

        /// <summary>
        /// 保留
        /// </summary>
        public const byte Reserved = 0x00;

        /// <summary>
        /// 目标引用 唯一标识
        /// </summary>
        public ushort DestinationReference { get; private set; }

        /// <summary>
        /// 源引用 唯一标识
        /// </summary>
        public ushort SourceReference { get; private set; }

        /// <summary>
        /// 协议数据单元参考，通过请求事件增加；
        /// </summary>
        public ushort ProtocolDataUnitReference { get; private set; }
        public byte Rack { get; set; }
        public byte Slot { get; set; }
        public CpuType CpuType { get; set; }
        public TsapPair TsapPair { get; set; }
        public bool IsLittleEndian { get; } = true;

        protected S7Client(EtherNetParameter parameter)
            : base(parameter, ConnectMode.Tcp) { }

        public S7Client(
            string ip,
            ushort port = 102,
            CpuType cpuType = CpuType.S71500,
            byte rack = 0,
            byte slot = 1
        )
            : this(EtherNetParameter.Create(ip, port), cpuType, rack, slot) { }

        public S7Client(EtherNetParameter parameter, CpuType cpuType, byte rack = 0, byte slot = 1)
            : this(parameter, TsapPair.GetDefaultTsapPair(cpuType, rack, slot))
        {
            CpuType = cpuType;
            Rack = rack;
            Slot = slot;
        }

        public S7Client(EtherNetParameter parameter, TsapPair tsapPair)
            : this(parameter)
        {
            TsapPair = tsapPair;
        }

        protected override byte[]? ExtractPayload(byte[] writeData, byte[] readData)
        {
            if (ValidateReceivedData(writeData, readData))
            {
                ProtocolDataUnitReference++;
                return readData;
            }
            return default;
        }

        private bool ValidateReceivedData(byte[] sendData, byte[] receiveData)
        {
            if (receiveData.Length < 4)
                return false;
            if (receiveData[0] != sendData[0] && receiveData[1] != sendData[1])
                return false;
            if (receiveData.Length != receiveData[2] * 256 + receiveData[3])
                return false;
            var pduType = (CotpPduType)receiveData[5];
            switch (pduType)
            {
                case CotpPduType.ExpeditedData:
                    break;
                case CotpPduType.ExpeditedDataAcknowledgement:
                    break;
                case CotpPduType.UserData:
                    break;
                case CotpPduType.Reject:
                    break;
                case CotpPduType.DataAcknowledgement:
                    break;
                case CotpPduType.TPDUError:
                    break;
                case CotpPduType.DisconnectRequest:
                    break;
                case CotpPduType.DisconnectConfirm:
                    break;
                case CotpPduType.ConnectConfirm: //连接请求
                {
                    if (
                        receiveData.Length - 5 == receiveData[4]
                        && receiveData[10] == 0x00
                        && receiveData[11] == 0xC0
                        && receiveData[12] == 0x01
                        && receiveData[13] == 0x0A
                        && receiveData[14] == 0xC1
                        && receiveData[15] == 0x02
                        && receiveData[16] == TsapPair.Local.FirstByte
                        && receiveData[17] == TsapPair.Local.SecondByte
                        && receiveData[18] == 0xC2
                        && receiveData[19] == 0x02
                        && receiveData[20] == TsapPair.Remote.FirstByte
                        && receiveData[21] == TsapPair.Remote.SecondByte
                    )
                    {
                        return true;
                    }
                    break;
                }
                case CotpPduType.ConnectRequest:
                    break;
                case CotpPduType.Data: //数据传输
                {
                    var s7CommPduType = (S7CommPduType)receiveData[8];
                    switch (s7CommPduType)
                    {
                        case S7CommPduType.AckData:
                        case S7CommPduType.Job:
                        case S7CommPduType.Ack:
                        {
                            if (
                                receiveData[4] == 2
                                && receiveData[6] == 0x80
                                && receiveData[7] == ProtocolId
                            )
                            {
                                if (receiveData[17] != 0 || receiveData[18] != 0)
                                {
                                    S7ErrorClass s7ErrorClass = (S7ErrorClass)receiveData[17];
                                    S7ErrorCode s7ErrorCode = (S7ErrorCode)receiveData[18];
                                    throw new ReceivedException(
                                        $"The header message is incorrect，S7ErrorClass：{s7ErrorClass},S7ErrorCode：{s7ErrorCode}",
                                        sendData,
                                        receiveData,
                                        DriverId
                                    );
                                }

                                //ReturnCode
                                var func = (S7CommFuncCode)receiveData[19];
                                if (
                                    func == S7CommFuncCode.WriteVar
                                    || func == S7CommFuncCode.ReadVar
                                )
                                {
                                    var rc = (S7CommReturnCode)receiveData[21];
                                    ValidateReturnCode(rc);
                                }

                                return true;
                            }
                            break;
                        }
                        case S7CommPduType.UserData:
                        {
                            if (
                                receiveData[4] == 2
                                && receiveData[6] == 0x80
                                && receiveData[7] == ProtocolId
                            )
                            {
                                if ((receiveData[15] * 256) + receiveData[16] > 0)
                                {
                                    S7ErrorClass s7ErrorClass = (S7ErrorClass)receiveData[17];
                                    S7ErrorCode s7ErrorCode = (S7ErrorCode)receiveData[18];
                                    if (receiveData[27] != 0 || receiveData[28] != 0)
                                    {
                                        throw new ReceivedException(
                                            $"The header message is incorrect，S7ErrorClass：{s7ErrorClass},S7ErrorCode：{s7ErrorCode}",
                                            sendData,
                                            receiveData,
                                            DriverId
                                        );
                                    }
                                }
                                return true;
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            return false;
        }

        private int GetReceivePaloadDataLength(byte transportSize, int len)
        {
            switch (transportSize)
            {
                case 3:
                case 4:
                case 5:
                    return len / 8; //返回的数据是按位组织的
                case 6:
                case 7:
                case 8:
                default:
                    return len; //返回的数据是按字节组织
            }
        }

        private void ValidateReturnCode(S7CommReturnCode statusCode)
        {
            switch (statusCode)
            {
                case S7CommReturnCode.ObjectDoesNotExist:
                    throw new Exception("Received error from PLC: Object does not exist.");
                case S7CommReturnCode.DataTypeInconsistent:
                    throw new Exception("Received error from PLC: Data type inconsistent.");
                case S7CommReturnCode.DataTypeNotSupported:
                    throw new Exception("Received error from PLC: Data type not supported.");
                case S7CommReturnCode.AccessingTheObjectNotAllowed:
                    throw new Exception("Received error from PLC: Accessing object not allowed.");
                case S7CommReturnCode.AddressOutOfRange:
                    throw new Exception("Received error from PLC: Address out of range.");
                case S7CommReturnCode.HardwareFault:
                    throw new Exception("Received error from PLC: Hardware fault.");
                case S7CommReturnCode.Success:
                    break;
                default:
                    throw new Exception(
                        $"Invalid response from PLC: statusCode={(byte)statusCode}."
                    );
            }
        }
    }
}
