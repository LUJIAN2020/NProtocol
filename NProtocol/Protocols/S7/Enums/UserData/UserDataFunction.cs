namespace NProtocol.Protocols.S7.Enums.UserData
{
    /// <summary>
    /// 用户数据的时候的功能码
    /// </summary>
    public enum UserDataFunction : byte
    {
        /// <summary>
        /// 转换工作模式
        /// </summary>
        ModeTransition = 0x00,

        /// <summary>
        /// 工程师调试命令
        /// </summary>
        ProgrammerCommands = 0x01,

        /// <summary>
        /// 循环读取
        /// </summary>
        CyclicData = 0x02,

        /// <summary>
        /// 块功能
        /// </summary>
        BlockFunctions = 0x03,

        /// <summary>
        /// CPU功能
        /// </summary>
        CPUFunctions = 0x04,

        /// <summary>
        /// 安全功能
        /// </summary>
        Security = 0x05,

        /// <summary>
        /// 可编程块函数发送接收
        /// </summary>
        PBC_BSEND_BRECV = 0x06,

        /// <summary>
        /// 时间功能码
        /// </summary>
        TimeFunctions = 0x07,

        /// <summary>
        /// NC编程
        /// </summary>
        NcProgramming = 0x0F,
    }
}
