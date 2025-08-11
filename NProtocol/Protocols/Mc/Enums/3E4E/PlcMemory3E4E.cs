namespace NProtocol.Protocols.Mc;

/// <summary>
/// Memory type iQ-R series
/// </summary>
public enum PlcMemory3E4E : byte
{
    /// <summary>
    /// input
    /// </summary>
    X = 0x9C,

    /// <summary>
    /// Direct input
    /// </summary>
    DX = 0xA2,

    /// <summary>
    /// output
    /// </summary>
    Y = 0x9D,

    /// <summary>
    /// Direct output
    /// </summary>
    DY = 0xA3,

    /// <summary>
    /// Link relay
    /// </summary>
    B = 0xA0,

    /// <summary>
    /// Special link relay
    /// </summary>
    SB = 0xA1,

    /// <summary>
    /// Internal relay
    /// </summary>
    M = 0x90,

    /// <summary>
    /// Special internal relay
    /// </summary>
    SM = 0x91,

    /// <summary>
    /// Latch relay
    /// </summary>
    L = 0x92,

    /// <summary>
    /// Signal relay
    /// </summary>
    F = 0x93,

    /// <summary>
    /// Edge relay
    /// </summary>
    V = 0x94,

    /// <summary>
    /// Step relay
    /// </summary>
    S = 0x98,

    /// <summary>
    /// Timer contact
    /// </summary>
    TS = 0xC1,

    /// <summary>
    /// Timer coil
    /// </summary>
    TC = 0xC0,

    /// <summary>
    /// Long-duration timer contact
    /// </summary>
    STS = 0xC7,

    /// <summary>
    /// Long-duration timer coil
    /// </summary>
    SC = 0xC6,

    /// <summary>
    /// Counter contact
    /// </summary>
    CS = 0xC4,

    /// <summary>
    /// Counter coil
    /// </summary>
    CC = 0xC3,

    /// <summary>
    /// Timer value
    /// </summary>
    TN = 0xC2,

    /// <summary>
    /// Long-duration timer value
    /// </summary>
    SN = 0xC8,

    /// <summary>
    /// Counter value
    /// </summary>
    CN = 0xC5,

    /// <summary>
    /// Data register
    /// </summary>
    D = 0xA8,

    /// <summary>
    /// Special data register
    /// </summary>
    SD = 0xA9,

    /// <summary>
    /// Link register
    /// </summary>
    W = 0xB4,

    /// <summary>
    /// Special link register
    /// </summary>
    SW = 0xB5,

    /// <summary>
    /// File register
    /// </summary>
    R = 0xAF,

    /// <summary>
    /// File register
    /// </summary>
    ZR = 0xB0,

    /// <summary>
    /// Index register
    /// </summary>
    Z = 0xCC,
}
