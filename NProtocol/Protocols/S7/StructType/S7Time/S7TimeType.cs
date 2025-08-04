namespace NProtocol.Protocols.S7.StructType
{
    /// <summary>
    /// Type of TimeSpan
    /// </summary>
    public enum S7TimeType
    {
        /// <summary>
        /// 4 byte ms
        /// </summary>
        Time,
        /// <summary>
        /// 8 byte ns
        /// </summary>
        LTime,
        /// <summary>
        /// 4 byte ms
        /// </summary>
        TimeOfDay,
        /// <summary>
        /// 8 byte ns
        /// </summary>
        LTimeOfDay
    }
}
