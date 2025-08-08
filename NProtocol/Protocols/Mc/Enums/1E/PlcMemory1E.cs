namespace NProtocol.Protocols.Mc
{
    /// <summary>
    /// 1E帧软元件 主要FX3U系列
    /// </summary>
    public enum PlcMemory1E : ushort
    {
        /// <summary>
        /// 输入
        /// </summary>
        X = 0x5820,

        /// <summary>
        /// 输出
        /// </summary>
        Y = 0x5920,

        /// <summary>
        /// 内部中继
        /// </summary>
        M = 0x4D20,

        /// <summary>
        /// 步进中继
        /// </summary>
        S = 0x5320,

        /// <summary>
        /// 计时器触点
        /// </summary>
        TS = 0x5453,

        /// <summary>
        /// 计数器触点
        /// </summary>
        CS = 0x4353,

        /// <summary>
        /// 计时器值
        /// </summary>
        TN = 0x544E,

        /// <summary>
        /// 计数器值
        /// </summary>
        CN = 0x434E,

        /// <summary>
        /// 数据寄存器
        /// </summary>
        D = 0x4420,

        /// <summary>
        /// 文件寄存器
        /// </summary>
        R = 0x5220,
    }
}
