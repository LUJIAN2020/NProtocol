namespace NProtocol.Protocols.S7.Enums
{
    /// <summary>
    /// 寄存器数据类型
    /// </summary>
    public enum S7MemoryAreaType
    {
        /// <summary>
        /// 输入I,E区 只读 可操作数据类型：int16/uint16/bit
        /// </summary>
        Input = 0x81,

        /// <summary>
        /// 输出Q,A区 读/写 可操作数据类型：int16/uint16/bit
        /// </summary>
        Output = 0x82,

        /// <summary>
        /// 标志内存M 读/写 可操作数据类型：int16/uint16/bit
        /// </summary>
        Memory = 0x83,

        /// <summary>
        /// 变量内存V，全局DB块 可操作数据类型：byte/uint16/bit/int32/uint32/float/double/string/wstring
        /// </summary>
        DataBlock = 0x84,

        /// <summary>
        /// 计数器C 读/写 可操作数据类型：int16/uint16
        /// </summary>
        S7Counter = 0x1C,

        /// <summary>
        /// 计时器T 读/写 可操作数据类型：int32/uint32/float
        /// </summary>
        S7Timer = 0x1D,
    }
}
