using NProtocol.Connectors;
using NProtocol.Enums;
using NProtocol.Exceptions;
using System;
using System.Buffers;
using System.Threading;

namespace NProtocol.Base
{
    public delegate void LogReadWriteRawHandler(string driverId, RW rw, ReadOnlySpan<byte> data);
    public abstract class DriverBase : IDriver
    {
        private int readTimeout = 1000;
        private int writeTimeout = 1000;
        private readonly object _lock = new();
        private readonly IConnector connecter;

        /// <summary>
        /// Records read/write raw message data, performance loss, do not enable if not necessary
        /// </summary>
        public LogReadWriteRawHandler? LogReadWriteRaw { get; set; }

        public DriverBase(IParameter parameter, ConnectMode mode)
        {
            connecter = CreateConnector(parameter, mode);
            Parameter = parameter;
            ConnectMode = mode;
        }

        public IParameter Parameter { get; }
        public ConnectMode ConnectMode { get; }

        private IConnector CreateConnector(IParameter parameter, ConnectMode mode) =>
            mode switch
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
        /// Verify whether the received data is correct
        /// </summary>
        /// <param name="writeData">write data</param>
        /// <param name="readData">read data</param>
        /// <returns></returns>
        protected abstract bool ValidateReceivedData(ReadOnlySpan<byte> writeData, ReadOnlySpan<byte> readData);

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
            var result = new Result() { SendData = writeData };
            Write(writeData);
            int offset = 0;
            var rentBuf = ArrayPool<byte>.Shared.Rent(ReceivedBufferSize);
            var now = DateTime.Now;
            try
            {
                while (true)
                {
                    var buffer = Read();
                    if (buffer.Length > 0)
                    {
                        var span = rentBuf.AsSpan();
                        buffer.CopyTo(span.Slice(offset));
                        offset += buffer.Length;
                        var readData = span.Slice(0, offset);
                        if (ValidateReceivedData(writeData, readData))
                        {
                            result.ReceivedData = readData.ToArray();
                            return result;
                        }
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                    ThrowLoopTimeoutException(now);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rentBuf);
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
            var result = new Result { SendData = writeData };
            connecter.Write(writeData);
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

        public ReadOnlySpan<byte> Read()
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
