namespace NProtocol.Protocols.Modbus
{
    public enum ReadOrWrite : byte
    {
        Read = 0,
        Write = 1,
        ReadWrite = 2,
    }
}
