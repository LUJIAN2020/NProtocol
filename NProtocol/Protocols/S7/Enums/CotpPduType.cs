namespace NProtocol.Protocols.S7.Enums
{
    /// <summary>
    /// PDU Type of the COTP connection packet
    /// </summary>
    public enum CotpPduType : byte
    {
        /// <summary>
        /// Expedited data
        /// </summary>
        ExpeditedData = 0x01,

        /// <summary>
        /// Expedited data acknowledgment
        /// </summary>
        ExpeditedDataAcknowledgement = 0x02,

        /// <summary>
        /// User data
        /// </summary>
        UserData = 0x04,

        /// <summary>
        /// Reject
        /// </summary>
        Reject = 0x05,

        /// <summary>
        /// Data acknowledgment
        /// </summary>
        DataAcknowledgement = 0x06,

        /// <summary>
        /// TPDU error
        /// </summary>
        TPDUError = 0x07,

        /// <summary>
        /// Disconnect request
        /// </summary>
        DisconnectRequest = 0x08,

        /// <summary>
        /// Disconnect confirmation
        /// </summary>
        DisconnectConfirm = 0xC0,

        /// <summary>
        /// Connect confirmation
        /// </summary>
        ConnectConfirm = 0xD0,

        /// <summary>
        /// Connect request
        /// </summary>
        ConnectRequest = 0xE0,

        /// <summary>
        /// Data transfer
        /// </summary>
        Data = 0xF0,
    }
}
