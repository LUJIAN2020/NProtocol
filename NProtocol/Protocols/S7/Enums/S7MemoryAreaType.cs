namespace NProtocol.Protocols.S7.Enums
{
    /// <summary>
    /// Register data types
    /// </summary>
    public enum S7MemoryAreaType
    {
        /// <summary>
        /// Input I, E areas, read-only. Supported data types: int16/uint16/bit
        /// </summary>
        Input = 0x81,

        /// <summary>
        /// Output Q, A areas, read/write. Supported data types: int16/uint16/bit
        /// </summary>
        Output = 0x82,

        /// <summary>
        /// Flag memory M, read/write. Supported data types: int16/uint16/bit
        /// </summary>
        Memory = 0x83,

        /// <summary>
        /// Variable memory V, global DB blocks. Supported data types: byte/uint16/bit/int32/uint32/float/double/string/wstring
        /// </summary>
        DataBlock = 0x84,

        /// <summary>
        /// Counter C, read/write. Supported data types: int16/uint16
        /// </summary>
        S7Counter = 0x1C,

        /// <summary>
        /// Timer T, read/write. Supported data types: int32/uint32/float
        /// </summary>
        S7Timer = 0x1D,
    }
}
