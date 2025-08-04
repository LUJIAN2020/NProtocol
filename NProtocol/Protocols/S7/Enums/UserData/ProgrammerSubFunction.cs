namespace NProtocol.Protocols.S7.Enums
{
    public enum ProgrammerSubFunction : byte
    {
        RequestDiagDataType1 = 0x01,
        VarTab = 0x02,
        Erase = 0x0C,
        ReadDiagData = 0x0E,
        RemoveDiagData = 0x0F,
        Forces = 0x10,
        RequestDiagDataType2 = 0x13,
    }
}
