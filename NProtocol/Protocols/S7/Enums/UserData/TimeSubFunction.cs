namespace NProtocol.Protocols.S7.Enums.UserData
{
    /// <summary>
    /// User data time sub-function codes
    /// </summary>
    public enum TimeSubFunction : byte
    {
        /// <summary>
        /// Read clock
        /// </summary>
        ReadClock = 0x01,

        /// <summary>
        /// Set clock
        /// </summary>
        SetClock = 0x02,

        /// <summary>
        /// The following two are rarely used
        /// </summary>
        ReadClockFollowing = 0x03,
        SetClockFollowing = 0x04,
    }

}
