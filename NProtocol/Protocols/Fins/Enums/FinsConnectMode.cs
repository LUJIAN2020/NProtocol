namespace NProtocol.Protocols.Fins
{
    /// <summary>
    /// Fin connection type
    /// </summary>
    public enum FinsConnectMode : byte
    {
        /// <summary>
        /// fins over tcp
        /// </summary>
        FinsTcp = 0,

        /// <summary>
        /// fins over udp
        /// </summary>
        FinsUdp = 1,
    }
}
