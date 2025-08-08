using System;

namespace NProtocol.Protocols.S7.StructType
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class S7TimeAttribute : Attribute
    {
        public S7TimeAttribute(S7TimeType type)
        {
            Type = type;
        }

        public S7TimeType Type { get; }
        public byte Size =>
            Type switch
            {
                S7TimeType.Time => 4,
                S7TimeType.LTime => 8,
                S7TimeType.TimeOfDay => 4,
                S7TimeType.LTimeOfDay => 8,
                _ => throw new ArgumentException("Not supported type.", nameof(Type)),
            };
    }
}
