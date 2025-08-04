using System;

namespace NProtocol.Extensions
{
    internal static class StringExtension
    {
        internal static bool IsNullOrEmpty(this string? s) => string.IsNullOrEmpty(s);
        internal static void ThrowIsNullOrEmpty(this string? s, string? paramName = default, string? message = default)
        {
            if (s.IsNullOrEmpty())
                throw new ArgumentNullException(paramName, message);
        }
        internal static bool IsNullOrWhiteSpace(this string? s) => string.IsNullOrWhiteSpace(s);
        internal static void ThrowIsNullOrWhiteSpace(this string? s, string? paramName = default, string? message = default)
        {
            if (s.IsNullOrWhiteSpace())
                throw new ArgumentNullException(paramName, message);
        }
    }
}
