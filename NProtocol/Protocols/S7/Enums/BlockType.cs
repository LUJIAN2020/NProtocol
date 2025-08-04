namespace NProtocol.Protocols.S7.Enums
{
    /// <summary>
    /// 块类型
    /// </summary>
    public enum BlockType : ushort
    {
        OB = 0x3038,
        DB = 0x3041,
        SDB = 0x3042,
        FC = 0x3043,
        SFC = 0x3044,
        FB = 0x3045,
        SFB = 0x3046,
    }

    /// <summary>
    /// 子块类型
    /// </summary>
    public enum SubBlockType : byte
    {
        OB = 0x08,
        DB = 0x0A,
        SDB = 0x0B,
        FC = 0x0C,
        SFC = 0x0D,
        FB = 0x0E,
        SFB = 0x0F,
    }
}
