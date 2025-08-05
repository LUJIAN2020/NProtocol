using NProtocol.Extensions;
using System.Text;

namespace NProtocol.Protocols.S7.CpuInfo
{
    /// <summary>
    /// CPU模块信息
    /// </summary>
    public class CpuModelInfo
    {
        public CpuModelInfo(byte[] buffer, Encoding encoding, bool isLittleEndian = true)
        {
            Model = new Model(buffer.Slice(0, 28), encoding, isLittleEndian);
            Hardware = new Model(buffer.Slice(28, 28), encoding, isLittleEndian);
            Firmware = new Model(buffer.Slice(2 * 28, 28), encoding, isLittleEndian);
            FirmwareExtension = new Model(buffer.Slice(3 * 28, 28), encoding, isLittleEndian);
        }
        /// <summary>
        /// 模块信息
        /// </summary>
        public Model Model { get; private set; }
        /// <summary>
        /// 硬件信息
        /// </summary>
        public Model Hardware { get; private set; }
        /// <summary>
        /// 版本信息
        /// </summary>
        public Model Firmware { get; private set; }
        /// <summary>
        /// 扩展版本信息
        /// </summary>
        public Model FirmwareExtension { get; private set; }
    }
    /// <summary>
    /// 模块信息
    /// </summary>
    public class Model
    {
        public Model(byte[] buffer, Encoding encoding, bool isLittleEndian = true)
        {
            Identification = buffer.Slice(0, 2).ToUInt16(isLittleEndian);
            OrderNumber = encoding.GetString(buffer.Slice(2, 20)).Replace("\0", "").Trim();
            TypeId = buffer.Slice(22, 2).ToUInt16(isLittleEndian);
            Version = buffer.Slice(24, 2).ToUInt16(isLittleEndian);
            Release = buffer.Slice(26, 2).ToUInt16(isLittleEndian);
        }
        /// <summary>
        /// Index: Identification of the module (0x0001)
        /// </summary>
        public ushort Identification { get; private set; }
        /// <summary>
        /// MlfB (Order number of the module): 6ES7 517-3UP00-0AB0 
        /// </summary>
        public string OrderNumber { get; private set; }
        /// <summary>
        /// BGTyp (Module type ID): 0x0000
        /// </summary>
        public ushort TypeId { get; private set; }
        /// <summary>
        /// Version of the module or release of the operating system
        /// </summary>
        public ushort Version { get; private set; }
        /// <summary>
        /// Release of the PG description file
        /// </summary>
        public ushort Release { get; private set; }
    }
}
