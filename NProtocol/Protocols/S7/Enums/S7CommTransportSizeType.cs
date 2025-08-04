namespace NProtocol.Protocols.S7.Enums
{
    /// <summary>
    /// S7Comm返回数据的每一项传输尺寸类型，决定数据编排类型
    /// </summary>
    public enum S7CommTransportSizeType : byte
    {
        /// <summary>
        /// 无数据
        /// </summary>
        NULL = 0,

        /// <summary>
        /// 按位进行数据编排
        /// </summary>
        BIT = 3,//1
        BYTE_WORD_DWORD = 4,//实际长度*byte
        INTEGER = 5,//2

        /// <summary>
        /// 按字节进行数据编排
        /// </summary>
        DINTEGER = 6,//4
        REAL = 7,//4
        OCTET_STRING = 9,//1*实际
    }
}
