using System.Text;
using NProtocol.Extensions;

namespace NProtocol.Protocols.S7.CpuInfo
{
    public class CpuCompnenetInfo
    {
        public CpuCompnenetInfo(byte[] buffer, Encoding encoding)
        {
            int startIndex = 0;
            AutomationSystemName = GetString(encoding, buffer, ref startIndex);
            ModuleName = GetString(encoding, buffer, ref startIndex);
            ModulePlantIdentification = GetString(encoding, buffer, ref startIndex);
            Copyright = GetString(encoding, buffer, ref startIndex);
            SerialNumber = GetString(encoding, buffer, ref startIndex);
            CpuTypeName = GetString(encoding, buffer, ref startIndex);
            MemoryCardSerialNumber = GetString(encoding, buffer, ref startIndex);
            ManufacturerProfile = GetString(encoding, buffer, ref startIndex);
            OemId = GetString(encoding, buffer, ref startIndex);
            LocationDesignation = GetString(encoding, buffer, ref startIndex);
        }

        private string GetString(Encoding encoding, byte[] buffer, ref int start)
        {
            string msg = encoding.GetString(buffer.Slice(start += 2, 32)).Replace("\0", "").Trim();
            start += 32;
            return msg;
        }

        public string AutomationSystemName { get; private set; }
        public string ModuleName { get; private set; }
        public string ModulePlantIdentification { get; private set; }
        public string Copyright { get; private set; }
        public string SerialNumber { get; private set; }
        public string CpuTypeName { get; private set; }
        public string MemoryCardSerialNumber { get; private set; }
        public string ManufacturerProfile { get; private set; }
        public string OemId { get; private set; }
        public string LocationDesignation { get; private set; }
    }
}
