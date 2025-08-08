using System;
using System.Linq;
using System.Text;

namespace NProtocol.Extensions
{
    public static class GeneralExtension
    {
        /// <summary>
        /// Chinese character filter
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static string FilterChinese(string content)
        {
            var sb = new StringBuilder();
            foreach (var c in content.ToCharArray())
            {
                if (c > 0 && c < 127)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Translate Chinese
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static string ConvertChinese(string content)
        {
            var sb = new StringBuilder();
            foreach (var c in content.ToCharArray())
            {
                if (c <= 0 || c >= 127)
                {
                    sb.Append(((short)c).ToString("X4"));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Hexadecimal String to Byte Array Only supports the conversion of hexadecimal strings
        /// </summary>
        /// <param name="hexString">A hexadecimal string</param>
        /// <param name="separator">separator</param>
        /// <returns></returns>
        public static byte[] ToBytesFromHexString(this string hexString, string separator = " ")
        {
            if (separator != "" && hexString.Contains(separator))
                hexString = hexString.Replace(separator, "");

            if (hexString.Length % 2 > 0)
                throw new ArgumentOutOfRangeException(
                    nameof(hexString.Length),
                    hexString.Length,
                    "The string length must be a multiple of 2"
                );

            var buffer = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i += 2)
            {
                string value = hexString.Substring(i, 2);
                buffer[i / 2] = Convert.ToByte(value, 16);
            }
            return buffer;
        }

        /// <summary>
        /// Convert an octal string to a Byte array
        /// </summary>
        /// <param name="octString"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static byte[] ToBytesFromOctString(this string octString, string separator = " ")
        {
            uint value = octString.ToUInt32FromOctString(separator);
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// The 16-bit string is converted to a UInt16<br/>
        /// Sample：F01F=>61471
        /// </summary>
        /// <param name="hexString">Hex address</param>
        /// <param name="separator">separator</param>
        /// <param name="isLittleEndian">LittleEndian</param>
        /// <returns></returns>
        public static ushort ToUInt16FromHexString(
            this string hexString,
            string separator = " ",
            bool isLittleEndian = true
        )
        {
            var buffer = hexString.ToBytesFromHexString(separator);

            if (buffer.Length > 2)
                throw new ArgumentOutOfRangeException(
                    nameof(hexString),
                    buffer.Length,
                    "The hex string length is incorrect"
                );

            if (buffer.Length < 2)
            {
                var array = new byte[2];
                for (int i = 0; i < buffer.Length; i++)
                {
                    array[1 - i] = buffer[i];
                }
                return array.ToUInt16(isLittleEndian);
            }
            return buffer.ToUInt16(isLittleEndian);
        }

        /// <summary>
        /// The 16-bit string is converted to a UInt32<br/>
        /// Sample：4FF01F=>2,031,951
        /// </summary>
        /// <param name="hexString">Hex string</param>
        /// <param name="separator">separator</param>
        /// <param name="isLittleEndian">LittleEndian</param>
        /// <returns></returns>
        public static uint ToUInt32FromHexString(
            this string hexString,
            string separator = " ",
            bool isLittleEndian = true
        )
        {
            var buffer = hexString.ToBytesFromHexString(separator);

            if (buffer.Length > 4)
                throw new ArgumentOutOfRangeException(
                    nameof(hexString),
                    buffer.Length,
                    "The hex string length is incorrect"
                );

            if (buffer.Length < 4)
            {
                var array = new byte[4];
                Array.Copy(buffer, 0, array, 4 - buffer.Length, buffer.Length);
                return array.ToUInt32(isLittleEndian);
            }
            return buffer.ToUInt32(isLittleEndian);
        }

        /// <summary>
        /// Convert an octal string to UInt32
        /// </summary>
        /// <param name="octString"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static uint ToUInt32FromOctString(this string octString, string separator = " ")
        {
            if (separator != "" && octString.Contains(separator))
                octString = octString.Replace(separator, "");

            return Convert.ToUInt32(octString, 8);
        }

        /// <summary>
        /// The byte array is converted to a hexadecimal string
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] buffer, string separator = " ")
        {
            return string.Join(separator, buffer.Select(c => c.ToString("X2"))).ToUpper();
        }

        /// <summary>
        /// Byte array to octal string
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ToOctString(this byte[] buffer, string separator = " ")
        {
            return string.Join(separator, buffer.Select(c => Convert.ToString(c, 8))).ToUpper();
        }

        /// <summary>
        /// String is converted to ASCII string Only English characters and symbols are converted
        /// </summary>
        /// <param name="content">String content</param>
        /// <param name="separator">separator</param>
        /// <param name="isFilterChinese">Whether to filter Chinese characters; true=filter; false=do not filter</param>
        /// <returns></returns>
        public static string ToAsciiString(
            this string content,
            string separator = " ",
            bool isFilterChinese = true
        )
        {
            string text = isFilterChinese ? FilterChinese(content) : ConvertChinese(content);
            return Encoding.ASCII.GetBytes(text).ToHexString(separator);
        }

        /// <summary>
        /// ASCII string is converted to a text string that does not contain Chinese characters
        /// </summary>
        /// <param name="asciiString">ASCII string</param>
        /// <param name="separator">separator</param>
        /// <returns></returns>
        public static string ToContentFromAsciiString(
            this string asciiString,
            string separator = " "
        )
        {
            var buffer = asciiString.ToBytesFromHexString(separator);
            return Encoding.ASCII.GetString(buffer);
        }

        public static string ToAsciiString(this byte[] buffer)
        {
            return Encoding.ASCII.GetString(buffer);
        }
    }
}
