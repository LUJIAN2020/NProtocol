namespace NProtocol.Protocols.S7.Enums
{
    /// <summary>
    /// S7Comm协议PDU Type
    /// </summary>
    public enum S7CommPduType : byte
    {
        /// <summary>
        /// 作业请求，如，读/写存储器，读/写块，启动/停止设备，设置通信 PduType_request
        /// </summary>
        Job = 0x01,

        /// <summary>
        /// 确认响应，没有数据的简单确认（未遇到过由S7 300/400设备发送得）；
        /// </summary>
        Ack = 0x02,

        /// <summary>
        /// 确认数据响应，这个一般都是响应Job的请求； PduType_response
        /// </summary>
        AckData = 0x03,

        /// <summary>
        /// 原始协议的扩展，参数字段包含请求/响应ID（用于编程/调试，读取SZL，安全功能，时间设置，循环读取...）。PduType_userdata
        /// </summary>
        UserData = 0x07,
    }
}
