using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;

namespace NProtocol.Extensions
{
    /// <summary>
    /// BitConverter扩展方法，实现大小端序方法,默认为小端序
    /// </summary>
    public static class BitConverterExtension
    {
        /// <summary>
        /// true=小端序 false=大端序
        /// </summary>
        public readonly static bool IsLittleEndian = BitConverter.IsLittleEndian;
        /// <summary>
        /// byte to bool[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool[] ToBooleans(this byte value)
        {
            bool[] bs = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                bs[i] = (value >> i & 0x01) == 1;
            }
            return bs;
        }
        /// <summary>
        /// byte[] to bool[]
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static bool[] ToBooleans(this byte[] bytes)
        {
            bool[] result = new bool[bytes.Length * 8];
            for (int i = 0; i < bytes.Length; i++)
            {
                bool[] data = ToBooleans(bytes[i]);
                Buffer.BlockCopy(data, 0, result, i * 8, data.Length);
            }
            return result;
        }
        /// <summary>
        /// Int16 to bool[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool[] ToBooleans(this short value, bool isLittleEndian = true)
        {
            return value.ToBytes(isLittleEndian).ToBooleans();
        }
        /// <summary>
        /// UInt16 to bool[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool[] ToBooleans(this ushort value, bool isLittleEndian = true)
        {
            return ((short)value).ToBooleans(isLittleEndian);
        }
        /// <summary>
        /// Int32 to bool[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool[] ToBooleans(this int value, bool isLittleEndian = true)
        {
            return value.ToBytes(isLittleEndian).ToBooleans();
        }
        /// <summary>
        /// UInt32 to bool[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool[] ToBooleans(this uint value, bool isLittleEndian = true)
        {
            return value.ToBytes(isLittleEndian).ToBooleans();
        }
        /// <summary>
        /// Int64 to bool[]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static bool[] ToBooleans(this long value, bool isLittleEndian = true)
        {
            return value.ToBytes(isLittleEndian).ToBooleans();
        }
        /// <summary>
        /// UInt64 to bool[]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static bool[] ToBooleans(this ulong value, bool isLittleEndian = true)
        {
            return value.ToBytes(isLittleEndian).ToBooleans();
        }
        /// <summary>
        /// Int16[] to bool[]
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static bool[] ToBooleans(this short[] values, bool isLittleEndian = true)
        {
            return values.ToBytes(isLittleEndian).ToBooleans();
        }
        /// <summary>
        /// UInt16[] to bool[]
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static bool[] ToBooleans(this ushort[] values, bool isLittleEndian = true)
        {
            return values.ToBytes(isLittleEndian).ToBooleans();
        }
        /// <summary>
        /// Int32[] to bool[]
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static bool[] ToBooleans(this int[] values, bool isLittleEndian = true)
        {
            return values.ToBytes(isLittleEndian).ToBooleans();
        }
        /// <summary>
        /// UInt32[] to bool[]
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static bool[] ToBooleans(this uint[] values, bool isLittleEndian = true)
        {
            return values.ToBytes(isLittleEndian).ToBooleans();
        }
        /// <summary>
        /// Int64[] to bool[]
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static bool[] ToBooleans(this long[] values, bool isLittleEndian = true)
        {
            return values.ToBytes(isLittleEndian).ToBooleans();
        }
        /// <summary>
        /// UInt64[] to bool[]
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static bool[] ToBooleans(this ulong[] values, bool isLittleEndian = true)
        {
            return values.ToBytes(isLittleEndian).ToBooleans();
        }
        /// <summary>
        /// bool[] to byte
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">数组长度必须<=8</exception>
        public static byte ToByte(this bool[] bs)
        {
            if (bs.Length > 8)
                throw new ArgumentOutOfRangeException(nameof(bs), bs.Length, "The array length must be <= 8");

            byte b = 0;
            for (int i = 0; i < bs.Length; i++)
            {
                if (bs[i])
                {
                    b += (byte)(1 << i);
                }
            }
            return b;
        }
        /// <summary>
        /// bool[] to byte[]
        /// </summary>
        /// <param name="bs">bool数组</param>
        /// <returns></returns>
        public static byte[] ToBytes(this bool[] bs)
        {
            int size = bs.Length % 8 == 0 ? bs.Length / 8 : bs.Length / 8 + 1;
            byte[] buffer = new byte[size];
            for (int i = 0; i < buffer.Length; i++)
            {
                bool[] bs8 = new bool[8];
                if (i + 1 == buffer.Length) bs8 = new bool[bs.Length - i * 8];
                Buffer.BlockCopy(bs, i * 8, bs8, 0, bs8.Length);
                buffer[i] = bs8.ToByte();
            }
            return buffer;
        }
        /// <summary>
        /// Int16 to bte[]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isLittleEndian">默认小端序</param>
        /// <returns></returns>
        public static byte[] ToBytes(this short value, bool isLittleEndian = true)
        {
            return isLittleEndian
                ? BitConverter.GetBytes(value)
                : BitConverter.GetBytes(value).Reverse().ToArray();
        }
        /// <summary>
        /// UInt16 to byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this ushort value, bool isLittleEndian = true)
        {
            return isLittleEndian
                ? BitConverter.GetBytes(value)
                : BitConverter.GetBytes(value).Reverse().ToArray();
        }
        /// <summary>
        /// Int32 to byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this int value, bool isLittleEndian = true)
        {
            return isLittleEndian
                ? BitConverter.GetBytes(value)
                : BitConverter.GetBytes(value).Reverse().ToArray();
        }
        /// <summary>
        /// UInt32 to byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this uint value, bool isLittleEndian = true)
        {
            return isLittleEndian
                ? BitConverter.GetBytes(value)
                : BitConverter.GetBytes(value).Reverse().ToArray();
        }
        /// <summary>
        /// Int64 to byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this long value, bool isLittleEndian = true)
        {
            return isLittleEndian
                ? BitConverter.GetBytes(value)
                : BitConverter.GetBytes(value).Reverse().ToArray();
        }
        /// <summary>
        /// UInt64 to byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this ulong value, bool isLittleEndian = true)
        {
            return isLittleEndian
                ? BitConverter.GetBytes(value)
                : BitConverter.GetBytes(value).Reverse().ToArray();
        }
        /// <summary>
        /// float to byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this float value, bool isLittleEndian = true)
        {
            return isLittleEndian
                ? BitConverter.GetBytes(value)
                : BitConverter.GetBytes(value).Reverse().ToArray();
        }
        /// <summary>
        /// double to byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this double value, bool isLittleEndian = true)
        {
            return isLittleEndian
                 ? BitConverter.GetBytes(value)
                 : BitConverter.GetBytes(value).Reverse().ToArray();
        }
        /// <summary>
        /// Int16[] to byte[]
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this short[] values, bool isLittleEndian = true)
        {
            byte[] buffer = new byte[values.Length * 2];
            for (int i = 0; i < values.Length; i++)
            {
                byte[] bytes = ToBytes(values[i], isLittleEndian);
                buffer[i * 2] = bytes[0];
                buffer[i * 2 + 1] = bytes[1];
            }
            return buffer;
        }
        /// <summary>
        /// UInt16[] to byte[]
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this ushort[] values, bool isLittleEndian = true)
        {
            byte[] buffer = new byte[values.Length * 2];
            for (int i = 0; i < values.Length; i++)
            {
                byte[] bytes = ToBytes(values[i], isLittleEndian);
                buffer[i * 2] = bytes[0];
                buffer[i * 2 + 1] = bytes[1];
            }
            return buffer;
        }
        /// <summary>
        /// Int32[] to byte[]
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this int[] values, bool isLittleEndian = true)
        {
            byte[] buffer = new byte[values.Length * 4];
            for (int i = 0; i < values.Length; i++)
            {
                byte[] bytes = values[i].ToBytes(isLittleEndian);
                buffer[i * 4] = bytes[0];
                buffer[i * 4 + 1] = bytes[1];
                buffer[i * 4 + 2] = bytes[2];
                buffer[i * 4 + 3] = bytes[3];
            }
            return buffer;
        }
        /// <summary>
        /// UInt32[] to byte[]
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this uint[] values, bool isLittleEndian = true)
        {
            byte[] buffer = new byte[values.Length * 4];
            for (int i = 0; i < values.Length; i++)
            {
                byte[] bytes = values[i].ToBytes(isLittleEndian);
                buffer[i * 4] = bytes[0];
                buffer[i * 4 + 1] = bytes[1];
                buffer[i * 4 + 2] = bytes[2];
                buffer[i * 4 + 3] = bytes[3];
            }
            return buffer;
        }
        /// <summary>
        /// Int32[] to byte[]
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this long[] values, bool isLittleEndian = true)
        {
            byte[] buffer = new byte[values.Length * 8];
            for (int i = 0; i < values.Length; i++)
            {
                byte[] bytes = values[i].ToBytes(isLittleEndian);
                buffer[i * 8] = bytes[0];
                buffer[i * 8 + 1] = bytes[1];
                buffer[i * 8 + 2] = bytes[2];
                buffer[i * 8 + 3] = bytes[3];
                buffer[i * 8 + 4] = bytes[4];
                buffer[i * 8 + 5] = bytes[5];
                buffer[i * 8 + 6] = bytes[6];
                buffer[i * 8 + 7] = bytes[7];
            }
            return buffer;
        }
        /// <summary>
        /// UInt32[] to byte[]
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this ulong[] values, bool isLittleEndian = true)
        {
            byte[] buffer = new byte[values.Length * 8];
            for (int i = 0; i < values.Length; i++)
            {
                byte[] bytes = values[i].ToBytes(isLittleEndian);
                buffer[i * 8] = bytes[0];
                buffer[i * 8 + 1] = bytes[1];
                buffer[i * 8 + 2] = bytes[2];
                buffer[i * 8 + 3] = bytes[3];
                buffer[i * 8 + 4] = bytes[4];
                buffer[i * 8 + 5] = bytes[5];
                buffer[i * 8 + 6] = bytes[6];
                buffer[i * 8 + 7] = bytes[7];
            }
            return buffer;
        }
        /// <summary>
        /// float[] to byte[]
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this float[] values, bool isLittleEndian = true)
        {
            byte[] buffer = new byte[values.Length * 4];
            for (int i = 0; i < values.Length; i++)
            {
                var buf = values[i].ToBytes(isLittleEndian);
                buffer[i * 4] = buf[0];
                buffer[i * 4 + 1] = buf[1];
                buffer[i * 4 + 2] = buf[2];
                buffer[i * 4 + 3] = buf[3];
            }
            return buffer;
        }
        /// <summary>
        /// double[] to byte[]
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this double[] values, bool isLittleEndian = true)
        {
            byte[] buffer = new byte[values.Length * 8];
            for (int i = 0; i < values.Length; i++)
            {
                var buf = values[i].ToBytes(isLittleEndian);
                buffer[i * 8] = buf[0];
                buffer[i * 8 + 1] = buf[1];
                buffer[i * 8 + 2] = buf[2];
                buffer[i * 8 + 3] = buf[3];
                buffer[i * 8 + 4] = buf[4];
                buffer[i * 8 + 5] = buf[5];
                buffer[i * 8 + 6] = buf[6];
                buffer[i * 8 + 7] = buf[7];
            }
            return buffer;
        }
        /// <summary>
        /// byte[] to Int16
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static short ToInt16(this byte[] buffer, bool isLittleEndian = true)
        {
            if (buffer.Length != 2)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be 2");

            if (isLittleEndian) Array.Reverse(buffer);
            return BitConverter.ToInt16(buffer, 0);
        }
        /// <summary>
        /// byte[] to UInt16
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static ushort ToUInt16(this byte[] buffer, bool isLittleEndian = true)
        {
            if (buffer.Length != 2)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be 2");

            if (isLittleEndian) Array.Reverse(buffer);
            return BitConverter.ToUInt16(buffer, 0);
        }
        /// <summary>
        /// byte[] to Int32
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int ToInt32(this byte[] buffer, bool isLittleEndian = true)
        {
            if (buffer.Length != 4)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be 4");

            if (isLittleEndian) Array.Reverse(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }
        /// <summary>
        /// byte[] to UInt32
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static uint ToUInt32(this byte[] buffer, bool isLittleEndian = true)
        {
            return (uint)buffer.ToInt32(isLittleEndian);
        }
        /// <summary>
        /// byte[] to Int64
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static long ToInt64(this byte[] buffer, bool isLittleEndian = true)
        {
            if (buffer.Length != 8)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be 8");

            if (isLittleEndian) Array.Reverse(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }
        /// <summary>
        /// byte[] to UInt64
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static ulong ToUInt64(this byte[] buffer, bool isLittleEndian = true)
        {
            if (buffer.Length != 8)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be 8");

            if (isLittleEndian) Array.Reverse(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
        /// <summary>
        /// byte[] to float
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static float ToFloat(this byte[] buffer, bool isLittleEndian = true)
        {
            if (buffer.Length != 4)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be 4");

            if (isLittleEndian) Array.Reverse(buffer);
            return BitConverter.ToSingle(buffer, 0);
        }
        /// <summary>
        /// byte[] to double
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static double ToDouble(this byte[] buffer, bool isLittleEndian = true)
        {
            if (buffer.Length != 8)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be 8");

            if (isLittleEndian) Array.Reverse(buffer);
            return BitConverter.ToDouble(buffer, 0);
        }
        /// <summary>
        /// byte[] to Int16[]
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static short[] ToInt16Array(this byte[] buffer, bool isLittleEndian = true)
        {
            if (buffer.Length < 2)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be > 2");

            if (buffer.Length % 2 != 0)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be a multiple of 2");

            short[] shorts = new short[buffer.Length / 2];
            for (int i = 0; i < shorts.Length; i++)
            {
                byte[] buf = [buffer[i * 2], buffer[i * 2 + 1]];
                shorts[i] = buf.ToInt16(isLittleEndian);
            }
            return shorts;
        }
        /// <summary>
        /// byte[] to UInt16[]
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static ushort[] ToUInt16Array(this byte[] buffer, bool isLittleEndian = true)
        {
            if (buffer.Length < 2)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be > 2");

            if (buffer.Length % 2 != 0)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be a multiple of 2");

            ushort[] ushorts = new ushort[buffer.Length / 2];
            for (int i = 0; i < ushorts.Length; i++)
            {
                byte[] buf = [buffer[i * 2], buffer[i * 2 + 1]];
                ushorts[i] = buf.ToUInt16(isLittleEndian);
            }
            return ushorts;
        }
        /// <summary>
        /// byte[] to Int32[]
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int[] ToInt32Array(this byte[] buffer, bool isLittleEndian = true)
        {
            if (buffer.Length < 4)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be > 4");

            if (buffer.Length % 4 != 0)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be a multiple of 4");

            int[] ints = new int[buffer.Length / 4];
            byte[] buf = new byte[4];
            for (int i = 0; i < ints.Length; i++)
            {
                buf[0] = buffer[i * 4];
                buf[1] = buffer[i * 4 + 1];
                buf[2] = buffer[i * 4 + 2];
                buf[3] = buffer[i * 4 + 3];
                ints[i] = buf.ToInt32(isLittleEndian);
            }
            return ints;
        }
        /// <summary>
        /// byte[] to UInt32[]
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static uint[] ToUInt32Array(this byte[] buffer, bool isLittleEndian = true)
        {
            if (buffer.Length < 4)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be > 4");

            if (buffer.Length % 4 != 0)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be a multiple of 4");

            uint[] uints = new uint[buffer.Length / 4];
            byte[] buf = new byte[4];
            for (int i = 0; i < uints.Length; i++)
            {
                buf[0] = buffer[i * 4];
                buf[1] = buffer[i * 4 + 1];
                buf[2] = buffer[i * 4 + 2];
                buf[3] = buffer[i * 4 + 3];
                uints[i] = buf.ToUInt32(isLittleEndian);
            }
            return uints;
        }
        /// <summary>
        /// byte[] to float[]
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static float[] ToFloatArray(this byte[] buffer, bool isLittleEndian = true)
        {
            if (buffer.Length < 4)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be > 4");

            if (buffer.Length % 4 != 0)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be a multiple of 4");

            float[] floats = new float[buffer.Length / 4];
            byte[] buf = new byte[4];
            for (int i = 0; i < floats.Length; i++)
            {
                buf[0] = buffer[i * 4];
                buf[1] = buffer[i * 4 + 1];
                buf[2] = buffer[i * 4 + 2];
                buf[3] = buffer[i * 4 + 3];
                floats[i] = buf.ToFloat(isLittleEndian);
            }
            return floats;
        }
        /// <summary>
        /// byte[] to double[]
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static double[] ToDoubleArray(this byte[] buffer, bool isLittleEndian = true)
        {
            if (buffer.Length < 8)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be > 8");

            if (buffer.Length % 8 != 0)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be a multiple of 8");

            double[] doubles = new double[buffer.Length / 8];
            byte[] buf = new byte[8];
            for (int i = 0; i < doubles.Length; i++)
            {
                buf[0] = buffer[i * 8];
                buf[1] = buffer[i * 8 + 1];
                buf[2] = buffer[i * 8 + 2];
                buf[3] = buffer[i * 8 + 3];
                buf[4] = buffer[i * 8 + 4];
                buf[5] = buffer[i * 8 + 5];
                buf[6] = buffer[i * 8 + 6];
                buf[7] = buffer[i * 8 + 7];
                doubles[i] = buf.ToDouble(isLittleEndian);
            }
            return doubles;
        }
        /// <summary>
        /// byte[] to Int64[]
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static long[] ToInt64Array(this byte[] buffer, bool isLittleEndian = true)
        {
            if (buffer.Length < 8)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be > 8");

            if (buffer.Length % 8 != 0)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be a multiple of 8");

            long[] values = new long[buffer.Length / 8];
            byte[] buf = new byte[8];
            for (int i = 0; i < values.Length; i++)
            {
                buf[0] = buffer[i * 8];
                buf[1] = buffer[i * 8 + 1];
                buf[2] = buffer[i * 8 + 2];
                buf[3] = buffer[i * 8 + 3];
                buf[4] = buffer[i * 8 + 4];
                buf[5] = buffer[i * 8 + 5];
                buf[6] = buffer[i * 8 + 6];
                buf[7] = buffer[i * 8 + 7];
                values[i] = buf.ToInt64(isLittleEndian);
            }
            return values;
        }
        /// <summary>
        /// byte[] to UInt64[]
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static ulong[] ToUInt64Array(this byte[] buffer, bool isLittleEndian = true)
        {
            if (buffer.Length < 8)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be > 8");

            if (buffer.Length % 8 != 0)
                throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Length, "The array length must be a multiple of 8");

            ulong[] values = new ulong[buffer.Length / 8];
            byte[] buf = new byte[8];
            for (int i = 0; i < values.Length; i++)
            {
                buf[0] = buffer[i * 8];
                buf[1] = buffer[i * 8 + 1];
                buf[2] = buffer[i * 8 + 2];
                buf[3] = buffer[i * 8 + 3];
                buf[4] = buffer[i * 8 + 4];
                buf[5] = buffer[i * 8 + 5];
                buf[6] = buffer[i * 8 + 6];
                buf[7] = buffer[i * 8 + 7];
                values[i] = buf.ToUInt64(isLittleEndian);
            }
            return values;
        }
        /// <summary>
        /// byte[] to array 只支持基础类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static T[] ToValueArrayFromBytes<T>(this byte[] buffer, bool isLittleEndian = true) where T : struct
        {
            int typeSize = Marshal.SizeOf(typeof(T));
            if (buffer.Length < typeSize)
                throw new ArgumentOutOfRangeException(nameof(buffer.Length), buffer.Length, $"The array length must be > {typeSize}");

            if (buffer.Length % typeSize > 0)
                throw new ArgumentOutOfRangeException(nameof(buffer.Length), buffer.Length,
                    $"This type `{nameof(T)}`, the buffer length must be a multiple of {typeSize}");

            T t = default;
            switch (t)
            {
                case short:
                    if (buffer.ToInt16Array(isLittleEndian) is T[] value1)
                        return value1;
                    break;
                case ushort:
                    if (buffer.ToUInt16Array(isLittleEndian) is T[] value2)
                        return value2;
                    break;
                case int:
                    if (buffer.ToInt32Array(isLittleEndian) is T[] value3)
                        return value3;
                    break;
                case uint:
                    if (buffer.ToUInt32Array(isLittleEndian) is T[] value4)
                        return value4;
                    break;
                case long:
                    if (buffer.ToInt64Array(isLittleEndian) is T[] value5)
                        return value5;
                    break;
                case ulong:
                    if (buffer.ToUInt64Array(isLittleEndian) is T[] value6)
                        return value6;
                    break;
                case float:
                    if (buffer.ToFloatArray(isLittleEndian) is T[] value7)
                        return value7;
                    break;
                case double:
                    if (buffer.ToDoubleArray(isLittleEndian) is T[] value8)
                        return value8;
                    break;
            }
            throw new ArgumentException($"`{nameof(T)}` type not supported");
        }
        /// <summary>
        /// byte[] to value 只支持基础类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static T ToValueFromBytes<T>(this byte[] buffer, bool isLittleEndian = true) where T : struct
        {
            return buffer.ToValueArrayFromBytes<T>(isLittleEndian).FirstOrDefault();
        }
        /// <summary>
        /// 值类型转换为字节数组 只支持基础类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] ToBytesFromValue<T>(this T value, bool isLittleEndian = true) where T : struct
            => value switch
            {
                short val => val.ToBytes(isLittleEndian),
                ushort val => val.ToBytes(isLittleEndian),
                int val => val.ToBytes(isLittleEndian),
                uint val => val.ToBytes(isLittleEndian),
                float val => val.ToBytes(isLittleEndian),
                long val => val.ToBytes(isLittleEndian),
                ulong val => val.ToBytes(isLittleEndian),
                double val => val.ToBytes(isLittleEndian),
                _ => throw new ArgumentException($"`{nameof(T)}` type not supported", nameof(value)),
            };
        /// <summary>
        /// 值类型数组转换为字节数组 只支持基础类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static byte[] ToBytesFromValues<T>(this T[] values, bool isLittleEndian = true) where T : struct
            => values.SelectMany(c => c.ToBytesFromValue(isLittleEndian)).ToArray();
        /// <summary>
        /// Word转换为16个位
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static bool[] ToBooleansFromWord(this ushort[] values, bool isLittleEndian = true)
        {
            if (values.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(values.Length), values.Length, "The array length must be > 0");

            var bits = new bool[values.Length * 2 * 8];
            for (int i = 0; i < values.Length; i++)
            {
                var buffer = BitConverter.GetBytes(values[i]);
                if (isLittleEndian)
                    Array.Reverse(buffer);
                var bs = buffer.ToBooleansFromWord(isLittleEndian);
                Buffer.BlockCopy(bs, 0, bits, i * 16, 16);
            }
            return bits;
        }
        /// <summary>
        /// 2Byte的Word转16个位
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static bool[] ToBooleansFromWord(this byte[] values, bool isLittleEndian = true)
        {
            if (values.Length < 2 || values.Length % 2 > 0)
                throw new ArgumentOutOfRangeException(nameof(values.Length), values.Length, "The array length must be even");

            var bits = new bool[values.Length * 8];
            var bitArray = new BitArray(values);
            if (isLittleEndian)
            {
                for (int i = 0; i < bitArray.Count; i += 16)
                {
                    for (int j = 8; j < 16; j++)
                    {
                        bits[i + j - 8] = bitArray[i + j];
                    }
                    for (int j = 0; j < 8; j++)
                    {
                        bits[i + j + 8] = bitArray[i + j];
                    }
                }
            }
            else
            {
                for (int i = 0; i < bitArray.Count; i++)
                {
                    bits[i] = bitArray[i];
                }
            }
            return bits;
        }
    }
}
