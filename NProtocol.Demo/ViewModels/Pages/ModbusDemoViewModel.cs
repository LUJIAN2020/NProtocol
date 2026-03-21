using CommunityToolkit.Mvvm.ComponentModel;
using NProtocol.Connectors;
using NProtocol.Protocols.Modbus;
using System;
using System.IO.Ports;
using System.Linq;

namespace NProtocol.Demo.ViewModels.Pages
{
    public partial class ModbusDemoViewModel : ViewModelBase
    {
        private ModbusClient? client;
        [ObservableProperty] private string _connectStatus = "Connect";
        [ObservableProperty] private string _value = string.Empty;
        [ObservableProperty] private byte _selectedFuncCode;
        public string[] Modes => new string[] { "TCP", "UDP", "RTU Over TCP", "RTU Over UDP", "RTU" };
        public string[] PortNames => SerialPort.GetPortNames();
        public int[] BaudRates => new int[] { 1200, 2400, 4800, 9600, 14400, 19200, 38400, 56000, 57600, 115200, 194000 };
        public int[] DataBits => new int[] { 5, 6, 7, 8 };
        public string[] Paritys => Enum.GetNames(typeof(Parity));
        public string[] StopBits => Enum.GetNames(typeof(StopBits));
        public byte[] FuncCodes => new byte[] { 1, 2, 3, 4 };
        public void ConnectCommand(object[] args)
        {
            try
            {
                if (ConnectStatus == "Disconnect")
                {
                    client?.Dispose();
                    client = null;
                    ConnectStatus = "Connect";
                }
                else if (ConnectStatus == "Connect")
                {
                    if (args[0] is string mode)
                    {
                        switch (mode)
                        {
                            case "TCP":
                            case "UDP":
                            case "RTU Over TCP":
                            case "RTU Over UDP":
                                if (args[1] is string ip && args[2] is decimal port)
                                {
                                    if (ConnectStatus == "Connect")
                                    {
                                        client?.Dispose();
                                        client = null;
                                    }
                                    var eth = new EtherNetParameter() { IP = ip, Port = (ushort)port };
                                    var conMode = ToModbusConnectMode(mode);
                                    client = new ModbusClient(ip, (ushort)port, conMode);
                                }
                                break;
                            case "RTU":
                                if (args[3] is string portName &&
                                    args[4] is int baudRates &&
                                    args[5] is int dataBits &&
                                    args[6] is string paritys &&
                                    args[7] is string stopBits)
                                {
                                    var par = Enum.Parse<Parity>(paritys);
                                    var sb = Enum.Parse<StopBits>(stopBits);
                                    client = new ModbusClient(portName, baudRates, dataBits, par, sb);
                                }
                                break;
                            default:
                                throw new Exception($"Unsupported mode `{mode}`");
                        }
                    }

                    if (client != null)
                    {
                        client.Connect();
                        Info("Connect success");
                        ConnectStatus = "Disconnect";
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }
        private ModbusConnectMode ToModbusConnectMode(string mode)
        {
            return mode switch
            {
                "TCP" => ModbusConnectMode.Tcp,
                "UDP" => ModbusConnectMode.Udp,
                "RTU Over TCP" => ModbusConnectMode.RtuOverTcp,
                "RTU Over UDP" => ModbusConnectMode.RtuOverUdp,
                _ => throw new Exception($"Unsupported mode `{mode}`"),
            };
        }
        public void ReadCommand(object[] args)
        {
            try
            {
                if (args[0] is string fc && args[1] is decimal station && args[2] is decimal addr && args[3] is decimal cnt)
                {
                    if (client != null)
                    {
                        switch (fc)
                        {
                            case "ReadCoils":
                                {
                                    var result = client.ReadCoils((byte)station, (ushort)addr, (ushort)cnt);
                                    Value = string.Join(",", result.Value.Select(c => c.ToString()));
                                    Info(result.ToString());
                                    break;
                                }
                            case "ReadDiscreteInputs":
                                {
                                    var result = client.ReadDiscreteInputs((byte)station, (ushort)addr, (ushort)cnt);
                                    Value = string.Join(",", result.Value.Select(c => c.ToString()));
                                    Info(result.ToString());
                                    break;
                                }
                            case "ReadHoldingRegisters":
                                {
                                    var result = client.ReadHoldingRegisters<ushort>((byte)station, (ushort)addr, (byte)cnt, ByteFormat.AB);
                                    Value = string.Join(",", result.Value.Select(c => c.ToString()));
                                    Info(result.ToString());
                                    break;
                                }
                            case "ReadInputRegisters":
                                {
                                    var result = client.ReadInputRegisters<ushort>((byte)station, (ushort)addr, (byte)cnt, ByteFormat.AB);
                                    Value = string.Join(",", result.Value.Select(c => c.ToString()));
                                    Info(result.ToString());
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }
        public void WriteCommand(object[] args)
        {
            try
            {
                if (args[0] is string fc && args[1] is decimal station && args[2] is decimal addr && args[3] is string value)
                {
                    if (client != null)
                    {
                        var vals = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        switch (fc)
                        {
                            case "WriteSingleCoil":
                                {
                                    var data = Array.ConvertAll(vals, Convert.ToBoolean);
                                    var result = client.WriteSingleCoil((byte)station, (ushort)addr, data[0]);
                                    Info(result.ToString());
                                    break;
                                }
                            case "WriteMultipleCoils":
                                {
                                    var data = Array.ConvertAll(vals, Convert.ToBoolean);
                                    var result = client.WriteMultipleCoils((byte)station, (ushort)addr, data);
                                    Info(result.ToString());
                                    break;
                                }
                            case "WriteSingleRegister":
                                {
                                    var data = Array.ConvertAll(vals, Convert.ToUInt16);
                                    var result = client.WriteSingleRegister((byte)station, (ushort)addr, data[0]);
                                    Info(result.ToString());
                                    break;
                                }
                            case "WriteMultipleRegisters":
                                {
                                    var data = Array.ConvertAll(vals, Convert.ToUInt16);
                                    var result = client.WriteMultipleRegisters((byte)station, (ushort)addr, data);
                                    Info(result.ToString());
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }
    }
}
