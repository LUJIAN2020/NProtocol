namespace NProtocol.Protocols.S7.StructType
{
    /// <summary>
    /// Type of DateTime
    /// </summary>
    public enum S7DateTimeType
    {
        /// <summary>
        /// 2 byte yyyy-MM-dd
        /// </summary>
        Date,
        /// <summary>
        /// 8 byte yyyy-MM-dd HH:mm:ss
        /// </summary>
        DateAndTime,
        /// <summary>
        /// 12 byte yyyy-MM-DD HH:mm:ss.ffffff
        /// </summary>
        DTL
    }
}
