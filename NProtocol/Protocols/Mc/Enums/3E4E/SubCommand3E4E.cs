namespace NProtocol.Protocols.Mc
{
    /// <summary>
    /// MC指令码3E、4E子指令码
    /// </summary>
    public enum SubCommand3E4E : ushort
    {
        /// <summary>
        /// 字为单位操作
        /// </summary>
        Word = 0x0000,
        /// <summary>
        /// 位为单位操作
        /// </summary>
        Bit = 0x0001,
    }
}
