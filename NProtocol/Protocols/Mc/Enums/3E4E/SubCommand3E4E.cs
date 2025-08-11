namespace NProtocol.Protocols.Mc
{
    /// <summary>
    /// MC instruction code 3E, 4E sub-command codes
    /// </summary>
    public enum SubCommand3E4E : ushort
    {
        /// <summary>
        /// Word-based operation
        /// </summary>
        Word = 0x0000,

        /// <summary>
        /// Bit-based operation
        /// </summary>
        Bit = 0x0001,
    }
}
