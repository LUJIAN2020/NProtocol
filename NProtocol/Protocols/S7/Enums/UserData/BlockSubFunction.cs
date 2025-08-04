namespace NProtocol.Protocols.S7.Enums
{
    public enum BlockSubFunction : byte
    {
        ListBlocks = 0x01,
        ListBlocksOfType = 0x02,
        GetBlockInfo = 0x03,
    }
}
