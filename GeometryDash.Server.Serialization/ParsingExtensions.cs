using System.Buffers;
using System.Buffers.Binary;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;

using CommunityToolkit.HighPerformance;

namespace GeometryDash.Server.Serialization;
public static class ParsingExtensions
{
    public static T Parse<T>(this ReadOnlySpan<byte> input) where T : INumberBase<T>
        => T.Parse(input, NumberStyles.AllowLeadingSign, null);

    /// <summary>An always-throwing version of <see cref="IUtf8SpanFormattable.TryFormat(Span{byte}, out int, ReadOnlySpan{char}, IFormatProvider?)"/>.</summary>
    /// <returns>A part of <paramref name="output"/> containing just the formatted <paramref name="value"/>.</returns>
    internal static Span<byte> Format<T>(this T value, Span<byte> output) where T : IUtf8SpanFormattable
    {
        if (value.TryFormat(output, out var written, default, null))
            return output[..written];

        throw new ArgumentException("Not enough space in the output buffer.");
    }

    public static void WriteUtf8<T>(this IBufferWriter<byte> writer, T value) where T : INumberBase<T>
    {
        Span<byte> buf = stackalloc byte[20]; //max ulong/long length
        writer.Write(value.Format(buf));
    }

    public delegate R ReadOnlySpanFunc<E, R>(ReadOnlySpan<E> span);

    public static T ToUtf8<T>(this string s, ReadOnlySpanFunc<byte, T> consumer)
    {
        var buf = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(s.Length));

        var status = Utf8.FromUtf16(s, buf, out _, out var bytesWritten, false);
        if (status is not OperationStatus.Done)
        {
            ArrayPool<byte>.Shared.Return(buf);
            ThrowHelpers.OperationStatusUnsuccessful(status);
        }

        var inst = consumer(buf.AsSpan(..bytesWritten));

        ArrayPool<byte>.Shared.Return(buf);
        return inst;
    }

    public static unsafe T ToUtf8<T>(this string s, delegate*<ReadOnlySpan<byte>, T> consumer)
    {
        var buf = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(s.Length));

        var status = Utf8.FromUtf16(s, buf, out _, out var bytesWritten, false);
        if (status is not OperationStatus.Done)
        {
            ArrayPool<byte>.Shared.Return(buf);
            ThrowHelpers.OperationStatusUnsuccessful(status);
        }

        var inst = consumer(buf.AsSpan(..bytesWritten));

        ArrayPool<byte>.Shared.Return(buf);
        return inst;
    }

    public static bool ParseBool(this ReadOnlySpan<byte> input, char trueValue, char falseValue)
    {
        if (input.Length is not 1)
            throw new ArgumentException("Length of the span was not 1.", nameof(input));

        var c = (char)input[0];

        if (c != trueValue && c != falseValue)
            throw new ArgumentOutOfRangeException(nameof(input), c, $"Expected '{trueValue}' or '{falseValue}'");

        return c == trueValue;
    }

    /// <summary>Parses a span of bytes to a <see langword="bool"/>. The <see langword="false"/> value is an empty span.</summary>
    public static bool ParseBool(this ReadOnlySpan<byte> input, char trueValue)
    {
        if (input.Length is 0)
            return false;
        else if (input.Length is > 1)
            throw new ArgumentException("Length of the span was greater than 1.", nameof(input));

        var c = (char)input[0];

        if (c != trueValue)
            throw new ArgumentOutOfRangeException(nameof(input), c, $"Expected '{trueValue}' or an empty span");

        return true;
    }

    public static T ParseEnum<T>(this ReadOnlySpan<byte> input) where T : struct, Enum
    {
        ulong value = EnumRangeCheck<T>(input.Parse<long>());

        T enumValue = BitConverter.IsLittleEndian switch
        {
            true => Unsafe.As<ulong, T>(ref value),
            false => Unsafe.As<ulong, T>(ref Unsafe.AsRef(BinaryPrimitives.ReverseEndianness(value)))
        };

        if (!Enum.IsDefined(enumValue))
            ThrowUndefinedEnumValue(enumValue);

        return enumValue;
    }

    private static ulong EnumRangeCheck<T>(long toConvert) where T : struct, Enum
    {
        return EnumTypeInfo<T>.TypeCode switch
        {
            TypeCode.Int32 => (ulong)checked((int)toConvert),
            TypeCode.UInt32 => checked((uint)toConvert),
            TypeCode.UInt64 => checked((ulong)toConvert),
            TypeCode.Int64 => (ulong)toConvert,
            TypeCode.SByte => (ulong)checked((sbyte)toConvert),
            TypeCode.Byte => checked((byte)toConvert),
            TypeCode.Int16 => (ulong)checked((short)toConvert),
            TypeCode.UInt16 => checked((ushort)toConvert),
            var typeCode => ThrowEnumNotSupported(typeCode)
        };
    }

    private static void ThrowUndefinedEnumValue<T>(T value) where T : struct, Enum
        => throw new ArgumentException($"Undefined enum value: {value}");

    private static ulong ThrowEnumNotSupported(TypeCode tc)
        => throw new NotSupportedException($"Enums with underlying type {tc} are not supported.");

    public static TimeSpan ParseTimeSpan(this ReadOnlySpan<byte> input)
    {
        var space = input.IndexOf((byte)' ');
        if (space is not 1 or 2)
            throw new FormatException("");

        var count = input[..space].Parse<byte>();
        var plural = input[^1] == (byte)'s';
        //valid if (>1) == (has -s)
        if (count > 1 ^ plural)
            throw new FormatException("Mismatched grammatical number.");

        var unit = input[(space + 1)..^(plural ? 1 : 0)];

        //PERF bench how bad all of this is //mapperTable.IndexOf(input)?
        if (unit[0] is (byte)'m')
        {
            if (unit.SequenceEqual("minute"u8))
                return TimeSpan.FromMinutes(count);
            else if (unit.SequenceEqual("month"u8))
                return TimeUnit.Month * count;
        }
        else if (unit.SequenceEqual("second"u8))
            return TimeSpan.FromSeconds(count);
        else if (unit.SequenceEqual("hour"u8))
            return TimeSpan.FromHours(count);
        else if (unit.SequenceEqual("day"u8))
            return TimeSpan.FromDays(count);
        else if (unit.SequenceEqual("week"u8))
            return TimeUnit.Week * count;
        else if (unit.SequenceEqual("year"u8))
            return TimeUnit.Year * count;

        throw new FormatException("Unknown time unit.");
    }
}
