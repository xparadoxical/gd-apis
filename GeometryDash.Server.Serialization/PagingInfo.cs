using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;

namespace GeometryDash.Server.Serialization;
public record struct PagingInfo(uint Results, uint PageIndex, uint PageSize) : ISpanParsable<PagingInfo>
{
    public const NumberStyles NumberStyle = NumberStyles.None;

    /// <summary>Returns the string representation of the <see cref="PagingInfo"/>.</summary>
    public override string ToString() => $"{Results}:{PageIndex}:{PageSize}";

    /// <summary>Parses a span of characters into a <see cref="PagingInfo"/>.</summary>
    /// <param name="s">The span of characters to parse.</param>
    /// <param name="provider">Ignored. Pass <see langword="null"/>.</param>
    public static PagingInfo Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null)
    {
        ValidationHelpers.ThrowIfSpanNullOrEmpty(s);

        var colonIndex = s.IndexOf(":");
        var results = uint.Parse(s.Slice(0, colonIndex), NumberStyle);

        s = s.Slice(colonIndex + 1);
        colonIndex = s.IndexOf(":");
        var pageIndex = uint.Parse(s.Slice(0, colonIndex), NumberStyle);

        var pageSize = uint.Parse(s.Slice(colonIndex + 1), NumberStyle);

        return new(results, pageIndex, pageSize);
    }

    /// <summary>Parses a string into a <see cref="PagingInfo"/>.</summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">Ignored. Pass <see langword="null"/>.</param>
    public static PagingInfo Parse(string s, IFormatProvider? provider = null)
        => Parse((ReadOnlySpan<char>)s, provider);

    /// <summary>Tries to parse a span of characters into a <see cref="PagingInfo"/>.</summary>
    /// <param name="s">The span of characters to parse.</param>
    /// <param name="provider">Ignored. Pass <see langword="null"/>.</param>
    /// <param name="result">When this method returns, contains the result of successfully parsing <paramref name="s"/>,
    /// or an undefined value on failure.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was successfully parsed; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(ReadOnlySpan<char> s, [Optional, DefaultParameterValue(null)] IFormatProvider? provider, [MaybeNullWhen(false)] out PagingInfo result)
    {
        if (ValidationHelpers.IsSpanNullOrEmpty(s))
            goto failure;

        var colonIndex = s.IndexOf(":");
        if (uint.TryParse(s.Slice(0, colonIndex), NumberStyle, null, out uint results) is false)
            goto failure;

        s = s.Slice(colonIndex + 1);
        colonIndex = s.IndexOf(":");
        if (uint.TryParse(s.Slice(0, colonIndex), NumberStyle, null, out var pageIndex) is false
            || uint.TryParse(s.Slice(colonIndex + 1), NumberStyle, null, out var pageSize) is false)
            goto failure;

        result = new(results, pageIndex, pageSize);
        return true;

    failure:
        result = default;
        return false;
    }

    /// <summary>Tries to parse a string into a <see cref="PagingInfo"/>.</summary>
    /// <param name="s">The span of characters to parse.</param>
    /// <param name="provider">Ignored. Pass <see langword="null"/>.</param>
    /// <param name="result">When this method returns, contains the result of successfully parsing <paramref name="s"/>,
    /// or an undefined value on failure.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was successfully parsed; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? s, [Optional, DefaultParameterValue(null)] IFormatProvider? provider, [MaybeNullWhen(false)] out PagingInfo result)
        => TryParse((ReadOnlySpan<char>)s, provider, out result);
}
