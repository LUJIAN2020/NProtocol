using NProtocol.Extensions;
using System;
using System.Linq;

namespace NProtocol.Protocols.S7.Extensions
{
    public static class S7TimeExtension
    {
        private static readonly DateTime iecMinDate = new(year: 1990, month: 01, day: 01);
        private static readonly DateTime iecMaxDate = new(year: 2169, month: 06, day: 06);
        private static readonly ushort maxNumberOfDays = (ushort)(iecMaxDate - iecMinDate).TotalDays;

        private static readonly TimeSpan minimumTimeSpan = TimeSpan.FromMilliseconds(int.MinValue);
        private static readonly TimeSpan maximumTimeSpan = TimeSpan.FromMilliseconds(int.MaxValue);

        private static readonly DateTime minimumDateTime = new(1990, 1, 1);
        private static readonly DateTime maximumDateTime = new(2089, 12, 31, 23, 59, 59, 999);

        public static double ToTimer(this byte[] bytes)
        {
            double wert = ((bytes[0]) & 0x0F) * 100.0;
            wert += ((bytes[1] >> 4) & 0x0F) * 10.0;
            wert += ((bytes[1]) & 0x0F) * 1.0;
            switch ((bytes[0] >> 4) & 0x03)
            {
                case 0:
                    wert *= 0.01;
                    break;
                case 1:
                    wert *= 0.1;
                    break;
                case 2:
                    wert *= 1.0;
                    break;
                case 3:
                    wert *= 10.0;
                    break;
            }
            return wert;
        }
        public static T AssertRangeInclusive<T>(T input, T min, T max, string field) where T : IComparable<T>
        {
            if (input.CompareTo(min) < 0 || input.CompareTo(max) > 0)
                throw new ArgumentOutOfRangeException(nameof(input), input,
                    $"Value '{input}' is lower than the minimum '{min}' maximum '{max}' allowed for {field}.");
            return input;
        }
        public static DateTime ToDateTime(this byte[] bytes)
        {
            if (bytes.Length != 8)
                throw new ArgumentOutOfRangeException(nameof(bytes), bytes.Length, "Range of 8 byte.");
            int DecodeBcd(byte input) => 10 * (input >> 4) + (input & 0x0F);
            int ByteToYear(byte bcdYear)
            {
                var input = DecodeBcd(bcdYear);
                if (input < 90) return input + 2000;
                if (input < 100) return input + 1900;
                throw new ArgumentOutOfRangeException(nameof(bcdYear), bcdYear, "Maximum '99' of S7 date and time representation.");
            }
            var year = ByteToYear(bytes[0]);
            var month = AssertRangeInclusive(DecodeBcd(bytes[1]), 1, 12, "month");
            var day = AssertRangeInclusive(DecodeBcd(bytes[2]), 1, 31, "day of month");
            var hour = AssertRangeInclusive(DecodeBcd(bytes[3]), 0, 23, "hour");
            var minute = AssertRangeInclusive(DecodeBcd(bytes[4]), 0, 59, "minute");
            var second = AssertRangeInclusive(DecodeBcd(bytes[5]), 0, 59, "second");
            var hsec = AssertRangeInclusive(DecodeBcd(bytes[6]), 0, 99, "first two millisecond digits");
            var msec = AssertRangeInclusive(bytes[7] >> 4, 0, 9, "third millisecond digit");
            var dayOfWeek = AssertRangeInclusive(bytes[7] & 0b00001111, 1, 7, "day of week");
            return new DateTime(year, month, day, hour, minute, second, hsec * 10 + msec);
        }
        public static DateTime ToDate(this byte[] bytes, bool isLittleEndian = true)
        {
            if (bytes.Length != 2)
                throw new ArgumentOutOfRangeException(nameof(bytes.Length), bytes.Length, "Range of 2 byte.");
            var daysSinceDateStart = bytes.ToUInt16(isLittleEndian);
            if (daysSinceDateStart > maxNumberOfDays)
                throw new ArgumentException($"Read number exceeded the number of maximum days in the IEC date (read: {daysSinceDateStart}, max: {maxNumberOfDays})",
                    nameof(bytes));
            return iecMinDate.AddDays(daysSinceDateStart);
        }
        public static DateTime ToDtl(this byte[] bytes)
        {
            if (bytes.Length != 12)
                throw new ArgumentOutOfRangeException(nameof(bytes), bytes.Length, "Range of 12 byte.");
            var year = AssertRangeInclusive(bytes[1] + bytes[0] * 256, 1970, 2262, "year");
            var month = AssertRangeInclusive(bytes[2], 1, 12, "month");
            var day = AssertRangeInclusive(bytes[3], 1, 31, "day of month");
            var dayOfWeek = AssertRangeInclusive(bytes[4], 1, 7, "day of week");
            var hour = AssertRangeInclusive(bytes[5], 0, 23, "hour");
            var minute = AssertRangeInclusive(bytes[6], 0, 59, "minute");
            var second = AssertRangeInclusive(bytes[7], 0, 59, "second");
            var ns = BitConverter.ToUInt32([bytes[11], bytes[10], bytes[9], bytes[8]], 0);
            var nanoseconds = AssertRangeInclusive<uint>(ns, 0, 999999999, "nanoseconds");
            var time = new DateTime(year, month, day, hour, minute, second);
            return time.AddTicks(nanoseconds / 100);
        }
        public static TimeSpan[] ToLTimeOfDays(this byte[] bytes, bool isLittleEndian = true)
        {
            return bytes.ToLTimes(isLittleEndian);
        }
        public static TimeSpan ToLTimeOfDay(this byte[] bytes, bool isLittleEndian = true)
        {
            return bytes.ToLTime(isLittleEndian);
        }
        public static TimeSpan ToTimeOfDay(this byte[] bytes, bool isLittleEndian = true)
        {
            return bytes.ToTime(isLittleEndian);
        }
        public static TimeSpan[] ToLTimes(this byte[] bytes, bool isLittleEndian = true)
        {
            if (bytes.Length % 8 != 0)
                throw new ArgumentOutOfRangeException(nameof(bytes), bytes.Length, "Multiple of 8 byte.");
            return bytes.ToInt64Array(isLittleEndian).Select(c => TimeSpan.FromTicks(c)).ToArray();
        }
        public static TimeSpan ToLTime(this byte[] bytes, bool isLittleEndian = true)
        {
            if (bytes.Length != 8)
                throw new ArgumentOutOfRangeException(nameof(bytes), bytes.Length, "Range of 8 byte.");
            return TimeSpan.FromTicks(bytes.ToInt64(isLittleEndian));
        }
        public static TimeSpan[] ToTimes(this byte[] bytes, bool isLittleEndian = true)
        {
            if (bytes.Length % 4 != 0)
                throw new ArgumentOutOfRangeException(nameof(bytes), bytes.Length, "Multiple of 4 byte.");
            return bytes.ToInt32Array(isLittleEndian).Select(c => TimeSpan.FromMilliseconds(c)).ToArray();
        }
        public static TimeSpan ToTime(this byte[] bytes, bool isLittleEndian = true)
        {
            if (bytes.Length != 4)
                throw new ArgumentOutOfRangeException(nameof(bytes), bytes.Length, "Range of 4 byte.");
            return TimeSpan.FromMilliseconds(bytes.ToInt32(isLittleEndian));
        }
        public static byte[] ToBytesFromDateAndTime(this DateTime dateTime)
        {
            if (dateTime < minimumDateTime || dateTime > maximumDateTime)
                throw new ArgumentOutOfRangeException(nameof(dateTime), dateTime, $"Range of {minimumDateTime}-{maximumDateTime}.");
            byte EncodeBcd(int value) => (byte)((value / 10 << 4) | value % 10);
            byte MapYear(int year) => (byte)(year < 2000 ? year - 1900 : year - 2000);
            int DayOfWeekToInt(DayOfWeek dayOfWeek) => (int)dayOfWeek + 1;
            return new[]
            {
                EncodeBcd(MapYear(dateTime.Year)),
                EncodeBcd(dateTime.Month),
                EncodeBcd(dateTime.Day),
                EncodeBcd(dateTime.Hour),
                EncodeBcd(dateTime.Minute),
                EncodeBcd(dateTime.Second),
                EncodeBcd(dateTime.Millisecond / 10),
                (byte) (dateTime.Millisecond % 10 << 4 | DayOfWeekToInt(dateTime.DayOfWeek))
            };
        }
        public static byte[] ToBytesFromDate(this DateTime dateTime, bool isLittleEndian = true)
        {
            if (dateTime < iecMinDate || dateTime > iecMaxDate)
                throw new ArgumentOutOfRangeException(nameof(dateTime), dateTime, $"Range of {iecMinDate}-{iecMaxDate}.");
            var day = (ushort)(dateTime - iecMinDate).TotalDays;
            return day.ToBytes(isLittleEndian);
        }
        public static byte[] ToBytesFromTime(this TimeSpan timeSpan, bool isLittleEndian = true)
        {
            if (timeSpan < minimumTimeSpan || timeSpan > maximumTimeSpan)
                throw new ArgumentOutOfRangeException(nameof(timeSpan), timeSpan, $"Range of {minimumTimeSpan}-{maximumTimeSpan}.");
            return ((int)timeSpan.TotalMilliseconds).ToBytes(isLittleEndian);
        }
        public static byte[] ToBytesFromLTime(this TimeSpan timeSpan, bool isLittleEndian = true)
        {
            if (timeSpan < minimumTimeSpan || timeSpan > maximumTimeSpan)
                throw new ArgumentOutOfRangeException(nameof(timeSpan), timeSpan, $"Range of {minimumTimeSpan}-{maximumTimeSpan}.");
            return timeSpan.Ticks.ToBytes(isLittleEndian);
        }
        public static byte[] ToBytesFromLTimeOfDay(this TimeSpan timeSpan, bool isLittleEndian = true)
        {
            return timeSpan.ToBytesFromLTime(isLittleEndian);
        }
        public static byte[] ToBytesFromTimeOfDay(this TimeSpan timeSpan, bool isLittleEndian = true)
        {
            return timeSpan.ToBytesFromTime(isLittleEndian);
        }
        public static byte[] ToBytesFromDtl(this DateTime dateTime, bool isLittleEndian = true)
        {
            if (dateTime < minimumDateTime || dateTime > maximumDateTime)
                throw new ArgumentOutOfRangeException(nameof(dateTime), dateTime, $"Range of {minimumDateTime}-{maximumDateTime}.");
            var buffer = new byte[12];
            var years = ((ushort)dateTime.Year).ToBytes(isLittleEndian);
            buffer[0] = years[0];
            buffer[1] = years[1];
            buffer[2] = (byte)dateTime.Month;
            buffer[3] = (byte)dateTime.Day;
            buffer[4] = (byte)(dateTime.DayOfWeek + 1);
            buffer[5] = (byte)dateTime.Hour;
            buffer[6] = (byte)dateTime.Minute;
            buffer[7] = (byte)dateTime.Second;
            var nanoseconds = ((int)(dateTime.Ticks % 10000000 * 100)).ToBytes(isLittleEndian);
            buffer[8] = nanoseconds[0];
            buffer[9] = nanoseconds[1];
            buffer[10] = nanoseconds[2];
            buffer[11] = nanoseconds[3];
            return buffer;
        }
    }
}
