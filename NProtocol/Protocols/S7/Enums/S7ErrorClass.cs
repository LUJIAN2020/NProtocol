namespace NProtocol.Protocols.S7.Enums
{
    /// <summary>
    /// 错误类型
    /// </summary>
    public enum S7ErrorClass : byte
    {
        /// <summary>
        /// 没有错误
        /// </summary>
        NoError = 0x00,

        /// <summary>
        /// 应用关系
        /// </summary>
        ApplicationRelationship = 0x81,

        /// <summary>
        /// 对象定义
        /// </summary>
        ObjectDefinition = 0x82,

        /// <summary>
        /// 没有可用资源
        /// </summary>
        NoResourcesAvailable = 0x83,

        /// <summary>
        /// 服务处理中错误
        /// </summary>
        ErrorOnServiceProcessing = 0x84,

        /// <summary>
        /// 请求错误
        /// </summary>
        ErrorOnSupplies = 0x85,

        /// <summary>
        /// 访问错误
        /// </summary>
        AccessError = 0x87,
    }
}
