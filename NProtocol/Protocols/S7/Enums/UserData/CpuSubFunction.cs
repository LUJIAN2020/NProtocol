namespace NProtocol.Protocols.S7.Enums
{
    public enum CpuSubFunction : byte
    {
        /// <summary>
        /// Read system status list
        /// </summary>
        ReadSZL = 0x01,

        /// <summary>
        /// Message service
        /// </summary>
        MessageService = 0x02,

        /// <summary>
        /// Diagnostic message
        /// </summary>
        DiagnosticMessage = 0x03,

        /// <summary>
        /// Show alarm
        /// </summary>
        ShowAlarm = 0x05,

        /// <summary>
        /// Show notification
        /// </summary>
        ShowNotify = 0x06,

        /// <summary>
        /// Lock alarm
        /// </summary>
        LockingAlarm = 0x07,

        /// <summary>
        /// Cancel lock alarm
        /// </summary>
        CancelLockingAlarm = 0x08,

        /// <summary>
        /// Show scan
        /// </summary>
        ShowScan = 0x09,

        /// <summary>
        /// Acknowledge alarm
        /// </summary>
        AlarmAcknowledged = 0x0B,

        /// <summary>
        /// Acknowledge show alarm
        /// </summary>
        ShowAlarmAcknowledged = 0x0C,

        /// <summary>
        /// Lock show alarm
        /// </summary>
        LockingShowAlarm = 0x0D,

        /// <summary>
        /// Cancel lock show alarm
        /// </summary>
        CancelLockingShowAlarm = 0x0E,

        /// <summary>
        /// Show alarm SQ
        /// </summary>
        PlcIndicatingAlarmMessage = 0x11,

        /// <summary>
        /// Show alarm S
        /// </summary>
        PlcIndicatingAlarmS = 0x12,

        /// <summary>
        /// Query alarm
        /// </summary>
        HMI_SCADAInitiatingAlarmSubscription = 0x13,

        /// <summary>
        /// Show notification
        /// </summary>
        ShowNotify16 = 0x16,
    }

}
