namespace NProtocol.Protocols.Fins
{
    /// <summary>
    /// Memory area type
    /// </summary>
    public enum PlcMemory : byte
    {
        /// <summary>
        /// CIO area read/write minimum unit of bytes
        /// </summary>
        CIO = 0,

        /// <summary>
        /// Work area, read/write minimum unit of bytes
        /// </summary>
        W = 1,

        /// <summary>
        /// Keep the area, read/write the minimum unit of characters
        /// </summary>
        H = 2,

        /// <summary>
        /// Auxiliary area Part read only minimum unit word
        /// </summary>
        A = 3,

        /// <summary>
        /// Data storage area read/write minimum unit of bytes
        /// </summary>
        D = 4,

        /// <summary>
        /// Timer, read/write, word operations
        /// </summary>
        T = 5,

        /// <summary>
        /// Counters, read/write, word operations
        /// </summary>
        C = 6,

        /// <summary>
        /// Extend data memory (current memory group) read/write
        /// </summary>
        E = 7,

        /// <summary>
        /// Expand data memory read/write
        /// </summary>
        EB = 8,
    }
}
