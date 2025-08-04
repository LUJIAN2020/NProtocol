namespace NProtocol.Protocols.S7
{
    public readonly struct Tsap
    {
        public Tsap(byte firstByte, byte secondByte)
        {
            FirstByte = firstByte;
            SecondByte = secondByte;
        }
        public byte FirstByte { get; }
        public byte SecondByte { get; }
    }
}
