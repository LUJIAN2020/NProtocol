namespace NProtocol.Protocols.S7.Enums
{
    /// <summary>
    /// Syntax Id，全称Syntax Ids of variable specification，
    /// 即IDS 的地址规范的格式类型，用于确定寻址模式和其余项目结构的格式
    /// 一般读写数据都十返回S7ANY
    /// </summary>
    public enum S7CommSyntaxId : byte
    {
        S7ANY = 0x10,
        PBC_R_ID = 0x13,
        ALARM_LOCKFREE = 0x15,
        ALARM_IND = 0x16,
        ALARM_ACK = 0x19,
        ALARM_QUERYREQ = 0x1A,
        NOTIFY_IND = 0x1C,
        DRIVEESANY = 0xA2,
        SYM1200 = 0xB2,
        DBREAD = 0xB0,
        NCK = 0x82,
    }
}
