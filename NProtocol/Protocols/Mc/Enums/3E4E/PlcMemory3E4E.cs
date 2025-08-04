namespace NProtocol.Protocols.Mc;

/// <summary>
/// 内存类型 iQ-R系列
/// </summary>
public enum PlcMemory3E4E : byte
{
    /// <summary>
    /// 输入
    /// </summary>
    X = 0x9C,
    /// <summary>
    /// 直接输入
    /// </summary>
    DX = 0xA2,
    /// <summary>
    /// 输出
    /// </summary>
    Y = 0x9D,
    /// <summary>
    /// 直接输出
    /// </summary>
    DY = 0xA3,
    /// <summary>
    /// 链接中继
    /// </summary>
    B = 0xA0,
    /// <summary>
    /// 特殊链接中继
    /// </summary>
    SB = 0xA1,
    /// <summary>
    /// 内部中继
    /// </summary>
    M = 0x90,
    /// <summary>
    /// 特殊内部中继
    /// </summary>
    SM = 0x91,
    /// <summary>
    /// 锁存器中继
    /// </summary>
    L = 0x92,
    /// <summary>
    /// 信号器中继
    /// </summary>
    F = 0x93,
    /// <summary>
    /// 边缘中继
    /// </summary>
    V = 0x94,
    /// <summary>
    /// 步进中继
    /// </summary>
    S = 0x98,
    /// <summary>
    /// 计时器触点
    /// </summary>
    TS = 0xC1,
    /// <summary>
    /// 计时器线圈
    /// </summary>
    TC = 0xC0,
    /// <summary>
    /// 长效计时器触点
    /// </summary>
    STS = 0xC7,
    /// <summary>
    /// 长效计时器线圈
    /// </summary>
    SC = 0xC6,
    /// <summary>
    /// 计数器触点
    /// </summary>
    CS = 0xC4,
    /// <summary>
    /// 计数器线圈
    /// </summary>
    CC = 0xC3,
    /// <summary>
    /// 计时器值
    /// </summary>
    TN = 0xC2,
    /// <summary>
    /// 长效计时器值
    /// </summary>
    SN = 0xC8,
    /// <summary>
    /// 计数器值
    /// </summary>
    CN = 0xC5,
    /// <summary>
    /// 数据寄存器
    /// </summary>
    D = 0xA8,
    /// <summary>
    /// 特殊数据寄存器
    /// </summary>
    SD = 0xA9,
    /// <summary>
    /// 链接寄存器
    /// </summary>
    W = 0xB4,
    /// <summary>
    /// 特殊链接寄存器
    /// </summary>
    SW = 0xB5,
    /// <summary>
    /// 文件寄存器
    /// </summary>
    R = 0xAF,
    /// <summary>
    /// 文件寄存器
    /// </summary>
    ZR = 0xB0,
    /// <summary>
    /// 索引寄存器
    /// </summary>
    Z = 0xCC,
}
