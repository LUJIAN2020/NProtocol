namespace NProtocol.Protocols.S7.Enums
{
    /// <summary>
    /// 返回数据项响应的状态值
    /// </summary>
    public enum S7CommReturnCode : byte
    {
        /// <summary>
        /// 保留
        /// </summary>
        Reserved = 0x00,

        /// <summary>
        /// 硬件错误
        /// </summary>
        HardwareFault = 0x01,

        /// <summary>
        /// 对象不允许访问
        /// </summary>
        AccessingTheObjectNotAllowed = 0x03,

        /// <summary>
        /// 无效地址，所需的地址超出此PLC的极限
        /// </summary>
        AddressOutOfRange = 0x05,

        /// <summary>
        /// 数据类型不支持
        /// </summary>
        DataTypeNotSupported = 0x06,

        /// <summary>
        /// 日期类型不一致
        /// </summary>
        DataTypeInconsistent = 0x07,

        /// <summary>
        /// 对象不存在
        /// </summary>
        ObjectDoesNotExist = 0x0A,

        /// <summary>
        /// 成功
        /// </summary>
        Success = 0xFF,
    }
}
