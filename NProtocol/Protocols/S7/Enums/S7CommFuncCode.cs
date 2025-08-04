namespace NProtocol.Protocols.S7.Enums
{
    /// <summary>
    /// S7Comm主功能码
    /// </summary>
    public enum S7CommFuncCode : byte
    {
        /// <summary>
        /// CPU服务
        /// </summary>
        CpuServices = 0x00,

        /// <summary>
        /// 有错误的时候返回02功能码
        /// </summary>
        Response = 0x02,

        /// <summary>
        /// 读取值
        /// </summary>
        ReadVar = 0x04,

        /// <summary>
        /// 写入值
        /// </summary>
        WriteVar = 0x05,

        /// <summary>
        /// 请求下载
        /// </summary>
        RequestDownload = 0x1A,

        /// <summary>
        /// 下载数据块
        /// </summary>
        DownloadBlock = 0x1B,

        /// <summary>
        /// 下载结束
        /// </summary>
        DownloadEnded = 0x1C,

        /// <summary>
        /// 开始上传
        /// </summary>
        StartUpload = 0x1D,

        /// <summary>
        /// 上传
        /// </summary>
        Upload = 0x1E,

        /// <summary>
        /// 上传结束
        /// </summary>
        EndUpload = 0x1F,

        /// <summary>
        /// 程序调用服务
        /// </summary>
        PlcControl = 0x28,

        /// <summary>
        /// 关闭PLC
        /// </summary>
        PlcStop = 0x29,

        /// <summary>
        /// 建立通信 协商PDU长度
        /// </summary>
        SetupCommunication = 0xF0,
    }
}
