using NProtocol.Extensions;
using NProtocol.Protocols.S7.Enums;
using System;

namespace NProtocol.Protocols.S7.Info
{
    public class BlockObject
    {
        public BlockObject(BlockType blockType, byte[] buffer, bool isLittleEndian = true)
        {
            if (buffer.Length != 4) throw new ArgumentOutOfRangeException(nameof(buffer.Length), buffer.Length, "buffer length must 4.");
            BlockNumber = buffer.Slice(0, 2).ToUInt16(isLittleEndian);
            BlockFlags = buffer[2];
            BlockLanguage = buffer[3];
            BlockType = blockType;
        }
        public BlockType BlockType { get; private set; }
        public ushort BlockNumber { get; private set; }
        public byte BlockFlags { get; private set; }
        public byte BlockLanguage { get; private set; }
    }
}
