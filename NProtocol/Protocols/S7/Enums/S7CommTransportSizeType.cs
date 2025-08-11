namespace NProtocol.Protocols.S7.Enums
{
    /// <summary>
    /// Each item transport size type in the S7Comm response data, which determines the data arrangement type
    /// </summary>
    public enum S7CommTransportSizeType : byte
    {
        /// <summary>
        /// No data
        /// </summary>
        NULL = 0,

        /// <summary>
        /// Data arranged by bit
        /// </summary>
        BIT = 3, // 1
        BYTE_WORD_DWORD = 4, // Actual length * byte
        INTEGER = 5, // 2

        /// <summary>
        /// Data arranged by byte
        /// </summary>
        DINTEGER = 6, // 4
        REAL = 7, // 4
        OCTET_STRING = 9, // 1 * actual
    }
}
