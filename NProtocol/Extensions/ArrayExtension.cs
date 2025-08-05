using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NProtocol.Extensions
{
    public static class ArrayExtension
    {
        public static T[] Slice<T>(this T[] array, int start)
         => array.Slice(start, array.Length - start);
        public static T[] Slice<T>(this T[] array, int start, int length)
        {
            if (start < 0 || start > array.Length)
                throw new ArgumentOutOfRangeException(nameof(start), start, "Index error");

            if (length < 0 || length > array.Length - start)
                throw new ArgumentOutOfRangeException(nameof(start), start, "Length error");

            if (length == 0)
                return Array.Empty<T>();

            var buffer = new T[length];
            Array.Copy(array, start, buffer, 0, length);
            return buffer;
        }
        public static T[] Combine<T>(this T[] firstArray, T[] nextArray) where T : struct
        {
            if (nextArray.Length == 0) return firstArray;
            if (firstArray.Length == 0) return nextArray;

            var combined = new T[firstArray.Length + nextArray.Length];
            Array.Copy(firstArray, 0, combined, 0, firstArray.Length);
            Array.Copy(nextArray, 0, combined, firstArray.Length, nextArray.Length);
            return combined;
        }
        public static T[] Combine<T>(this T[] firstArray, params T[][] nextArray) where T : struct
        {
            if (nextArray.Length == 0) return firstArray;

            int totalLength = firstArray.Length + nextArray.Sum(arr => arr.Length);
            T[] result = new T[totalLength];
            Array.Copy(firstArray, 0, result, 0, firstArray.Length);
            int offset = firstArray.Length;
            foreach (var array in nextArray)
            {
                Array.Copy(array, 0, result, offset, array.Length);
                offset += array.Length;
            }
            return result;
        }
        private static void FlattenArray(Array array, List<object> result)
        {
            foreach (var item in array)
            {
                if (item is Array subArray)
                {
                    FlattenArray(subArray, result);
                }
                else
                {
                    result.Add(item);
                }
            }
        }
        public static List<object> Flatten(this Array array)
        {
            var result = new List<object>();
            if (array == null || array.Length == 0)
            {
                return result;
            }

            FlattenArray(array, result);
            return result;
        }
        public static string ToFlattenString(this Array array)
        {
            var flattened = array.Flatten();
            var sb = new StringBuilder("[ ");
            for (int i = 0; i < flattened.Count; i++)
            {
                var type = flattened[i].GetType();
                if (type.Name == nameof(String) || type.Name == nameof(TimeSpan) || type.Name == nameof(DateTime))
                {
                    sb.Append($"'{flattened[i]}'");
                }
                else
                {
                    sb.Append(flattened[i]);
                }

                if (i < flattened.Count - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(" ]");
            return sb.ToString();
        }
        public static IEnumerable<T[]> ChunkBy<T>(this T[] array, int size)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (size <= 0)
                throw new ArgumentException("Segment size must be positive.", nameof(size));

            for (int i = 0; i < array.Length; i += size)
            {
                int segmentLength = Math.Min(size, array.Length - i);
                T[] chunk = new T[segmentLength];
                Array.Copy(array, i, chunk, 0, segmentLength);
                yield return chunk;
            }
        }
    }
}