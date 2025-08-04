using NProtocol.Communication.Connectors;
using NProtocol.Communication.Enums;
using System;
using System.IO.Ports;

namespace NProtocol.Protocols.Modbus
{
    public class ModbusClient : ModbusBase
    {
        public ModbusClient(SerialPortParameter parameter) : base(parameter, ConnectMode.SerialPort)
        {
            ModbusConnectMode = ModbusConnectMode.Rtu;
        }
        public ModbusClient(string portName, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One)
            : this(SerialPortParameter.Create(portName, baudRate, dataBits, parity, stopBits))
        {
            ModbusConnectMode = ModbusConnectMode.Rtu;
        }
        public ModbusClient(EtherNetParameter parameter, ModbusConnectMode mode) : base(parameter, GetConnectMode(mode))
        {
            ModbusConnectMode = mode;
        }
        public ModbusClient(string ip, ushort port, ModbusConnectMode mode) : this(EtherNetParameter.Create(ip, port), mode)
        {
        }
        private static ConnectMode GetConnectMode(ModbusConnectMode mode) => mode switch
        {
            ModbusConnectMode.RtuOverTcp or ModbusConnectMode.Tcp => ConnectMode.Tcp,
            ModbusConnectMode.RtuOverUdp or ModbusConnectMode.Udp => ConnectMode.Udp,
            _ => throw new ArgumentException($"Unsupported modbus connect mode `{mode}`"),
        };
    }
}