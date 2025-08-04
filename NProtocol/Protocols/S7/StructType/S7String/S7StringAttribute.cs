using System;

namespace NProtocol.Protocols.S7.StructType
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class S7StringAttribute : Attribute
    {
        public S7StringAttribute(S7StringType type, byte reservedLength)
        {
            Type = type;
            ReservedLength = reservedLength;
        }
        public S7StringType Type { get; }
        public byte ReservedLength { get; }
        public byte ReservedLengthInBytes => Type == S7StringType.S7String
            ? (byte)(ReservedLength + 2)
            : (byte)(ReservedLength * 2 + 4);
    }
}
