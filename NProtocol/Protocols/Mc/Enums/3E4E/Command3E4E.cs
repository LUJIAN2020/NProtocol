namespace NProtocol.Protocols.Mc
{
    /// <summary>
    /// MC指令码3E、4E主指令码
    /// </summary>
    public enum Command3E4E : ushort
    {
        /// <summary>
        /// 批量读取 Word/Bit
        /// </summary>
        BatchRead = 0x0401,
        /// <summary>
        /// 批量写入 Word/Bit
        /// </summary>
        BatchWrite = 0x1401,
        /// <summary>
        /// 随机读取 Word
        /// </summary>
        RandomRead = 0x0403,
        /// <summary>
        /// 随机写入 Word/Bit
        /// </summary>
        RandomWrite = 0x1402,
        /// <summary>
        /// 多块读取 Word
        /// </summary>
        MultiblockRead = 0x0406,
        /// <summary>
        /// 多块写入 Word
        /// </summary>
        MultiblockWrite = 0x1406,
    }
}
