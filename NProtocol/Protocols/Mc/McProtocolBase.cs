using NProtocol.Base;
using NProtocol.Connectors;
using NProtocol.Enums;

namespace NProtocol.Protocols.Mc
{
    /*
        MC协议是一种现场总线通信协议，常见的通讯格式有以下几种：
        3E帧格式：
            3E帧格式是最常用的通讯格式，由三个字节的帧头、一个字节的设备地址、一个字节的功能码、一个字节的数据长度、若干字节的数据和两个字节的CRC校验码组成。
        3C帧格式：
            3C帧格式与3E帧格式相似，不同之处在于帧头由两个字节组成，其余部分与3E帧格式一致。
        4C帧格式：
            4C帧格式与3C帧格式类似，但加入了一个扩展位，表示数据是否需要加密。
        4E帧格式：
            4E帧格式是一种高速通讯格式，由四个字节的帧头、一个字节的设备地址、一个字节的功能码、一个字节的数据长度、若干字节的数据和两个字节的CRC校验码组成。
     */


    public abstract class McProtocolBase : DriverBase
    {
        public McProtocolBase(EtherNetParameter parameter, ConnectMode mode)
            : base(parameter, mode) { }

        /// <summary>
        /// Correct end code
        /// </summary>
        public const byte OkEndCode = 0;

        /// <summary>
        /// Error end code
        /// </summary>
        public const byte ErrorEndCode = 0x5B;

        /// <summary>
        /// Programmable Controller Number
        /// </summary>
        public byte PlcNumber { get; set; } = 0xFF;

        /// <summary>
        /// CPU monitoring timer
        /// </summary>
        public ushort CpuTimer { get; set; }

        /// <summary>
        /// MC protocol defaults to big-endian byte order
        /// </summary>
        public bool IsLittleEndian { get; } = false;
    }
}
