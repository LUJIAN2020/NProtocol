namespace NProtocol.Protocols.Modbus
{
    public enum ByteFormat : byte
    {
        /// <summary>
        /// Int16,UInt16
        /// </summary>
        AB,

        /// <summary>
        /// Int32,UInt32,Float
        /// </summary>
        ABCD,
        CDAB,
        BADC,
        DCBA,

        /// <summary>
        /// Int64,UInt64,Double
        /// </summary>
        ABCDEFGH,
        GHEFCDAB,
        BADCFEHG,
        HGFEDCBA,
    }
}
