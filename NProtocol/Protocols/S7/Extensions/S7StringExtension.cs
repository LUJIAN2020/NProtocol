using System;
using System.Text;

namespace NProtocol.Protocols.S7.Extensions
{
    public static class S7StringExtension
    {
        public const byte S7StringMaximumLength = 0xFE;
        public const ushort S7WStringMaximumLength = 0x3FFE;

        /// <summary>
        /// 字节数组转换为S7字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string ToS7String(this byte[] bytes)
        {
            if (bytes.Length < 2)
                throw new ArgumentOutOfRangeException(
                    nameof(bytes.Length),
                    bytes.Length,
                    "The array length must be > 2"
                );
            if (bytes[1] > bytes[0])
                throw new ArgumentOutOfRangeException(
                    nameof(bytes),
                    bytes[1],
                    "Maximum character length must be ≥ actual character length"
                );
            return Encoding.ASCII.GetString(bytes, 2, bytes[1]);
        }

        /// <summary>
        /// 字节数组转换为S7W字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string ToS7WString(this byte[] bytes)
        {
            if (bytes.Length < 4)
                throw new ArgumentOutOfRangeException(
                    nameof(bytes),
                    bytes.Length,
                    "The array length must be > 4"
                );
            int size = (bytes[0] << 8) | bytes[1];
            int length = (bytes[2] << 8) | bytes[3];
            if (length > size)
                throw new ArgumentOutOfRangeException(
                    nameof(bytes),
                    length,
                    "Maximum character length must be ≥ actual character length"
                );
            return Encoding.BigEndianUnicode.GetString(bytes, 4, length * 2);
        }

        /// <summary>
        /// ASCII字符串转字节数组
        /// </summary>
        /// <param name="value">字符串内容</param>
        /// <param name="reservedLength">有效长度</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] ToBytesFromS7String(this string value, int reservedLength)
        {
            if (reservedLength > S7StringMaximumLength)
                throw new ArgumentOutOfRangeException(
                    nameof(reservedLength),
                    reservedLength,
                    $"Maximum character length <= {S7StringMaximumLength}"
                );
            var bytes = Encoding.ASCII.GetBytes(value);
            if (bytes.Length > reservedLength)
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value.Length,
                    "The actual content length is greater than the specified length"
                );
            var buffer = new byte[2 + reservedLength];
            Array.Copy(bytes, 0, buffer, 2, bytes.Length);
            buffer[0] = (byte)reservedLength;
            buffer[1] = (byte)bytes.Length;
            return buffer;
        }

        /// <summary>
        /// Unicode字符串转字节数组
        /// </summary>
        /// <param name="value">字符串内容</param>
        /// <param name="reservedLength">有效长度</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] ToBytesFromS7WString(this string value, int reservedLength)
        {
            if (reservedLength > S7WStringMaximumLength)
                throw new ArgumentOutOfRangeException(
                    nameof(reservedLength),
                    reservedLength,
                    $"Maximum character length <= {S7WStringMaximumLength}"
                );
            var buffer = new byte[4 + reservedLength * 2];
            buffer[0] = (byte)((reservedLength >> 8) & 0xFF);
            buffer[1] = (byte)(reservedLength & 0xFF);
            buffer[2] = (byte)((value.Length >> 8) & 0xFF);
            buffer[3] = (byte)(value.Length & 0xFF);
            var stringLength =
                Encoding.BigEndianUnicode.GetBytes(value, 0, value.Length, buffer, 4) / 2;
            if (stringLength > reservedLength)
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value.Length,
                    "The actual content length is greater than the specified length"
                );
            return buffer;
        }
    }
}
