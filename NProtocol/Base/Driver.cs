using NProtocol.Communication.Connectors;
using NProtocol.Communication.Enums;
using NProtocol.Communication.Exceptions;
using NProtocol.Communication.Extensions;
using NProtocol.Enums;
using System;
using System.Threading;

namespace NProtocol.Communication.Base
{
    public abstract class DriverBase : IDriver
    {
        private int readTimeout = 1000;
        private int writeTimeout = 1000;
        private readonly object _lock = new();
        private readonly IConnector connecter;
        /// <summary>
        /// Records read/write raw message data, performance loss, do not enable if not necessary
        /// </summary>
        public event Action<string, RW, byte[]>? LogReadWriteRaw;
        public DriverBase(IParameter parameter, ConnectMode mode)
        {
            connecter = CreateConnector(parameter, mode);
            Parameter = parameter;
            ConnectMode = mode;
        }
        public IParameter Parameter { get; }
        public ConnectMode ConnectMode { get; }
        private IConnector CreateConnector(IParameter parameter, ConnectMode mode) => mode switch
        {
            ConnectMode.SerialPort => new SerialPortConnector(parameter),
            ConnectMode.Tcp => new TcpConnector(parameter),
            ConnectMode.Udp => new UdpConnector(parameter),
            _ => throw new ArgumentException($"Unsupported connection types `{mode}`"),
        };
        /// <summary>
        /// The size of the data cache received
        /// </summary>
        public int ReceivedBufferSize { get; set; } = 1024 * 1024;
        public bool Connected => connecter.Connected;
        /// <summary>
        /// Drive ID
        /// Serial port communication is usually a serial port name
        /// Network communications are generally local and remote IP addresses and port numbers
        /// </summary>
        public string DriverId => connecter.DriverId;
        public int ReadTimeout
        {
            get { return readTimeout; }
            set
            {
                readTimeout = value;
                if (connecter != null)
                {
                    connecter.ReadTimeout = value;
                }
            }
        }
        public int WriteTimeout
        {
            get { return writeTimeout; }
            set
            {
                writeTimeout = value;
                if (connecter != null)
                {
                    connecter.WriteTimeout = value;
                }
            }
        }
        /// <summary>
        /// Extract payload data and return the read data by default. If you need to extract payload data, rewrite this method
        /// This method can also be used for data verification, such as CRC verification, by returning null to indicate that the data verification fails
        /// </summary>
        /// <param name="writeData">Write data</param>
        /// <param name="readData">Read data</param>
        /// <returns></returns>
        protected abstract byte[]? ExtractPayload(byte[] writeData, byte[] readData);
        /// <summary>
        /// Join the team and execute
        /// </summary>
        /// <param name="action"></param>
        protected void EnqueueExecute(Action action)
        {
            lock (_lock)
            {
                action.Invoke();
            }
        }
        /// <summary>
        /// Join the team and execute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected T EnqueueExecute<T>(Func<T> func)
        {
            lock (_lock)
            {
                return func.Invoke();
            }
        }
        /// <summary>
        /// The command is not locked
        /// </summary>
        /// <param name="writeData"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected Result NoLockExecute(byte[] writeData)
        {
            var result = new Result()
            {
                SendData = writeData,
            };
            Write(writeData);
            int offset = 0;
            var data = new byte[ReceivedBufferSize];
            while (true)
            {
                var buffer = Read();
                if (buffer.Length > 0)
                {
                    Array.Copy(buffer, 0, data, offset, buffer.Length);
                    offset += buffer.Length;
                    var readData = data.Slice(0, offset);
                    var payload = ExtractPayload(writeData, readData);
                    if (payload != null && payload.Length > 0)
                    {
                        result.ReceivedData = readData;
                        result.Payload = payload;
                        result.EndTime = DateTime.Now;
                        return result;
                    }
                }
                else
                {
                    Thread.Sleep(1);
                }
                ThrowLoopTimeoutException(result.StartTime);
            }
        }
        /// <summary>
        /// An exception is thrown for timeout
        /// </summary>
        /// <param name="inLoopTime"></param>
        /// <exception cref="LoopTimeoutException"></exception>
        private void ThrowLoopTimeoutException(DateTime inLoopTime)
        {
            if ((DateTime.Now - inLoopTime).TotalMilliseconds > ReadTimeout)
            {
                DiscardBuffer();
                throw new LoopTimeoutException("Loop read data timed out.", DriverId);
            }
        }
        /// <summary>
        /// Execute an unconditional command not locked
        /// </summary>
        /// <param name="writeData"></param>
        /// <returns></returns>
        protected Result NoLockExecuteNoResponse(byte[] writeData)
        {
            var result = new Result
            {
                SendData = writeData,
                StartTime = DateTime.Now
            };
            connecter.Write(writeData);
            result.EndTime = DateTime.Now;
            return result;
        }
        public void Connect()
        {
            connecter.Connect();
        }
        public void Close()
        {
            connecter.Close();
        }
        public void Dispose()
        {
            connecter.Dispose();
        }
        public int Write(byte[] writeData)
        {
            LogReadWriteRaw?.Invoke(DriverId, RW.W, writeData);
            return connecter.Write(writeData);
        }
        public byte[] Read()
        {
            var buffer = connecter.Read();
            LogReadWriteRaw?.Invoke(DriverId, RW.R, buffer);
            return buffer;
        }
        public void DiscardBuffer(int timeout = 100)
        {
            connecter.DiscardBuffer(timeout);
        }
    }
}