using System;
using System.Buffers;
using System.IO.Ports;

namespace NProtocol.Connectors
{
    public class SerialPortConnector : IConnector
    {
        private int writeTimeout = 1000;
        private int readTimeout = 1000;
        private readonly SerialPort serialPort = new();
        public SerialPortParameter Parameter { get; }
        public string DriverId => serialPort.PortName;
        public bool Connected => serialPort.IsOpen;
        public int ReadTimeout
        {
            get { return readTimeout; }
            set
            {
                readTimeout = value;
                if (serialPort != null)
                {
                    serialPort.ReadTimeout = value;
                }
            }
        }
        public int WriteTimeout
        {
            get { return writeTimeout; }
            set
            {
                writeTimeout = value;
                if (serialPort != null)
                {
                    serialPort.WriteTimeout = value;
                }
            }
        }

        public SerialPortConnector(IParameter parameter)
        {
            if (parameter is SerialPortParameter para)
            {
                Parameter = para;
                serialPort.PortName = para.PortName;
                serialPort.BaudRate = para.BaudRate;
                serialPort.Parity = para.Parity;
                serialPort.DataBits = para.DataBits;
                serialPort.StopBits = para.StopBits;
                serialPort.ReadTimeout = ReadTimeout;
                serialPort.WriteTimeout = WriteTimeout;
            }
            else
            {
                throw new ArgumentException(
                    "Type error. The parameter type is not the serial port parameter type.",
                    nameof(parameter)
                );
            }
        }

        public void Connect()
        {
            if (serialPort.IsOpen)
                return;
            serialPort.WriteTimeout = WriteTimeout;
            serialPort.ReadTimeout = ReadTimeout;
            serialPort.Open();
            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
        }

        public void Close()
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        public void Dispose()
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
            serialPort.Dispose();
        }

        public ReadOnlySpan<byte> Read()
        {
            Connect();
            int len = serialPort.BytesToRead;
            var rentBuf = ArrayPool<byte>.Shared.Rent(len);
            try
            {
                int rlen = serialPort.Read(rentBuf, 0, len);
                return rentBuf.AsSpan().Slice(0, rlen);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rentBuf);
            }
        }

        public int Write(byte[] buffer)
        {
            Connect();
            serialPort.Write(buffer, 0, buffer.Length);
            return buffer.Length;
        }

        public void DiscardBuffer(int timeout = 100)
        {
            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
        }
    }
}
