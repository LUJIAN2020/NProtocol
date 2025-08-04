using NProtocol.Communication.Extensions;
using NProtocol.Protocols.S7.Extensions;
using NProtocol.Protocols.S7.StructType.S7DateTime;
using Robot.Communication.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NProtocol.Protocols.S7.StructType
{
    public class S7StructType
    {
        private static TValue GetValueOrThrow<TValue>(FieldInfo fi, object structValue) where TValue : struct
        {
            if (fi.GetValue(structValue) is TValue value)
                return value;
            throw new ArgumentException($"Failed to convert value of field {fi.Name} of {structValue} to type {typeof(TValue)}");
        }
        public static int GetStructSize(Type structType)
        {
            double byteCount = 0.0;
            var infos = structType.GetFields();
            foreach (var info in infos)
            {
                switch (info.FieldType.Name)
                {
                    case nameof(Boolean):
                        {
                            byteCount += 0.125;
                            break;
                        }
                    case nameof(Byte):
                        {
                            byteCount = Math.Ceiling(byteCount);
                            byteCount++;
                            break;
                        }
                    case nameof(Int16):
                    case nameof(UInt16):
                        {
                            byteCount = Math.Ceiling(byteCount);
                            if ((byteCount / 2 - Math.Floor(byteCount / 2.0)) > 0)
                                byteCount++;
                            byteCount += 2;
                            break;
                        }
                    case nameof(Int32):
                    case nameof(UInt32):
                    case nameof(Single):
                        {
                            byteCount = Math.Ceiling(byteCount);
                            if ((byteCount / 2 - Math.Floor(byteCount / 2.0)) > 0)
                                byteCount++;
                            byteCount += 4;
                            break;
                        }
                    case nameof(Double):
                        {
                            byteCount = Math.Ceiling(byteCount);
                            if ((byteCount / 2 - Math.Floor(byteCount / 2.0)) > 0)
                                byteCount++;
                            byteCount += 8;
                            break;
                        }
                    case nameof(String):
                        {
                            var attribute = info.GetCustomAttributes<S7StringAttribute>().SingleOrDefault();
                            if (attribute == default(S7StringAttribute))
                                throw new ArgumentException("Please add S7StringAttribute to the field");

                            byteCount = Math.Ceiling(byteCount);
                            if ((byteCount / 2 - Math.Floor(byteCount / 2.0)) > 0)
                                byteCount++;
                            byteCount += attribute.ReservedLengthInBytes;
                            break;
                        }
                    case nameof(TimeSpan):
                        {
                            var attribute = info.GetCustomAttributes<S7TimeAttribute>().SingleOrDefault();
                            if (attribute == default(S7TimeAttribute))
                                throw new ArgumentException("Please add S7TimeAttribute to the field");

                            byteCount = Math.Ceiling(byteCount);
                            if ((byteCount / 2 - Math.Floor(byteCount / 2.0)) > 0)
                                byteCount++;
                            byteCount += attribute.Size;
                            break;
                        }
                    case nameof(DateTime):
                        {
                            var attribute = info.GetCustomAttributes<S7DateTimeAttribute>().SingleOrDefault();
                            if (attribute == default(S7DateTimeAttribute))
                                throw new ArgumentException("Please add S7DateTimeAttribute to the field");

                            byteCount = Math.Ceiling(byteCount);
                            if ((byteCount / 2 - Math.Floor(byteCount / 2.0)) > 0)
                                byteCount++;
                            byteCount += attribute.Size;
                            break;
                        }
                    default:
                        if (info.FieldType.Name == nameof(Int64) || info.FieldType.Name == nameof(UInt64) || !info.FieldType.IsValueType)
                            throw new ArgumentException("Unsupported data types", info.FieldType.Name);
                        byteCount += GetStructSize(info.FieldType);
                        break;
                }
            }
            return (int)byteCount;
        }
        public static object ToStruct(Type type, byte[] bytes)
        {
            if (bytes is null) throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length != GetStructSize(type))
                throw new ArgumentOutOfRangeException(nameof(bytes.Length), bytes.Length, "The array length does not match the struct length.");
            double byteCount = 0.0;
            var structValue = Activator.CreateInstance(type);
            var infos = structValue.GetType().GetFields();
            foreach (var info in infos)
            {
                switch (info.FieldType.Name)
                {
                    case nameof(Boolean): //Bool
                        {
                            int bytePos = (int)Math.Floor(byteCount);
                            int bitPos = (int)((byteCount - bytePos) / 0.125);
                            if ((bytes[bytePos] & (int)Math.Pow(2, bitPos)) != 0)
                                info.SetValue(structValue, true);
                            else
                                info.SetValue(structValue, false);
                            byteCount += 0.125;
                            break;
                        }
                    case nameof(Byte)://Byte USInt
                        {
                            byteCount = Math.Ceiling(byteCount);
                            info.SetValue(structValue, bytes[(int)byteCount]);
                            byteCount++;
                            break;
                        }
                    case nameof(Int16)://Int
                        {
                            byteCount = Math.Ceiling(byteCount);
                            if (byteCount / 2 - Math.Floor(byteCount / 2.0) > 0)
                                byteCount++;
                            var source = new byte[2] { bytes[(int)byteCount], bytes[(int)byteCount + 1] };
                            info.SetValue(structValue, source.ToInt16(true));
                            byteCount += 2;
                            break;
                        }
                    case nameof(UInt16)://Word UInt
                        {
                            byteCount = Math.Ceiling(byteCount);
                            if (byteCount / 2 - Math.Floor(byteCount / 2.0) > 0)
                                byteCount++;
                            var source = new byte[2] { bytes[(int)byteCount], bytes[(int)byteCount + 1] };
                            info.SetValue(structValue, source.ToUInt16());
                            byteCount += 2;
                            break;
                        }
                    case nameof(Int32)://DInt 
                        {
                            byteCount = Math.Ceiling(byteCount);
                            if (byteCount / 2 - Math.Floor(byteCount / 2.0) > 0)
                                byteCount++;
                            var source = new byte[4] {
                            bytes[(int)byteCount + 0],
                            bytes[(int)byteCount + 1],
                            bytes[(int)byteCount + 2],
                            bytes[(int)byteCount + 3] };
                            info.SetValue(structValue, source.ToInt32());
                            byteCount += 4;
                            break;
                        }
                    case nameof(UInt32)://DWord
                        {
                            byteCount = Math.Ceiling(byteCount);
                            if (byteCount / 2 - Math.Floor(byteCount / 2.0) > 0)
                                byteCount++;
                            var source = new byte[4] {
                            bytes[(int)byteCount + 0],
                            bytes[(int)byteCount + 1],
                            bytes[(int)byteCount + 2],
                            bytes[(int)byteCount + 3] };
                            info.SetValue(structValue, source.ToUInt32());
                            byteCount += 4;
                            break;
                        }
                    case nameof(Single)://Real
                        {
                            byteCount = Math.Ceiling(byteCount);
                            if (byteCount / 2 - Math.Floor(byteCount / 2.0) > 0)
                                byteCount++;
                            var source = new byte[4] {
                            bytes[(int)byteCount + 0],
                            bytes[(int)byteCount + 1],
                            bytes[(int)byteCount + 2],
                            bytes[(int)byteCount + 3] };
                            info.SetValue(structValue, source.ToFloat());
                            byteCount += 4;
                            break;
                        }
                    case nameof(Double)://LReal
                        {
                            byteCount = Math.Ceiling(byteCount);
                            if (byteCount / 2 - Math.Floor(byteCount / 2.0) > 0)
                                byteCount++;
                            var data = new byte[8];
                            Array.Copy(bytes, (int)byteCount, data, 0, 8);
                            info.SetValue(structValue, data.ToDouble());
                            byteCount += 8;
                            break;
                        }
                    case nameof(String)://string wstring
                        {
                            var attribute = info.GetCustomAttributes<S7StringAttribute>().FirstOrDefault()
                                ?? throw new ArgumentException("Please add S7StringAttribute to the field");

                            byteCount = Math.Ceiling(byteCount);
                            if (byteCount / 2 - Math.Floor(byteCount / 2.0) > 0)
                                byteCount++;
                            var sData = new byte[attribute.ReservedLengthInBytes];
                            Array.Copy(bytes, (int)byteCount, sData, 0, sData.Length);
                            switch (attribute.Type)
                            {
                                case S7StringType.S7String:
                                    info.SetValue(structValue, sData.ToS7String());
                                    break;
                                case S7StringType.S7WString:
                                    info.SetValue(structValue, sData.ToS7WString());
                                    break;
                                default:
                                    throw new ArgumentException("Please use a valid string type for the S7StringAttribute");
                            }
                            byteCount += sData.Length;
                            break;
                        }
                    case nameof(TimeSpan)://TimeSpan
                        {
                            byteCount = Math.Ceiling(byteCount);
                            if (byteCount / 2 - Math.Floor(byteCount / 2.0) > 0)
                                byteCount++;
                            var attribute = info.GetCustomAttributes<S7TimeAttribute>().FirstOrDefault()
                                ?? throw new ArgumentException("Please add S7TimeAttribute to the field");

                            byte[] source = new byte[attribute.Size];
                            Buffer.BlockCopy(bytes, (int)byteCount, source, 0, source.Length);
                            var timeSpan = TimeSpan.MinValue;
                            switch (attribute.Type)
                            {
                                case S7TimeType.Time:
                                    {
                                        timeSpan = source.ToTime();
                                        break;
                                    }
                                case S7TimeType.LTime:
                                    {
                                        timeSpan = source.ToLTime();
                                        break;
                                    }
                                case S7TimeType.TimeOfDay:
                                    {
                                        timeSpan = source.ToTimeOfDay();
                                        break;
                                    }
                                case S7TimeType.LTimeOfDay:
                                    {
                                        timeSpan = source.ToLTimeOfDay();
                                        break;
                                    }
                                default:
                                    throw new ArgumentException("Please use a valid type for the S7TimeAttribute");
                            }
                            info.SetValue(structValue, timeSpan);
                            byteCount += attribute.Size;
                            break;
                        }
                    case nameof(DateTime)://DateTime
                        {
                            byteCount = Math.Ceiling(byteCount);
                            if (byteCount / 2 - Math.Floor(byteCount / 2.0) > 0)
                                byteCount++;

                            var attribute = info.GetCustomAttributes<S7DateTimeAttribute>().FirstOrDefault()
                                ?? throw new ArgumentException("Please add S7DateTimeAttribute to the field");

                            byte[] source = new byte[attribute.Size];
                            Buffer.BlockCopy(bytes, (int)byteCount, source, 0, source.Length);
                            var dateTime = DateTime.MinValue;
                            dateTime = attribute.Type switch
                            {
                                S7DateTimeType.Date => source.ToDate(),
                                S7DateTimeType.DateAndTime => source.ToDateTime(),
                                S7DateTimeType.DTL => source.ToDtl(),
                                _ => throw new ArgumentException("Please use a valid type for the S7DateTimeAttribute"),
                            };
                            info.SetValue(structValue, dateTime);
                            byteCount += attribute.Size;
                            break;
                        }
                    default:
                        {
                            if (!info.FieldType.IsValueType || info.FieldType.Name == nameof(Int64) || info.FieldType.Name == nameof(UInt64))
                                throw new ArgumentException("不支持的数据类型", info.FieldType.Name);

                            var buffer = new byte[GetStructSize(info.FieldType)];
                            if (buffer.Length == 0)
                                continue;
                            Buffer.BlockCopy(bytes, (int)Math.Ceiling(byteCount), buffer, 0, buffer.Length);
                            info.SetValue(structValue, ToStruct(info.FieldType, buffer));
                            byteCount += buffer.Length;
                            break;
                        }
                }
            }
            return structValue;
        }
        public static byte[] ToBytes(object structValue)
        {
            var type = structValue.GetType();

            if (type.IsPrimitive || type.IsEnum || !type.IsValueType)
                throw new ArgumentException("Type is not struct.", nameof(structValue));

            int size = GetStructSize(type);
            byte[] bytes = new byte[size];
            int byteOffset;
            int bitOffset;
            double byteCount = 0.0;
            var infos = type.GetFields();
            foreach (var info in infos)
            {
                byte[]? bytes2 = null;
                switch (info.FieldType.Name)
                {
                    case nameof(Boolean):
                        {
                            byteOffset = (int)Math.Floor(byteCount);
                            bitOffset = (int)((byteCount - byteOffset) / 0.125);
                            if (GetValueOrThrow<bool>(info, structValue))
                                bytes[byteOffset] |= (byte)Math.Pow(2, bitOffset);            // is true
                            else
                                bytes[byteOffset] &= (byte)~(byte)Math.Pow(2, bitOffset);   // is false
                            byteCount += 0.125;
                            break;
                        }
                    case nameof(Byte):
                        {
                            byteCount = (int)Math.Ceiling(byteCount);
                            byteOffset = (int)byteCount;
                            bytes[byteOffset] = GetValueOrThrow<byte>(info, structValue);
                            byteCount++;
                            break;
                        }
                    case nameof(Int16):
                        {
                            bytes2 = GetValueOrThrow<short>(info, structValue).ToBytes(false);
                            break;
                        }
                    case nameof(UInt16):
                        {
                            bytes2 = GetValueOrThrow<ushort>(info, structValue).ToBytes(false);
                            break;
                        }
                    case nameof(Int32):
                        {
                            bytes2 = GetValueOrThrow<int>(info, structValue).ToBytes(false);
                            break;
                        }
                    case nameof(UInt32):
                        {
                            bytes2 = GetValueOrThrow<uint>(info, structValue).ToBytes(false);
                            break;
                        }
                    case nameof(Single):
                        {
                            bytes2 = GetValueOrThrow<float>(info, structValue).ToBytes(false);
                            break;
                        }
                    case nameof(Double):
                        {
                            bytes2 = GetValueOrThrow<double>(info, structValue).ToBytes(false);
                            break;
                        }
                    case nameof(String):
                        {
                            var attribute = info.GetCustomAttributes<S7StringAttribute>().SingleOrDefault()
                                ?? throw new ArgumentException("Please add S7StringAttribute to the field");

                            var obj = info.GetValue(structValue)
                                ?? throw new ArgumentNullException(nameof(structValue));

                            string content = (string)obj;
                            bytes2 = attribute.Type switch
                            {
                                S7StringType.S7String => content.ToBytesFromS7String(attribute.ReservedLength),
                                S7StringType.S7WString => content.ToBytesFromS7WString(attribute.ReservedLength),
                                _ => throw new ArgumentException("Please use a valid type for the S7StringAttribute"),
                            };
                            break;
                        }
                    case nameof(TimeSpan):
                        {
                            var attribute = info.GetCustomAttributes<S7TimeAttribute>().SingleOrDefault()
                                ?? throw new ArgumentException("Please add S7TimeAttribute to the field");

                            var ts = GetValueOrThrow<TimeSpan>(info, structValue);
                            bytes2 = attribute.Type switch
                            {
                                S7TimeType.Time => ts.ToBytesFromTime(false),
                                S7TimeType.TimeOfDay => ts.ToBytesFromTimeOfDay(false),
                                S7TimeType.LTime => ts.ToBytesFromLTime(false),
                                S7TimeType.LTimeOfDay => ts.ToBytesFromLTimeOfDay(false),
                                _ => throw new ArgumentException("Please use a valid type for the S7TimeAttribute"),
                            };
                            break;
                        }
                    case nameof(DateTime):
                        {
                            var attribute = info.GetCustomAttributes<S7DateTimeAttribute>().SingleOrDefault()
                                ?? throw new ArgumentException("Please add S7DateTimeAttribute to the field");

                            var dt = GetValueOrThrow<DateTime>(info, structValue);
                            bytes2 = attribute.Type switch
                            {
                                S7DateTimeType.Date => dt.ToBytesFromDate(false),
                                S7DateTimeType.DateAndTime => dt.ToBytesFromDateAndTime(),
                                S7DateTimeType.DTL => dt.ToBytesFromDtl(false),
                                _ => throw new ArgumentException("Please use a valid type for the S7DateTimeAttribute"),
                            };
                            break;
                        }
                }
                if (bytes2 != null)
                {
                    byteCount = Math.Ceiling(byteCount);
                    if (byteCount / 2 - Math.Floor(byteCount / 2.0) > 0)
                        byteCount++;
                    byteOffset = (int)byteCount;
                    for (int bCnt = 0; bCnt < bytes2.Length; bCnt++)
                        bytes[byteOffset + bCnt] = bytes2[bCnt];
                    byteCount += bytes2.Length;
                }
            }
            return bytes;
        }
    }
}