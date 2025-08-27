using NProtocol.Base;
using NProtocol.Connectors;
using NProtocol.Enums;
using NProtocol.Extensions;
using System;
using System.IO.Ports;
using System.Text;

namespace NProtocol.Protocols.Nano
{
    public class NanoClient : DriverBase
    {
        private byte[] @M => DefaultEncoding.GetBytes("?M\r");
        private byte[] ER => DefaultEncoding.GetBytes("ER\r");
        private byte[] @E => DefaultEncoding.GetBytes("?E\r");
        private byte[] @K => DefaultEncoding.GetBytes("?K\r");
        public NanoClient(EtherNetParameter parameter, ConnectMode mode) : base(parameter, mode)
        {
            if (mode == ConnectMode.SerialPort)
                throw new ArgumentException("EtherNet not supported serial port");
        }
        public NanoClient(string ip, ushort port, ConnectMode mode) : this(EtherNetParameter.Create(ip, port), mode)
        {
        }
        public NanoClient(SerialPortParameter parameter) : base(parameter, ConnectMode.SerialPort)
        {
        }
        public NanoClient(string portName, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One)
            : this(SerialPortParameter.Create(portName, baudRate, dataBits, parity, stopBits))
        {
        }
        protected override byte[]? ExtractPayload(byte[] writeData, byte[] readData)
        {
            int rlen = readData.Length;
            if (rlen > 2 && readData[rlen - 2] == '\r' && readData[rlen - 1] == '\n')
            {
                return readData;
            }
            return default;
        }
        public Encoding DefaultEncoding { get; set; } = Encoding.ASCII;
        public Result ChangeMode(RunMode mode)
        {
            string cmd = $"M{(int)mode}\r";
            var buffer = DefaultEncoding.GetBytes(cmd);
            var result = NoLockExecute(buffer);
            var res = DefaultEncoding.GetString(result.ReceivedData).Trim();
            return res switch
            {
                "OK" => result,
                "E1" => throw new Exception("Command error `E1`"),
                "E2" => throw new Exception("No program loaded `E2`"),
                "E5" => throw new Exception("Unit error `E5`"),
                _ => throw new Exception($"Unknown error `{res}`"),
            };
        }
        public Result<RunMode> QueryRunMode()
        {
            var result = NoLockExecute(@M);
            var res = DefaultEncoding.GetString(result.ReceivedData).Trim();
            if (ushort.TryParse(res, out ushort mode))
            {
                return result.ToResult((RunMode)mode);
            }
            else if (res == "E1")
            {
                throw new Exception("Command error `E1`");
            }
            else
            {
                throw new Exception($"Unknown error `{res}`");
            }
        }
        public Result ClearError()
        {
            var result = NoLockExecute(ER);
            var res = DefaultEncoding.GetString(result.ReceivedData).Trim();
            return res switch
            {
                "OK" => result,
                "E1" => throw new Exception("Command error `E1`"),
                _ => throw new Exception($"Unknown error `{res}`"),
            };
        }
        public Result<ushort> GetErrorCode()
        {
            var result = NoLockExecute(@E);
            var res = DefaultEncoding.GetString(result.ReceivedData).Trim();
            if (ushort.TryParse(res, out ushort code))
            {
                return result.ToResult(code);
            }
            else if (res == "E1")
            {
                throw new Exception("Command error `E1`");
            }
            else
            {
                throw new Exception($"Unknown error `{res}`");
            }
        }
        public Result<PlcType> QueryPlcType()
        {
            var result = NoLockExecute(@K);
            var res = DefaultEncoding.GetString(result.ReceivedData).Trim();
            if (ushort.TryParse(res, out ushort type))
            {
                return result.ToResult((PlcType)type);
            }
            else if (res == "E1")
            {
                throw new Exception("Command error `E1`");
            }
            else
            {
                throw new Exception($"Unknown error `{res}`");
            }
        }
        public Result SetDateTime(DateTime dt)
        {
            string cmd = $"WRT {dt.Year:00} {dt.Month:00} {dt.Day:00} {dt.Hour:00} {dt.Minute:00} {dt.Second:00} {dt.DayOfWeek}\r";
            var buffer = DefaultEncoding.GetBytes(cmd);
            var result = NoLockExecute(buffer);
            var res = DefaultEncoding.GetString(result.ReceivedData).Trim();
            return res switch
            {
                "OK" => result,
                "E1" => throw new Exception("Command error `E1`"),
                _ => throw new Exception($"Unknown error `{res}`"),
            };
        }
        public Result ForceSet(string address, bool onOff)
        {
            string cmd = $"{(onOff ? "ST" : "RS")} {address}\r";
            var buffer = DefaultEncoding.GetBytes(cmd);
            var result = NoLockExecute(buffer);
            var res = DefaultEncoding.GetString(result.ReceivedData).Trim();
            return res switch
            {
                "OK" => result,
                "E0" => throw new Exception("Soft device number error `E0`"),
                "E1" => throw new Exception("Command error `E1`"),
                _ => throw new Exception($"Unknown error `{res}`"),
            };
        }
        public Result ForceSets(string address, ushort count, bool onOff)
        {
            string cmd = $"{(onOff ? "STS" : "RSS")} {address} {count}\r";
            var buffer = DefaultEncoding.GetBytes(cmd);
            var result = NoLockExecute(buffer);
            var res = DefaultEncoding.GetString(result.ReceivedData).Trim();
            return res switch
            {
                "OK" => result,
                "E0" => throw new Exception("Soft device number error `E0`"),
                "E1" => throw new Exception("Command error `E1`"),
                _ => throw new Exception($"Unknown error `{res}`"),
            };
        }
        public Result<T[]> ReadValues<T>(string address, ushort count)
        {
            if (count < 1) throw new ArgumentOutOfRangeException(nameof(count), count, "Read count must be greater than 1");

            string cmd = $"RDS {address} {count}\r";
            var buffer = DefaultEncoding.GetBytes(cmd);
            var result = NoLockExecute(buffer);
            var res = DefaultEncoding.GetString(result.ReceivedData).Trim();
            if (res == "E0")
                throw new Exception("Soft device number error `E0`");

            if (res == "E1")
                throw new Exception("Command error `E1`");

            var values = ReadValues<T>(address, count, res);
            return result.ToResult(values);
        }
        private T[] ReadValues<T>(string address, ushort count, string res)
        {
            var addr = new NanoAddress(address);
            switch (addr.SoftDeviceType)
            {
                case SoftDeviceType.R:
                case SoftDeviceType.B:
                case SoftDeviceType.MR:
                case SoftDeviceType.LR:
                case SoftDeviceType.CR:
                case SoftDeviceType.VB:
                    {
                        var type = typeof(T);
                        if (type.Name != "Boolean")
                            throw new Exception($"`{addr.SoftDeviceType}` only bit operations are supported");

                        var values = res.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (count != values.Length)
                            throw new ArgumentOutOfRangeException(nameof(count), count, $"The read data count `{count}` does not match the actual returned data count `{values.Length}`");

                        var bs = new bool[values.Length];
                        for (int i = 0; i < bs.Length; i++)
                        {
                            bs[i] = Convert.ToByte(values[i].Trim()) == 1;
                        }
                        return (T[])(object)bs;
                    }
                case SoftDeviceType.DM:
                case SoftDeviceType.EM:
                case SoftDeviceType.FM:
                case SoftDeviceType.ZF:
                case SoftDeviceType.W:
                case SoftDeviceType.TM:
                case SoftDeviceType.Z:
                case SoftDeviceType.CM:
                case SoftDeviceType.VM:
                    {
                        var type = typeof(T);
                        if (type.Name != "UInt16")
                            throw new Exception($"`{addr.SoftDeviceType}` only uint16 operations are supported");

                        var values = res.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (count != values.Length)
                            throw new ArgumentOutOfRangeException(nameof(count), count, $"The read data count `{count}` does not match the actual returned data count `{values.Length}`");

                        var vals = new ushort[values.Length];
                        for (int i = 0; i < vals.Length; i++)
                        {
                            vals[i] = Convert.ToUInt16(values[i].Trim());
                        }
                        return (T[])(object)vals;
                    }
                case SoftDeviceType.T:
                case SoftDeviceType.TC:
                case SoftDeviceType.TS:
                case SoftDeviceType.C:
                case SoftDeviceType.CC:
                case SoftDeviceType.CS:
                case SoftDeviceType.AT:
                case SoftDeviceType.CTC:
                case SoftDeviceType.CTH:
                    {
                        var type = typeof(T);
                        if (type.Name != "Int32")
                            throw new Exception($"`{addr.SoftDeviceType}` only int32 operations are supported");

                        var values = res.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (count != values.Length)
                            throw new ArgumentOutOfRangeException(nameof(count), count, $"The read data count `{count}` does not match the actual returned data count `{values.Length}`");

                        var vals = new uint[values.Length];
                        for (int i = 0; i < vals.Length; i++)
                        {
                            vals[i] = Convert.ToUInt32(values[i].Trim());
                        }
                        return (T[])(object)vals;
                    }
                default:
                    throw new Exception($"Unsupported SoftDeviceType `{addr.SoftDeviceType}`");
            }
        }
        public Result<T> ReadValue<T>(string address)
        {
            string cmd = $"RD {address} \r";
            var buffer = DefaultEncoding.GetBytes(cmd);
            var result = NoLockExecute(buffer);
            var res = DefaultEncoding.GetString(result.ReceivedData).Trim();
            if (res == "E0")
                throw new Exception("Soft device number error `E0`");

            if (res == "E1")
                throw new Exception("Command error `E1`");

            var value = ReadValue<T>(address, res);
            return result.ToResult(value);
        }
        private T ReadValue<T>(string address, string res)
        {
            var addr = new NanoAddress(address);
            switch (addr.SoftDeviceType)
            {
                case SoftDeviceType.R:
                case SoftDeviceType.B:
                case SoftDeviceType.MR:
                case SoftDeviceType.LR:
                case SoftDeviceType.CR:
                case SoftDeviceType.VB:
                    {
                        var type = typeof(T);
                        if (type.Name != "Boolean")
                            throw new Exception($"`{addr.SoftDeviceType}` only bit operations are supported");

                        object b = res.Trim() == "1";
                        return (T)b;
                    }
                case SoftDeviceType.DM:
                case SoftDeviceType.EM:
                case SoftDeviceType.FM:
                case SoftDeviceType.ZF:
                case SoftDeviceType.W:
                case SoftDeviceType.TM:
                case SoftDeviceType.Z:
                case SoftDeviceType.CM:
                case SoftDeviceType.VM:
                    {
                        var type = typeof(T);
                        if (type.Name != "UInt16")
                            throw new Exception($"`{addr.SoftDeviceType}` only uint16 operations are supported");

                        object value = Convert.ToUInt16(res.Trim());
                        return (T)value;
                    }
                case SoftDeviceType.T:
                case SoftDeviceType.TC:
                case SoftDeviceType.TS:
                case SoftDeviceType.C:
                case SoftDeviceType.CC:
                case SoftDeviceType.CS:
                case SoftDeviceType.AT:
                case SoftDeviceType.CTC:
                case SoftDeviceType.CTH:
                    {
                        var type = typeof(T);
                        if (type.Name != "Int32")
                            throw new Exception($"`{addr.SoftDeviceType}` only int32 operations are supported");

                        object value = Convert.ToInt32(res.Trim());
                        return (T)value;
                    }
                default:
                    throw new Exception($"Unsupported SoftDeviceType `{addr.SoftDeviceType}`");
            }
        }
        public Result WriteValues<T>(string address, T[] writeValues)
        {
            var sb = new StringBuilder();
            sb.Append("WRS ")
                .Append(address)
                .Append(" ")
                .Append(writeValues.Length.ToString())
                .Append(" ");
            var addr = new NanoAddress(address);
            switch (addr.SoftDeviceType)
            {
                case SoftDeviceType.R:
                case SoftDeviceType.B:
                case SoftDeviceType.MR:
                case SoftDeviceType.LR:
                case SoftDeviceType.CR:
                case SoftDeviceType.VB:
                case SoftDeviceType.VM:
                    {
                        var type = typeof(T);
                        if (type.Name != "Boolean")
                            throw new Exception($"`{addr.SoftDeviceType}` only bit operations are supported");

                        for (int i = 0; i < writeValues.Length; i++)
                        {
                            if (writeValues[i] is bool b)
                            {
                                sb.Append(b ? "1" : "0");
                            }
                        }
                        break;
                    }
                case SoftDeviceType.DM:
                case SoftDeviceType.EM:
                case SoftDeviceType.FM:
                case SoftDeviceType.ZF:
                case SoftDeviceType.W:
                case SoftDeviceType.TM:
                case SoftDeviceType.Z:
                case SoftDeviceType.CM:
                    {
                        var type = typeof(T);
                        if (type.Name != "UInt16")
                            throw new Exception($"`{addr.SoftDeviceType}` only uint16 operations are supported");

                        for (int i = 0; i < writeValues.Length; i++)
                        {
                            if (writeValues[i] is ushort value)
                            {
                                sb.Append(value);
                            }
                        }
                        break;
                    }
                case SoftDeviceType.T:
                case SoftDeviceType.TC:
                case SoftDeviceType.TS:
                case SoftDeviceType.C:
                case SoftDeviceType.CC:
                case SoftDeviceType.CS:
                    {
                        var type = typeof(T);
                        if (type.Name != "Int32")
                            throw new Exception($"`{addr.SoftDeviceType}` only int32 operations are supported");

                        for (int i = 0; i < writeValues.Length; i++)
                        {
                            if (writeValues[i] is int value)
                            {
                                sb.Append(value);
                            }
                        }
                        break;
                    }
                case SoftDeviceType.CTH:
                case SoftDeviceType.CTC:
                case SoftDeviceType.AT:
                default:
                    throw new Exception($"`{addr.SoftDeviceType}` not supported");
            }
            sb.Append("\r\n");
            string cmd = sb.ToString();
            var buffer = DefaultEncoding.GetBytes(cmd);
            var result = NoLockExecute(buffer);
            var res = DefaultEncoding.GetString(result.ReceivedData).Trim();
            return res switch
            {
                "OK" => result,
                "E0" => throw new Exception("Soft device number error `E0`"),
                "E1" => throw new Exception("Command error `E1`"),
                "E4" => throw new Exception("Write prohibited `E1`"),
                _ => throw new Exception($"Unknown error `{res}`"),
            };
        }
        public Result WriteValue<T>(string address, T value)
        {
            var sb = new StringBuilder();
            sb.Append("WR ")
                .Append(address)
                .Append(" ");
            var addr = new NanoAddress(address);
            switch (addr.SoftDeviceType)
            {
                case SoftDeviceType.R:
                case SoftDeviceType.B:
                case SoftDeviceType.MR:
                case SoftDeviceType.LR:
                case SoftDeviceType.CR:
                case SoftDeviceType.VB:
                case SoftDeviceType.VM:
                    {
                        var type = typeof(T);
                        if (type.Name != "Boolean")
                            throw new Exception($"`{addr.SoftDeviceType}` only bit operations are supported");

                        if (value is bool b)
                        {
                            sb.Append(b ? "1" : "0");
                        }
                        break;
                    }
                case SoftDeviceType.DM:
                case SoftDeviceType.EM:
                case SoftDeviceType.FM:
                case SoftDeviceType.ZF:
                case SoftDeviceType.W:
                case SoftDeviceType.TM:
                case SoftDeviceType.Z:
                case SoftDeviceType.CM:
                    {
                        var type = typeof(T);
                        if (type.Name != "UInt16")
                            throw new Exception($"`{addr.SoftDeviceType}` only uint16 operations are supported");

                        if (value is ushort val)
                        {
                            sb.Append(val);
                        }
                        break;
                    }
                case SoftDeviceType.T:
                case SoftDeviceType.TC:
                case SoftDeviceType.TS:
                case SoftDeviceType.C:
                case SoftDeviceType.CC:
                case SoftDeviceType.CS:
                    {
                        var type = typeof(T);
                        if (type.Name != "Int32")
                            throw new Exception($"`{addr.SoftDeviceType}` only int32 operations are supported");

                        if (value is int val)
                        {
                            sb.Append(val);
                        }
                        break;
                    }
                case SoftDeviceType.CTH:
                case SoftDeviceType.CTC:
                case SoftDeviceType.AT:
                default:
                    throw new Exception($"`{addr.SoftDeviceType}` not supported");
            }
            sb.Append("\r\n");
            string cmd = sb.ToString();
            var buffer = DefaultEncoding.GetBytes(cmd);
            var result = NoLockExecute(buffer);
            var res = DefaultEncoding.GetString(result.ReceivedData).Trim();
            return res switch
            {
                "OK" => result,
                "E0" => throw new Exception("Soft device number error `E0`"),
                "E1" => throw new Exception("Command error `E1`"),
                "E4" => throw new Exception("Write prohibited `E1`"),
                _ => throw new Exception($"Unknown error `{res}`"),
            };
        }
    }
}
