namespace NProtocol.Protocols.S7.Enums
{
    /// <summary>
    /// Status values for response to return data items
    /// </summary>
    public enum S7CommReturnCode : byte
    {
        /// <summary>
        /// Reserved
        /// </summary>
        Reserved = 0x00,

        /// <summary>
        /// Hardware fault
        /// </summary>
        HardwareFault = 0x01,

        /// <summary>
        /// Access to the object is not allowed
        /// </summary>
        AccessingTheObjectNotAllowed = 0x03,

        /// <summary>
        /// Invalid address, the required address is out of range for this PLC
        /// </summary>
        AddressOutOfRange = 0x05,

        /// <summary>
        /// Data type not supported
        /// </summary>
        DataTypeNotSupported = 0x06,

        /// <summary>
        /// Data type inconsistency
        /// </summary>
        DataTypeInconsistent = 0x07,

        /// <summary>
        /// Object does not exist
        /// </summary>
        ObjectDoesNotExist = 0x0A,

        /// <summary>
        /// Success
        /// </summary>
        Success = 0xFF,
    }
}
