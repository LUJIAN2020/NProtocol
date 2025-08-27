using System;

namespace NProtocol.Protocols.Nano
{
    public readonly struct NanoAddress
    {
        public NanoAddress(string addr)
        {
            ParseAddress(addr, out DataType dataType, out SoftDeviceType softDeviceType, out int deviceNo);
            DataType = dataType;
            SoftDeviceType = softDeviceType;
            DeviceNo = deviceNo;
        }
        public DataType DataType { get; }
        public SoftDeviceType SoftDeviceType { get; }
        public int DeviceNo { get; }
        public static void ParseAddress(string addr, out DataType dataType, out SoftDeviceType softDeviceType, out int deviceNo)
        {
            addr = addr.Trim().ToUpper();
            switch (addr[0])
            {
                case 'R':
                case 'B':
                    {
                        dataType = DataType.BIT;
                        softDeviceType = (SoftDeviceType)Enum.Parse(typeof(SoftDeviceType), addr[0].ToString());
                        if (int.TryParse(addr.Substring(1), out int val))
                        {
                            deviceNo = val;
                        }
                        else
                        {
                            throw new Exception($"Address formatter error `{addr}`");
                        }
                        break;
                    }
                case 'M':
                case 'L':
                case 'C':
                    {
                        if (addr[1] == 'R')
                        {
                            dataType = DataType.BIT;
                            softDeviceType = (SoftDeviceType)Enum.Parse(typeof(SoftDeviceType), addr.Substring(0, 2));
                            if (int.TryParse(addr.Substring(2), out int val))
                            {
                                deviceNo = val;
                            }
                            else
                            {
                                throw new Exception($"Address formatter error `{addr}`");
                            }
                        }
                        else if (addr[1] == 'M')
                        {
                            dataType = DataType.U;
                            softDeviceType = (SoftDeviceType)Enum.Parse(typeof(SoftDeviceType), addr.Substring(0, 2));
                            if (int.TryParse(addr.Substring(2), out int val))
                            {
                                deviceNo = val;
                            }
                            else
                            {
                                throw new Exception($"Address formatter error `{addr}`");
                            }
                        }
                        else
                        {
                            dataType = DataType.D;
                            softDeviceType = (SoftDeviceType)Enum.Parse(typeof(SoftDeviceType), addr[0].ToString());
                            if (int.TryParse(addr.Substring(1), out int val))
                            {
                                deviceNo = val;
                            }
                            else
                            {
                                throw new Exception($"Address formatter error `{addr}`");
                            }
                        }
                        break;
                    }
                case 'V':
                    {
                        if (addr[1] == 'B')
                        {
                            dataType = DataType.BIT;
                            softDeviceType = (SoftDeviceType)Enum.Parse(typeof(SoftDeviceType), addr.Substring(0, 2));
                            if (int.TryParse(addr.Substring(2), out int val))
                            {
                                deviceNo = val;
                            }
                            else
                            {
                                throw new Exception($"Address formatter error `{addr}`");
                            }
                        }
                        else if (addr[1] == 'M')
                        {
                            dataType = DataType.U;
                            softDeviceType = (SoftDeviceType)Enum.Parse(typeof(SoftDeviceType), addr.Substring(0, 2));
                            if (int.TryParse(addr.Substring(2), out int val))
                            {
                                deviceNo = val;
                            }
                            else
                            {
                                throw new Exception($"Address formatter error `{addr}`");
                            }
                        }
                        else
                        {
                            throw new Exception($"Unsupported formatter address `{addr}`");
                        }
                        break;
                    }
                case 'D':
                case 'E':
                case 'F':
                case 'T':
                    {
                        if (addr[1] == 'M')
                        {
                            dataType = DataType.U;
                            softDeviceType = (SoftDeviceType)Enum.Parse(typeof(SoftDeviceType), addr.Substring(0, 2));
                            if (int.TryParse(addr.Substring(2), out int val))
                            {
                                deviceNo = val;
                            }
                            else
                            {
                                throw new Exception($"Address formatter error `{addr}`");
                            }
                        }
                        else if (addr[1] == 'C' || addr[1] == 'S')
                        {
                            dataType = DataType.D;
                            softDeviceType = (SoftDeviceType)Enum.Parse(typeof(SoftDeviceType), addr.Substring(0, 2));
                            if (int.TryParse(addr.Substring(2), out int val))
                            {
                                deviceNo = val;
                            }
                            else
                            {
                                throw new Exception($"Address formatter error `{addr}`");
                            }
                        }
                        else
                        {
                            dataType = DataType.D;
                            softDeviceType = (SoftDeviceType)Enum.Parse(typeof(SoftDeviceType), addr.Substring(0, 1));
                            if (int.TryParse(addr.Substring(1), out int val))
                            {
                                deviceNo = val;
                            }
                            else
                            {
                                throw new Exception($"Address formatter error `{addr}`");
                            }
                        }
                        break;
                    }
                case 'Z':
                    {
                        if (addr[1] == 'F')
                        {
                            dataType = DataType.U;
                            softDeviceType = (SoftDeviceType)Enum.Parse(typeof(SoftDeviceType), addr.Substring(0, 2));
                            if (int.TryParse(addr.Substring(2), out int val))
                            {
                                deviceNo = val;
                            }
                            else
                            {
                                throw new Exception($"Address formatter error `{addr}`");
                            }
                        }
                        else
                        {
                            dataType = DataType.U;
                            softDeviceType = (SoftDeviceType)Enum.Parse(typeof(SoftDeviceType), addr.Substring(0, 1));
                            if (int.TryParse(addr.Substring(1), out int val))
                            {
                                deviceNo = val;
                            }
                            else
                            {
                                throw new Exception($"Address formatter error `{addr}`");
                            }
                        }
                        break;
                    }
                case 'W':
                    {
                        dataType = DataType.U;
                        softDeviceType = (SoftDeviceType)Enum.Parse(typeof(SoftDeviceType), addr.Substring(0, 1));
                        if (int.TryParse(addr.Substring(1), out int val))
                        {
                            deviceNo = val;
                        }
                        else
                        {
                            throw new Exception($"Address formatter error `{addr}`");
                        }
                        break;
                    }
                case 'A':
                    {
                        dataType = DataType.D;
                        softDeviceType = (SoftDeviceType)Enum.Parse(typeof(SoftDeviceType), addr.Substring(0, 2));
                        if (int.TryParse(addr.Substring(2), out int val))
                        {
                            deviceNo = val;
                        }
                        else
                        {
                            throw new Exception($"Address formatter error `{addr}`");
                        }
                        break;
                    }
                default:
                    throw new Exception($"Unsupported formatter address `{addr}`");
            }
        }
    }
}
