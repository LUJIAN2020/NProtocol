using NProtocol.Protocols.S7.Enums;

namespace NProtocol.Protocols.S7
{
    public class MultipleItem
    {
        public MultipleItem(
            S7MemoryAreaType type = S7MemoryAreaType.DataBlock,
            ushort startAddress = 0,
            ushort count = 1,
            ushort? db = default,
            byte[]? writeData = default
        )
        {
            MemoryAreaType = type;
            DbNumber = db;
            StartAddress = startAddress;
            Count = count;
            WriteData = writeData;
        }

        public S7MemoryAreaType MemoryAreaType { get; }
        public ushort? DbNumber { get; }
        public ushort StartAddress { get; }
        public byte BitAddress { get; }
        public ushort Count { get; }
        public byte[]? WriteData { get; }
        public byte[]? ReadValue { get; set; }
        public S7CommReturnCode ReturnCode { get; set; }

        public override string ToString()
        {
            return MultipleParameterToAddressString(this);
        }

        private string MultipleParameterToAddressString(MultipleItem item)
        {
            string addr;
            if (item.DbNumber is not null || item.DbNumber > 0)
            {
                addr = $"DB{item.DbNumber}.DBX{item.StartAddress:0.0}";
            }
            else
            {
                string type = string.Empty;
                switch (item.MemoryAreaType)
                {
                    case S7MemoryAreaType.Input:
                        type = "I";
                        break;
                    case S7MemoryAreaType.Output:
                        type = "Q";
                        break;
                    case S7MemoryAreaType.Memory:
                        type = "M";
                        break;
                    case S7MemoryAreaType.DataBlock:
                        type = "DB";
                        break;
                    case S7MemoryAreaType.S7Counter:
                        type = "C";
                        break;
                    case S7MemoryAreaType.S7Timer:
                        type = "T";
                        break;
                    default:
                        break;
                }
                addr = $"{type}{item.StartAddress}";
            }
            return addr;
        }
    }
}
