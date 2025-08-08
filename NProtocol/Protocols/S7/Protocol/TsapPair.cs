using System;
using NProtocol.Protocols.S7.Enums;

namespace NProtocol.Protocols.S7
{
    /// <summary>
    /// TSAP部分
    /// </summary>
    public readonly struct TsapPair
    {
        public TsapPair(Tsap local, Tsap remote)
        {
            Local = local;
            Remote = remote;
        }

        /// <summary>
        /// 本机
        /// </summary>
        public Tsap Local { get; }

        /// <summary>
        /// 远程
        /// </summary>
        public Tsap Remote { get; }

        public static TsapPair GetDefaultTsapPair(CpuType cpuType, byte rack, byte slot)
        {
            if (rack > 0x0F)
                throw new ArgumentOutOfRangeException(nameof(rack), "maximum rack < 16");
            if (slot > 0x0F)
                throw new ArgumentOutOfRangeException(nameof(slot), "maximum slot < 16");
            switch (cpuType)
            {
                case CpuType.S7200:
                    return new TsapPair(new Tsap(0x10, 0x00), new Tsap(0x10, 0x01));
                case CpuType.Logo0BA8:
                    return new TsapPair(new Tsap(0x01, 0x00), new Tsap(0x01, 0x02));
                case CpuType.S7200Smart:
                case CpuType.S71200:
                case CpuType.S71500:
                case CpuType.S7300:
                case CpuType.S7400:
                    return new TsapPair(
                        new Tsap(0x01, 0x00),
                        new Tsap(0x03, (byte)(rack << 5 | slot))
                    );
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(cpuType),
                        "Invalid CPU Type specified"
                    );
            }
        }
    }
}
