using NProtocol.Communication.Extensions;
using System;

namespace NProtocol.Protocols.Modbus
{
    /// <summary>
    /// 字节序扩展方法
    /// </summary>
    public static class EndianFormatExtension
    {
        public static int ToInt32(this byte[] bytes, ByteFormat format)
        {
            ValidateTypeFromByteFormat(typeof(int), format);
            if (bytes.Length != 4)
                throw new ArgumentOutOfRangeException(nameof(bytes.Length));
            switch (format)
            {
                case ByteFormat.ABCD:
                    return BitConverter.ToInt32(new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] }, 0);
                case ByteFormat.CDAB:
                    return BitConverter.ToInt32(new byte[] { bytes[1], bytes[0], bytes[3], bytes[2] }, 0);
                case ByteFormat.BADC:
                    return BitConverter.ToInt32(new byte[] { bytes[2], bytes[3], bytes[0], bytes[1] }, 0);
                case ByteFormat.DCBA:
                    return BitConverter.ToInt32(bytes, 0);
            }
            return default;
        }
        public static int[] ToInt32Array(this byte[] bytes, ByteFormat format)
        {
            if (bytes.Length < 4 || bytes.Length % 4 > 0)
                throw new ArgumentOutOfRangeException(nameof(bytes.Length));
            int[] values = new int[bytes.Length / 4];
            for (int i = 0; i < bytes.Length; i += 4)
            {
                values[i / 4] = bytes.Slice(i, 4).ToInt32(format);
            }
            return values;
        }
        public static uint ToUInt32(this byte[] bytes, ByteFormat format)
        {
            ValidateTypeFromByteFormat(typeof(uint), format);
            if (bytes.Length != 4)
                throw new ArgumentOutOfRangeException(nameof(bytes.Length));
            switch (format)
            {
                case ByteFormat.ABCD:
                    return BitConverter.ToUInt32(new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] }, 0);
                case ByteFormat.CDAB:
                    return BitConverter.ToUInt32(new byte[] { bytes[1], bytes[0], bytes[3], bytes[2] }, 0);
                case ByteFormat.BADC:
                    return BitConverter.ToUInt32(new byte[] { bytes[2], bytes[3], bytes[0], bytes[1] }, 0);
                case ByteFormat.DCBA:
                    return BitConverter.ToUInt32(bytes, 0);
            }
            return default;
        }
        public static uint[] ToUInt32Array(this byte[] bytes, ByteFormat format)
        {
            if (bytes.Length < 4 || bytes.Length % 4 > 0)
                throw new ArgumentOutOfRangeException(nameof(bytes.Length));
            uint[] values = new uint[bytes.Length / 4];
            for (int i = 0; i < bytes.Length; i += 4)
            {
                values[i / 4] = bytes.Slice(i, 4).ToUInt32(format);
            }
            return values;
        }
        public static float ToFloat(this byte[] bytes, ByteFormat format)
        {
            ValidateTypeFromByteFormat(typeof(float), format);
            if (bytes.Length != 4)
                throw new ArgumentOutOfRangeException(nameof(bytes.Length));
            switch (format)
            {
                case ByteFormat.ABCD:
                    return BitConverter.ToSingle(new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] }, 0);
                case ByteFormat.CDAB:
                    return BitConverter.ToSingle(new byte[] { bytes[1], bytes[0], bytes[3], bytes[2] }, 0);
                case ByteFormat.BADC:
                    return BitConverter.ToSingle(new byte[] { bytes[2], bytes[3], bytes[0], bytes[1] }, 0);
                case ByteFormat.DCBA:
                    return BitConverter.ToSingle(bytes, 0);
            }
            return default;
        }
        public static float[] ToFloatArray(this byte[] bytes, ByteFormat format)
        {
            if (bytes.Length < 4 || bytes.Length % 4 > 0)
                throw new ArgumentOutOfRangeException(nameof(bytes.Length), bytes.Length, "The length of the array must be a multiple of 4");
            float[] values = new float[bytes.Length / 4];
            for (int i = 0; i < bytes.Length; i += 4)
            {
                values[i / 4] = bytes.Slice(i, 4).ToFloat(format);
            }
            return values;
        }
        public static double ToDouble(this byte[] bytes, ByteFormat format)
        {
            ValidateTypeFromByteFormat(typeof(double), format);
            if (bytes.Length != 8)
                throw new ArgumentOutOfRangeException(nameof(bytes.Length));
            switch (format)
            {
                case ByteFormat.ABCDEFGH:
                    return BitConverter.ToDouble(new byte[] { bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2], bytes[1], bytes[0] }, 0);
                case ByteFormat.GHEFCDAB:
                    return BitConverter.ToDouble(new byte[] { bytes[1], bytes[0], bytes[3], bytes[2], bytes[5], bytes[4], bytes[7], bytes[6] }, 0);
                case ByteFormat.BADCFEHG:
                    return BitConverter.ToDouble(new byte[] { bytes[6], bytes[7], bytes[4], bytes[5], bytes[2], bytes[3], bytes[0], bytes[1] }, 0);
                case ByteFormat.HGFEDCBA:
                    return BitConverter.ToDouble(bytes, 0);
            }
            return default;
        }
        public static double[] ToDoubleArray(this byte[] bytes, ByteFormat format)
        {
            if (bytes.Length < 8 || bytes.Length % 8 > 0)
                throw new ArgumentOutOfRangeException(nameof(bytes.Length), bytes.Length, "The length of the array must be a multiple of 8");
            double[] values = new double[bytes.Length / 8];
            for (int i = 0; i < bytes.Length; i += 8)
            {
                values[i / 8] = bytes.Slice(i, 8).ToDouble(format);
            }
            return values;
        }
        public static long ToInt64(this byte[] bytes, ByteFormat format)
        {
            ValidateTypeFromByteFormat(typeof(long), format);
            if (bytes.Length != 8)
                throw new ArgumentOutOfRangeException(nameof(bytes.Length));
            switch (format)
            {
                case ByteFormat.ABCDEFGH:
                    return BitConverter.ToInt64(new byte[] { bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2], bytes[1], bytes[0] }, 0);
                case ByteFormat.GHEFCDAB:
                    return BitConverter.ToInt64(new byte[] { bytes[1], bytes[0], bytes[3], bytes[2], bytes[5], bytes[4], bytes[7], bytes[6] }, 0);
                case ByteFormat.BADCFEHG:
                    return BitConverter.ToInt64(new byte[] { bytes[6], bytes[7], bytes[4], bytes[5], bytes[2], bytes[3], bytes[0], bytes[1] }, 0);
                case ByteFormat.HGFEDCBA:
                    return BitConverter.ToInt64(bytes, 0);
            }
            return default;
        }
        public static long[] ToInt64Array(this byte[] bytes, ByteFormat format)
        {
            if (bytes.Length < 8 || bytes.Length % 8 > 0)
                throw new ArgumentOutOfRangeException(nameof(bytes.Length), bytes.Length, "The length of the array must be a multiple of 8");
            long[] values = new long[bytes.Length / 8];
            for (int i = 0; i < bytes.Length; i += 8)
            {
                values[i / 8] = bytes.Slice(i, 8).ToInt64(format);
            }
            return values;
        }
        public static ulong ToUInt64(this byte[] bytes, ByteFormat format)
        {
            ValidateTypeFromByteFormat(typeof(ulong), format);
            if (bytes.Length != 8)
                throw new ArgumentOutOfRangeException(nameof(bytes.Length));
            switch (format)
            {
                case ByteFormat.ABCDEFGH:
                    return BitConverter.ToUInt64(new byte[] { bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2], bytes[1], bytes[0] }, 0);
                case ByteFormat.GHEFCDAB:
                    return BitConverter.ToUInt64(new byte[] { bytes[1], bytes[0], bytes[3], bytes[2], bytes[5], bytes[4], bytes[7], bytes[6] }, 0);
                case ByteFormat.BADCFEHG:
                    return BitConverter.ToUInt64(new byte[] { bytes[6], bytes[7], bytes[4], bytes[5], bytes[2], bytes[3], bytes[0], bytes[1] }, 0);
                case ByteFormat.HGFEDCBA:
                    return BitConverter.ToUInt64(bytes, 0);
            }
            return default;
        }
        public static ulong[] ToUInt64Array(this byte[] bytes, ByteFormat format)
        {
            if (bytes.Length < 8 || bytes.Length % 8 > 0)
                throw new ArgumentOutOfRangeException(nameof(bytes.Length), bytes.Length, "The length of the array must be a multiple of 8");
            ulong[] values = new ulong[bytes.Length / 8];
            for (int i = 0; i < bytes.Length; i += 8)
            {
                values[i / 8] = bytes.Slice(i, 8).ToUInt64(format);
            }
            return values;
        }
        private static void ValidateTypeFromByteFormat(Type type, ByteFormat format)
        {
            string name = type.FullName;
            switch (format)
            {
                case ByteFormat.ABCD:
                case ByteFormat.CDAB:
                case ByteFormat.BADC:
                case ByteFormat.DCBA:
                    if (name != typeof(int).FullName && name != typeof(uint).FullName && name != typeof(float).FullName)
                        throw new ArgumentException("Type error. Only 4-byte basic data types are supported", nameof(type));
                    break;
                case ByteFormat.ABCDEFGH:
                case ByteFormat.GHEFCDAB:
                case ByteFormat.BADCFEHG:
                case ByteFormat.HGFEDCBA:
                    if (name != typeof(long).FullName && name != typeof(ulong).FullName && name != typeof(double).FullName)
                        throw new ArgumentException("Type error. Only 8-byte basic data types are supported", nameof(type));
                    break;
                default:
                    throw new Exception($"Unsupported byte format `{format}` `{name}`");
            }
        }
    }
}
