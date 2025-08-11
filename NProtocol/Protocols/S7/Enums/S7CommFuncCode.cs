namespace NProtocol.Protocols.S7.Enums
{
    /// <summary>
    /// S7Comm main function codes
    /// </summary>
    public enum S7CommFuncCode : byte
    {
        /// <summary>
        /// CPU services
        /// </summary>
        CpuServices = 0x00,

        /// <summary>
        /// Return function code 02 in case of error
        /// </summary>
        Response = 0x02,

        /// <summary>
        /// Read value
        /// </summary>
        ReadVar = 0x04,

        /// <summary>
        /// Write value
        /// </summary>
        WriteVar = 0x05,

        /// <summary>
        /// Request download
        /// </summary>
        RequestDownload = 0x1A,

        /// <summary>
        /// Download data block
        /// </summary>
        DownloadBlock = 0x1B,

        /// <summary>
        /// Download ended
        /// </summary>
        DownloadEnded = 0x1C,

        /// <summary>
        /// Start upload
        /// </summary>
        StartUpload = 0x1D,

        /// <summary>
        /// Upload
        /// </summary>
        Upload = 0x1E,

        /// <summary>
        /// End upload
        /// </summary>
        EndUpload = 0x1F,

        /// <summary>
        /// Program control service
        /// </summary>
        PlcControl = 0x28,

        /// <summary>
        /// Stop PLC
        /// </summary>
        PlcStop = 0x29,

        /// <summary>
        /// Establish communication and negotiate PDU length
        /// </summary>
        SetupCommunication = 0xF0,
    }

}
