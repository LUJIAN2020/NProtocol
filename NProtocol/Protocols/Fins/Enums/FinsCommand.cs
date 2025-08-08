namespace NProtocol.Protocols.Fins
{
    /// <summary>
    /// Fins command types
    /// </summary>
    public enum FinsCommand : ushort
    {
        /// <summary>
        /// area read
        /// </summary>
        ReadMemoryArea = 0x0101,

        /// <summary>
        /// area write
        /// </summary>
        WriteMemoryArea = 0x0102,

#if DEBUG
        #region 下面的指令不常用，目前没有实现

        /// <summary>
        /// 内存填充 向特定范围I/O内存区填充相同的数据
        /// </summary>
        FillMemoryArea = 0x0103,

        /// <summary>
        /// 非连续内存读取 读取指定的非连续I/O内存区域数据
        /// </summary>
        ReadMultipleStorageArea = 0x0104,

        /// <summary>
        ///  内存转移 将非连续内存区的
        /// </summary>
        StorageTransfer = 0x0105,

        //参数区读写
        /// <summary>
        /// 参数区读取 读取连续参数区内容
        /// </summary>
        ReadParameterArea = 0x0201,

        /// <summary>
        /// 参数区写入 写入连续参数区内容
        /// </summary>
        WriteParameterArea = 0x0202,

        /// <summary>
        /// 参数区填充 向特定范围参数区填充相同的内容
        /// </summary>
        FillParameterArea = 0x0203,

        //程序区读写
        /// <summary>
        /// 程序读取 读取UM（用户内存）区
        /// </summary>
        ProgramRead = 0x0306,

        /// <summary>
        /// 程序写入 写入UM（用户内存）区
        /// </summary>
        ProgramWrite = 0x0307,

        /// <summary>
        /// 程序清除 清除UM（用户内存）区
        /// </summary>
        ProgramClear = 0x0308,

        //操作模式切换
        /// <summary>
        /// 运行 将CPU单元操作模式切换为运行或监视
        /// </summary>
        Run = 0x0401,

        /// <summary>
        /// 停止 将CPU单元操作模式切换为编程
        /// </summary>
        Stop = 0x0402,

        //设备配置读取
        /// <summary>
        /// CPU单元数据读取 读取CPU单元数据
        /// </summary>
        CpuUnitDataRead = 0x0501,

        /// <summary>
        /// 连接状态读取 读取对应地址的模块数量
        /// </summary>
        ConnectionStatusRead = 0x0502,

        //状态读取
        /// <summary>
        /// CPU单元状态读取 读取CPU单元状态
        /// </summary>
        CpuUnitStatusRead = 0x0601,

        /// <summary>
        /// 循环时间读取 读取最大、最小和平均循环时间
        /// </summary>
        CycleTimeRead = 0x0620,

        //时间数据读写
        /// <summary>
        /// 时钟读取 读取当前年、月、日、分、秒和星期几
        /// </summary>
        ClockRead = 0x0701,

        /// <summary>
        /// 时钟写入 改变当前年、月、日、分、秒和星期几
        /// </summary>
        ClockWrite = 0x0702,

        //故障信息显示
        /// <summary>
        /// 信息读取/清除 读取和清除信息，读取故障和严重故障信息
        /// </summary>
        ReadOrClearFaultInformation = 0x0920,

        //访问控制权限
        /// <summary>
        /// 获取访问权限 只要没有其它设备持有访问权限，则获得访问权限
        /// </summary>
        GetAccess = 0x0C01,

        /// <summary>
        /// 强制获取访问权限 即使有其它设备持有访问权限，仍获得访问权限
        /// </summary>
        ForceAccess = 0x0C02,

        /// <summary>
        /// 释放访问权限 即使已经持有访问权限，仍释放访问权限
        /// </summary>
        FreeAccess = 0x0C03,

        //错误日志
        /// <summary>
        /// 清除错误 清除错误或报警
        /// </summary>
        ClearError = 0x2101,

        /// <summary>
        /// 读取错误日志 读取错误日志
        /// </summary>
        ReadErrorLog = 0x2102,

        /// <summary>
        /// 清除若无日志 清除错误日志指针
        /// </summary>
        ClearErrorLogPointer = 0x2103,

        //FINS登入日志
        /// <summary>
        /// FINS登入日志读取 CPU单元自动保存有执行过FINS登入命令的日志。这条命令读取此日志。
        /// </summary>
        FinsLoginLogRead = 0x2140,

        /// <summary>
        /// FINS登入日志清除 清除FINS登入列表
        /// </summary>
        FinsLoginLogClear = 0x2141,

        //文件内存
        /// <summary>
        /// 文件名读取 读取文件内存区数据
        /// </summary>
        FileNameRead = 0x2201,

        /// <summary>
        /// 单个文件读取 从某个文件中的指定位置读取特定长度的文件数据
        /// </summary>
        SingleFileRead = 0x2202,

        /// <summary>
        /// 单个文件写入 从某个文件中的指定位置写入特定长度的文件数据
        /// </summary>
        SingleFileWrite = 0x2203,

        /// <summary>
        /// 文件内存格式化 格式化文件内存
        /// </summary>
        FormattingFileMemory = 0x2204,

        /// <summary>
        /// 文件删除 从文件内存中删除指定文件
        /// </summary>
        DeleteFile = 0x2205,

        /// <summary>
        /// 文件复制 在系统中将某些文件复制到其他位置
        /// </summary>
        CopyFile = 0x2207,

        /// <summary>
        /// 重命名文件 改变一个文件的名字
        /// </summary>
        FileRename = 0x2208,

        /// <summary>
        /// 内存区间数据转移1 在I/O内存和文件内存间转移或比较数据
        /// </summary>
        MemoryIntervalDataTransfer1 = 0x220A,

        /// <summary>
        /// 内存区间数据转移2 在参数区和文件内存间转移或比较数据
        /// </summary>
        MemoryIntervalDataTransfer2 = 0x220B,

        /// <summary>
        /// 内存区间数据转移3 在用户内存和文件内存间转移或比较数据
        /// </summary>
        MemoryIntervalDataTransfer3 = 0x220C,

        /// <summary>
        /// 创建/删除文件夹 创建或删除一个文件夹
        /// </summary>
        CreateOrDeleteFolders = 0x2215,

        /// <summary>
        /// 存储盒转移（只针对CP1H，CP1L CPU单元） 在存储盒与CPU单元间转移和修改数据
        /// </summary>
        StorageBoxTransfer = 0x2220,

        //调试
        /// <summary>
        ///  强制设置/重置 强制设置或重置位，或推出强制设置状态
        /// </summary>
        ForceSettingOrReset = 0x2301,

        /// <summary>
        ///  强制设置/重置取消 取消所有强制设置或重置过的位
        /// </summary>
        CancelForceSettingOrReset = 0x2302

        #endregion
#endif
    }
}
