namespace NProtocol.Protocols.S7.Enums
{
    /// <summary>
    /// 错误代码
    /// </summary>
    public enum S7ErrorCode : byte
    {
        /// <summary>
        /// The function has been executed correctly
        /// </summary>
        NoError = 0,

        /// <summary>
        /// Wrong type of CPU error
        /// </summary>
        WrongCpuType = 1,

        /// <summary>
        /// Connection error
        /// </summary>
        ConnectionError = 2,

        /// <summary>
        /// Ip address not available
        /// </summary>
        IPAddressNotAvailable,

        /// <summary>
        /// Wrong format of the variable
        /// </summary>
        WrongVarFormat = 0x0A,

        /// <summary>
        /// Wrong number of received bytes
        /// </summary>
        WrongNumberReceivedBytes = 0x0B,

        /// <summary>
        /// Error on send data
        /// </summary>
        SendData = 0x14,

        /// <summary>
        /// Error on read data
        /// </summary>
        ReadData = 0x1E,

        /// <summary>
        /// Error on write data
        /// </summary>
        WriteData = 0x32,
    }
}
