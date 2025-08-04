namespace NProtocol.Protocols.Fins
{
    /// <summary>
    /// String format type
    /// </summary>
    public enum StringFormatType : byte
    {
        /// <summary>
        /// Bytes are sorted from high to low
        /// </summary>
        H = 0,
        /// <summary>
        /// Bytes are sorted from low to high
        /// </summary>
        L = 1,
        /// <summary>
        /// Only the high byte of each character is used
        /// </summary>
        D = 2,
        /// <summary>
        /// Only the lower byte of each character is used
        /// </summary>
        E = 3
    }
}
