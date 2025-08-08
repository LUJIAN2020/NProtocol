namespace NProtocol.Protocols.S7.Enums.UserData
{
    /// <summary>
    /// 用户数据时间子功能码
    /// </summary>
    public enum TimeSubFunction : byte
    {
        /// <summary>
        /// 读时钟
        /// </summary>
        ReadClock = 0x01,

        /// <summary>
        /// 设置时钟
        /// </summary>
        SetClock = 0x02,

        /// <summary>
        /// 下面这两个不常用
        /// </summary>
        ReadClockFollowing = 0x03,
        SetClockFollowing = 0x04,
    }
}
