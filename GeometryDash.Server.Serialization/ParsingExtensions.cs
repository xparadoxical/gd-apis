using System.Buffers.Binary;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

using CommunityToolkit.HighPerformance;

namespace GeometryDash.Server.Serialization;
public static class ParsingExtensions
{
    public static T Parse<T>(this ReadOnlySpan<byte> input) where T : INumberBase<T>
        => T.Parse(input, NumberStyles.AllowLeadingSign, null);

    public static bool ParseBool(this ReadOnlySpan<byte> input, OptionalRef<ReadOnlySpan<byte>> trueValue, OptionalRef<ReadOnlySpan<byte>> falseValue)
    {
        if (!trueValue.HasValue && !falseValue.HasValue)
            throw new ArgumentException($"{nameof(trueValue)} or {nameof(falseValue)} must have a value.");

        if (trueValue.HasValue && falseValue.HasValue && trueValue.Value.SequenceEqual(falseValue))
            throw new ArgumentException($"{nameof(trueValue)} and {nameof(falseValue)} cannot be equal.");

        var equalToTrue = trueValue.HasValue && input.SequenceEqual(trueValue);
        var equalToFalse = falseValue.HasValue && input.SequenceEqual(falseValue);

        if (equalToTrue || (!trueValue.HasValue && falseValue.HasValue && !equalToFalse))
            return true;

        if (equalToFalse || (!falseValue.HasValue && trueValue.HasValue && !equalToTrue))
            return false;

        throw new ArgumentException("Unhandled value.", nameof(input));
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
        return Type.GetTypeCode(typeof(T)) switch
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

        var amount = input[..space].Parse<byte>();
        var plural = input[^1] == (byte)'s';
        //valid if (>1) == (has -s)
        if (amount > 1 ^ plural)
            throw new FormatException("Mismatched grammatical number.");

        var unit = input[(space + 1)..^(plural ? 1 : 0)];

        //PERF bench how bad all of this is //mapperTable.IndexOf(input)?
        if (unit[0] is (byte)'m')
        {
            if (unit.SequenceEqual("minute"u8))
                return TimeSpan.FromMinutes(amount);
            else if (unit.SequenceEqual("month"u8))
                return TimeUnit.Month * amount;
        }
        else if (unit.SequenceEqual("second"u8))
            return TimeSpan.FromSeconds(amount);
        else if (unit.SequenceEqual("hour"u8))
            return TimeSpan.FromHours(amount);
        else if (unit.SequenceEqual("day"u8))
            return TimeSpan.FromDays(amount);
        else if (unit.SequenceEqual("week"u8))
            return TimeUnit.Week * amount;
        else if (unit.SequenceEqual("year"u8))
            return TimeUnit.Year * amount;

        throw new FormatException("Unknown time unit.");
    }
}
