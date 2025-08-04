namespace NProtocol.Protocols.Modbus
{
    /// <summary>
    /// Connection mode
    /// </summary>
    public enum ModbusConnectMode : byte
    {
        /// <summary>
        /// Modbus-RTU serial port
        /// </summary>
        Rtu = 0,
        /// <summary>
        /// Modbus-RTU encapsulates TCP transmission over the network port
        /// </summary>
        RtuOverTcp,
        /// <summary>
        /// Modbus-RTU encapsulates the network port for UDP transmission
        /// </summary>
        RtuOverUdp,
        /// <summary>
        /// Modbus-TCP
        /// </summary>
        Tcp,
        /// <summary>
        /// Modbus-UDP
        /// </summary>
        Udp
    }
}
