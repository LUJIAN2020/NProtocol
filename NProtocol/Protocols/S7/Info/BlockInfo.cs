using NProtocol.Extensions;
using NProtocol.Protocols.S7.Enums;
using System;

namespace NProtocol.Protocols.S7.Info
{
    public class BlockInfo
    {
        public BlockInfo(byte[] buffer, bool isLittleEndian = true)
        {
            if (buffer.Length != 42) throw new ArgumentOutOfRangeException(nameof(buffer.Length), buffer.Length, "buffer length must 42.");
            BlockType = buffer.Slice(0, 2).ToUInt16(isLittleEndian);
            LengthOfInfo = buffer.Slice(2, 2).ToUInt16(isLittleEndian);
            UnknownBlockinfo2 = buffer.Slice(4, 2).ToUInt16(isLittleEndian);
            Constant3 = buffer.Slice(6, 2).ToUInt16(isLittleEndian);
            UnknownBlockinfo = buffer[8];
            BlockFlags = buffer[9];
            BlockLanguage = buffer[10];
            Type = (SubBlockType)buffer[11];
            BlockNumber = buffer.Slice(12, 2).ToUInt16(isLittleEndian);
            LengthLoadMemory = buffer.Slice(14, 4).ToInt32(isLittleEndian);
            BlockSecurity = buffer.Slice(18, 4);
            //不知道怎么计算的
            //byte[] left = new byte[2];
            //long t1 = left.Combine(buffer.ArrayCut(22, 27)).ToInt64(false);
            //long t2 = left.Combine(buffer.ArrayCut(28, 33)).ToInt64(false);
            //CodeTimestamp = new DateTime(t1);
            //IntferfaceTimestamp = new DateTime(t2);
            SsbLength = buffer.Slice(34, 2).ToUInt16(isLittleEndian);
            AddLength = buffer.Slice(36, 2).ToUInt16(isLittleEndian);
            LocalDataLength = buffer.Slice(38, 2).ToUInt16(isLittleEndian);
            MC7CodeLength = buffer.Slice(40, 2).ToUInt16(isLittleEndian);
        }
        public ushort BlockType { get; private set; }
        public ushort LengthOfInfo { get; private set; }
        public ushort UnknownBlockinfo2 { get; private set; }
        public ushort Constant3 { get; private set; }
        public byte UnknownBlockinfo { get; private set; }
        public byte BlockFlags { get; private set; }
        public byte BlockLanguage { get; private set; }
        public SubBlockType Type { get; private set; }
        public ushort BlockNumber { get; private set; }
        public int LengthLoadMemory { get; private set; }
        public byte[] BlockSecurity { get; private set; }
        public DateTime CodeTimestamp { get; private set; }
        public DateTime IntferfaceTimestamp { get; private set; }
        public ushort SsbLength { get; private set; }
        public ushort AddLength { get; private set; }
        public ushort LocalDataLength { get; private set; }
        public ushort MC7CodeLength { get; private set; }

    }
}
