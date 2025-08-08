using System.IO.Ports;

namespace NProtocol.Connectors
{
    public class SerialPortParameter : IParameter
    {
        public string PortName { get; set; } = string.Empty;
        public int BaudRate { get; set; } = 9600;
        public int DataBits { get; set; } = 8;
        public Parity Parity { get; set; } = Parity.None;
        public StopBits StopBits { get; set; } = StopBits.One;
        public ParameterType Type => ParameterType.SerialPort;

        public static SerialPortParameter Create(
            string portName,
            int baudRate = 9600,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One
        )
        {
            return new SerialPortParameter
            {
                PortName = portName,
                BaudRate = baudRate,
                DataBits = dataBits,
                Parity = parity,
                StopBits = stopBits,
            };
        }
    }
}
