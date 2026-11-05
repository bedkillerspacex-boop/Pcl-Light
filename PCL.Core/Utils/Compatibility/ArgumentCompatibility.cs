using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PCL.Core.Utils.Compatibility;

internal static class ArgumentCompatibility
{
    internal static void ThrowIfNullOrEmpty(string? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value is null) throw new ArgumentNullException(paramName);
        if (value.Length == 0) throw new ArgumentException("The value cannot be an empty string.", paramName);
    }

    internal static void ThrowIfNullOrWhiteSpace(string? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value is null) throw new ArgumentNullException(paramName);
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("The value cannot be empty or whitespace.", paramName);
    }
}

public static class Ascii
{
    public static bool IsDigit(char value) => value is >= '0' and <= '9';

    public static bool IsHexDigit(char value) => IsDigit(value) ||
        value is >= 'A' and <= 'F' or >= 'a' and <= 'f';

    public static bool IsLetter(char value) => value is >= 'A' and <= 'Z' or >= 'a' and <= 'z';

    public static bool IsLetterOrDigit(char value) => IsLetter(value) || IsDigit(value);
}

public static class RegexCompatibility
{
    private static readonly ConcurrentDictionary<(string Pattern, RegexOptions Options, int Timeout), Regex> Cache = new();

    public static Regex Get(string pattern, RegexOptions options = RegexOptions.None,
        int matchTimeoutMilliseconds = -1)
    {
        return Cache.GetOrAdd((pattern, options, matchTimeoutMilliseconds), static key =>
            new Regex(key.Pattern, key.Options | RegexOptions.Compiled,
                key.Timeout < 0 ? Regex.InfiniteMatchTimeout : TimeSpan.FromMilliseconds(key.Timeout)));
    }
}

public static class DictionaryCompatibility
{
    public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary) where TKey : notnull => new(dictionary);
}

public static class StopwatchCompatibility
{
    public static TimeSpan GetElapsedTime(long startingTimestamp) =>
        GetElapsedTime(startingTimestamp, Stopwatch.GetTimestamp());

    public static TimeSpan GetElapsedTime(long startingTimestamp, long endingTimestamp) =>
        TimeSpan.FromSeconds((endingTimestamp - startingTimestamp) / (double)Stopwatch.Frequency);
}

public static class StreamCompatibility
{
    public static async Task ReadExactlyAsync(this Stream stream, Memory<byte> buffer,
        CancellationToken cancellationToken = default)
    {
        while (!buffer.IsEmpty)
        {
            var bytesRead = await stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
            if (bytesRead == 0) throw new EndOfStreamException();
            buffer = buffer[bytesRead..];
        }
    }
}

public static class ArgumentOutOfRangeCompatibility
{
    public static void ThrowIfGreaterThanOrEqual<T>(T value, T other,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
    {
        if (value.CompareTo(other) >= 0)
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be less than the comparison value.");
    }

    public static void ThrowIfGreaterThan<T>(T value, T other,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
    {
        if (value.CompareTo(other) > 0)
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be less than or equal to the comparison value.");
    }

    public static void ThrowIfLessThan<T>(T value, T other,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
    {
        if (value.CompareTo(other) < 0)
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be greater than or equal to the comparison value.");
    }

    public static void ThrowIfZero<T>(T value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
    {
        if (value.CompareTo(default!) == 0)
            throw new ArgumentOutOfRangeException(paramName, value, "Value cannot be zero.");
    }

    public static void ThrowIfNegativeOrZero<T>(T value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
    {
        if (value.CompareTo(default!) <= 0)
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be a positive number.");
    }
}

