namespace NProtocol.Protocols.Fins
{
    /// <summary>
    /// Fins command types
    /// </summary>
    public enum FinsCommand : ushort
    {
        /// <summary>
        /// area read
        /// </summary>
        ReadMemoryArea = 0x0101,

        /// <summary>
        /// area write
        /// </summary>
        WriteMemoryArea = 0x0102,
    }
}
