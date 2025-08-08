using System;

namespace NProtocol.Protocols.S7.StructType
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class S7DateTimeAttribute : Attribute
    {
        public S7DateTimeAttribute(S7DateTimeType type)
        {
            Type = type;
        }

        public S7DateTimeType Type { get; }
        public byte Size =>
            Type switch
            {
                S7DateTimeType.Date => 2,
                S7DateTimeType.DateAndTime => 8,
                S7DateTimeType.DTL => 12,
                _ => throw new ArgumentException("Not supported type.", nameof(Type)),
            };
    }
}
