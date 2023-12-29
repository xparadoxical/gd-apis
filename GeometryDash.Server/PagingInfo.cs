using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;

namespace GeometryDash.Server;
public record struct PagingInfo(uint Results, uint PageIndex, uint PageSize) : IUtf8SpanParsable<PagingInfo>
{
    public const NumberStyles NumberStyle = NumberStyles.None;

    /// <summary>Returns the string representation of the <see cref="PagingInfo"/>.</summary>
    public override readonly string ToString() => $"{Results}:{PageIndex}:{PageSize}";

    /// <summary>Parses a span of characters into a <see cref="PagingInfo"/>.</summary>
    /// <param name="s">The span of characters to parse.</param>
    /// <param name="provider">Ignored. Pass <see langword="null"/>.</param>
    public static PagingInfo Parse(ReadOnlySpan<byte> s, IFormatProvider? provider = null)
    {
        ValidationHelpers.ThrowIfSpanNullOrEmpty(s);

        var colonIndex = s.IndexOf((byte)':');
        var results = uint.Parse(s[..colonIndex], NumberStyle);

        s = s.Slice(colonIndex + 1);
        colonIndex = s.IndexOf((byte)':');
        var pageIndex = uint.Parse(s[..colonIndex], NumberStyle);

        var pageSize = uint.Parse(s[(colonIndex + 1)..], NumberStyle);

        return new(results, pageIndex, pageSize);
    }

    /// <summary>Tries to parse a span of characters into a <see cref="PagingInfo"/>.</summary>
    /// <param name="s">The span of characters to parse.</param>
    /// <param name="provider">Ignored. Pass <see langword="null"/>.</param>
    /// <param name="result">When this method returns, contains the result of successfully parsing <paramref name="s"/>,
    /// or an undefined value on failure.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was successfully parsed; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(ReadOnlySpan<byte> s, [Optional, DefaultParameterValue(null)] IFormatProvider? provider, [MaybeNullWhen(false)] out PagingInfo result)
    {
        if (ValidationHelpers.IsSpanNullOrEmpty(s))
            goto failure;

        var colonIndex = s.IndexOf((byte)':');
        if (uint.TryParse(s.Slice(0, colonIndex), NumberStyle, null, out uint results) is false)
            goto failure;

        s = s.Slice(colonIndex + 1);
        colonIndex = s.IndexOf((byte)':');
        if (uint.TryParse(s.Slice(0, colonIndex), NumberStyle, null, out var pageIndex) is false
            || uint.TryParse(s.Slice(colonIndex + 1), NumberStyle, null, out var pageSize) is false)
            goto failure;

        result = new(results, pageIndex, pageSize);
        return true;

    failure:
        result = default;
        return false;
    }
}
