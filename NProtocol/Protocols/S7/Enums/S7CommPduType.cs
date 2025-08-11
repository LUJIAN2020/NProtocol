namespace NProtocol.Protocols.S7.Enums
{
    /// <summary>
    /// S7Comm protocol PDU Type
    /// </summary>
    public enum S7CommPduType : byte
    {
        /// <summary>
        /// Job request, such as read/write memory, read/write block, start/stop device, set communication PduType_request
        /// </summary>
        Job = 0x01,

        /// <summary>
        /// Acknowledgment response, a simple acknowledgment without data (never encountered from S7 300/400 devices);
        /// </summary>
        Ack = 0x02,

        /// <summary>
        /// Acknowledgment data response, usually in response to a Job request; PduType_response
        /// </summary>
        AckData = 0x03,

        /// <summary>
        /// Extension of the original protocol, parameter field contains request/response ID (used for programming/debugging, reading SZL, security functions, time settings, cyclic reading, etc.). PduType_userdata
        /// </summary>
        UserData = 0x07,
    }
}
