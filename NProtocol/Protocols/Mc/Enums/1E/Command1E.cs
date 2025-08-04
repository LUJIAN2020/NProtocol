namespace NProtocol.Protocols.Mc
{
    public enum Command1E : byte
    {
        /// <summary>
        /// 批量读取位
        /// </summary>
        BatchReadBit = 0,

        /// <summary>
        /// 批量读取字
        /// </summary>
        BatchReadWord = 1,

        /// <summary>
        /// 批量写入位
        /// </summary>
        BatchWriteBit = 2,

        /// <summary>
        /// 批量写入字
        /// </summary>
        BatchWriteWord = 3,

        /// <summary>
        /// 随机写入位
        /// </summary>
        RandomWriteBit = 4,

        /// <summary>
        /// 随机写入字
        /// </summary>
        RandomWriteWord = 5,
    }
}
