namespace NProtocol.Protocols.S7.Enums
{
    /// <summary>
    /// COTP连接包的PDU Type
    /// </summary>
    public enum CotpPduType : byte
    {
        /// <summary>
        /// 加急数据
        /// </summary>
        ExpeditedData = 0x01,

        /// <summary>
        /// 加急数据确认
        /// </summary>
        ExpeditedDataAcknowledgement = 0x02,

        /// <summary>
        /// 用户数据
        /// </summary>
        UserData = 0x04,

        /// <summary>
        /// 拒绝
        /// </summary>
        Reject = 0x05,

        /// <summary>
        /// 数据确认
        /// </summary>
        DataAcknowledgement = 0x06,

        /// <summary>
        /// TPDU错误
        /// </summary>
        TPDUError = 0x07,

        /// <summary>
        /// 断开请求
        /// </summary>
        DisconnectRequest = 0x08,

        /// <summary>
        /// 断开确认
        /// </summary>
        DisconnectConfirm = 0xC0,

        /// <summary>
        /// 连接确认
        /// </summary>
        ConnectConfirm = 0xD0,

        /// <summary>
        /// 连接请求
        /// </summary>
        ConnectRequest = 0xE0,

        /// <summary>
        /// 数据传输
        /// </summary>
        Data = 0xF0,
    }
}
