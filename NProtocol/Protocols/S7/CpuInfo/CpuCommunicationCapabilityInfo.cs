using NProtocol.Extensions;

namespace NProtocol.Protocols.S7.CpuInfo
{
    /// <summary>
    /// Communication capabilities
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
        /// Maximum PDU length, in bytes
        /// </summary>
        public ushort MaximumPduSize { get; private set; }

        /// <summary>
        /// Maximum number of connections
        /// </summary>
        public ushort MaximumActiveConnections { get; private set; }

        /// <summary>
        /// Maximum MPI transmission rate, in bps
        /// </summary>
        public uint MaximumMpiRate { get; private set; }

        /// <summary>
        /// Maximum communication bus rate, in bps
        /// </summary>
        public uint MaximumCommunicationBusRate { get; private set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved { get; set; }
    }
}
