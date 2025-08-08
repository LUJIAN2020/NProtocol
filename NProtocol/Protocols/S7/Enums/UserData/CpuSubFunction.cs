namespace NProtocol.Protocols.S7.Enums
{
    public enum CpuSubFunction : byte
    {
        /// <summary>
        /// 读系统状态列表
        /// </summary>
        ReadSZL = 0x01,

        /// <summary>
        /// 消息服务
        /// </summary>
        MessageService = 0x02,

        /// <summary>
        /// 诊断消息
        /// </summary>
        DiagnosticMessage = 0x03,

        /// <summary>
        /// 显示警报
        /// </summary>
        ShowAlarm = 0x05,

        /// <summary>
        /// 显示通知
        /// </summary>
        ShowNotify = 0x06,

        /// <summary>
        /// 锁定报警
        /// </summary>
        LockingAlarm = 0x07,

        /// <summary>
        /// 取消锁定报警
        /// </summary>
        CancelLockingAlarm = 0x08,

        /// <summary>
        /// 显示扫描
        /// </summary>
        ShowScan = 0x09,

        /// <summary>
        /// 确认报警
        /// </summary>
        AlarmAcknowledged = 0x0B,

        /// <summary>
        /// 确认显示报警
        /// </summary>
        ShowAlarmAcknowledged = 0x0C,

        /// <summary>
        /// 锁定显示报警
        /// </summary>
        LockingShowAlarm = 0x0D,

        /// <summary>
        /// 取消锁定显示报警
        /// </summary>
        CancelLockingShowAlarm = 0x0E,

        /// <summary>
        /// 显示报警SQ
        /// </summary>
        PlcIndicatingAlarmMessage = 0x11,

        /// <summary>
        /// 显示报警S
        /// </summary>
        PlcIndicatingAlarmS = 0x12,

        /// <summary>
        /// 查询报警
        /// </summary>
        HMI_SCADAInitiatingAlarmSubscription = 0x13,

        /// <summary>
        /// 显示通知
        /// </summary>
        ShowNotify16 = 0x16,
    }
}
