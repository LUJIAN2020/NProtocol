namespace NProtocol.Protocols.Mc
{
    /// <summary>
    /// MC instruction code 3E and 4E main instruction code
    /// </summary>
    public enum Command3E4E : ushort
    {
        /// <summary>
        /// Batch read (Word/Bit)
        /// </summary>
        BatchRead = 0x0401,

        /// <summary>
        /// Batch write (Word/Bit)
        /// </summary>
        BatchWrite = 0x1401,

        /// <summary>
        /// Random read (Word)
        /// </summary>
        RandomRead = 0x0403,

        /// <summary>
        /// Random write (Word/Bit)
        /// </summary>
        RandomWrite = 0x1402,

        /// <summary>
        /// Multiblock read (Word)
        /// </summary>
        MultiblockRead = 0x0406,

        /// <summary>
        /// Multiblock write (Word)
        /// </summary>
        MultiblockWrite = 0x1406,
    }
}
