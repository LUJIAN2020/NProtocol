using NProtocol.Communication.Extensions;
using Robot.Communication.Extensions;

namespace NProtocol.Protocols.S7.CpuInfo
{
    /// <summary>
    /// 通讯能力
    /// </summary>
    public class CpuCommunicationCapabilityInfo
    {
        public CpuCommunicationCapabilityInfo(byte[] buffer, bool isLittleEndian = true)
        {
            int startIndex = 2;
            MaximumPduSize = buffer.Slice(startIndex, 2).ToUInt16(isLittleEndian);
            MaximumActiveConnections = buffer.Slice(startIndex += 2, 2).ToUInt16(isLittleEndian);
            MaximumMpiRate = buffer.Slice(startIndex += 2, 4).ToUInt32(isLittleEndian);
            MaximumCommunicationBusRate = buffer.Slice(startIndex += 4, 4).ToUInt32(isLittleEndian);
            Reserved = buffer.Slice(startIndex += 4);
        }
        /// <summary>
        /// 最大Pdu长度 单位：byte
        /// </summary>
        public ushort MaximumPduSize { get; private set; }
        /// <summary>
        /// 最大连接数
        /// </summary>
        public ushort MaximumActiveConnections { get; private set; }
        /// <summary>
        /// 最大MPI传输速率 单位：bps
        /// </summary>
        public uint MaximumMpiRate { get; private set; }
        /// <summary>
        /// 最大通讯总线速率 单位：bps
        /// </summary>
        public uint MaximumCommunicationBusRate { get; private set; }
        /// <summary>
        /// 保留
        /// </summary>
        public byte[] Reserved { get; set; }
    }
}
