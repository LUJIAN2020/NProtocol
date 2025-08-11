namespace NProtocol.Protocols.S7.Enums
{
    /// <summary>
    /// Syntax Id, full name: Syntax Ids of variable specification,
    /// which refers to the format type of the IDS address specification, used to determine the addressing mode and the format of the remaining item structure.
    /// Typically, read and write data will return S7ANY.
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
