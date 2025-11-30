namespace GeometryDash.Tests.Server.Serialization;
public class ParsingExtensionsTests
{
    [Theory]
    [InlineData("-2")]
    [InlineData("-0")]
    public void Parse_WithSignedNumbers_Accepts(string toParse)
    {
        Assert.StrictEqual(int.Parse(toParse), Utf8(toParse).Parse<int>());
    }

    [Theory]
    [InlineData(" 1")]
    [InlineData("1 ")]
    [InlineData("\t1  ")]
    public void Parse_WithUnnecessaryWhitespace_Throws(string toParse)
    {
        Assert.Throws<FormatException>(() => Utf8(toParse).Parse<int>());
    }

    [Theory]
    [InlineData("70000")]
    [InlineData("-70000")]
    public void ParseEnum_ShortForOutOfRangeValues_Throws(string toParse)
    {
        Assert.Throws<OverflowException>(() => Utf8(toParse).ParseEnum<ShortEnum>());
    }

    [Theory]
    [InlineData("-1")]
    public void ParseEnum_UnsignedForNegativeValues_Throws(string toParse)
    {
        Assert.Throws<OverflowException>(() => Utf8(toParse).ParseEnum<UShortEnum>());
    }

    [Theory]
    [InlineData("-1")]
    public void ParseEnum_SignedForNegativeValues_Accepts(string toParse)
    {
        Assert.StrictEqual(ShortEnum.A, Utf8(toParse).ParseEnum<ShortEnum>());
    }

    [Theory]
    [InlineData("1")]
    public void ParseEnum_ForUndefinedEnums_Throws(string toParse)
    {
        Assert.Throws<ArgumentException>(() => Utf8(toParse).ParseEnum<UShortEnum>());
    }

    [Theory]
    [InlineData("1 days")]
    [InlineData("2 month")]
    public void ParseTimeSpan_WithIncorrectUnit_Throws(string input)
    {
        Assert.Throws<FormatException>(() => Utf8(input).ParseTimeSpan());
    }

    [Theory]
    [InlineData("1 day", TimeSpan.TicksPerDay)]
    [InlineData("2 months", TimeSpan.TicksPerDay * 31 * 2)]
    public void ParseTimeSpan_Works(string input, long resultTicks)
    {
        Assert.Equal(TimeSpan.FromTicks(resultTicks), Utf8(input).ParseTimeSpan());
    }

    [Theory]
    [InlineData("5", "5", "0", true)]
    [InlineData("", "", null, true)]
    [InlineData("5", null, "", true)]
    [InlineData("", null, "0", true)]
    [InlineData("00", null, "00", false)]
    [InlineData("00", "0", "00", false)]
    [InlineData("00", "1", null, false)]
    [InlineData("", "1", "", false)]
    public void ParseBool_Works(string input, string? trueValue, string? falseValue, bool expected)
    {
        OptionalRef<ReadOnlySpan<byte>> optionalTrueValue = trueValue is null ? default : new(Utf8(trueValue));
        OptionalRef<ReadOnlySpan<byte>> optionalFalseValue = falseValue is null ? default : new(Utf8(falseValue));

        Assert.Equal(expected, ParsingExtensions.ParseBool(Utf8(input), optionalTrueValue, optionalFalseValue));
    }

    [Theory]
    [InlineData("", "", "")]
    [InlineData("", null, null)]
    [InlineData("w", "t", "f")]
    [InlineData("w", "t", "")]
    [InlineData("w", "", "f")]
    public void ParseBool_Throws(string input, string? trueValue, string? falseValue)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            OptionalRef<ReadOnlySpan<byte>> optionalTrueValue = trueValue is null ? default : new(Utf8(trueValue));
            OptionalRef<ReadOnlySpan<byte>> optionalFalseValue = falseValue is null ? default : new(Utf8(falseValue));

            return ParsingExtensions.ParseBool(Utf8(input), optionalTrueValue, optionalFalseValue);
        });
    }

    private enum ShortEnum : short { A = -1 }
    private enum UShortEnum : ushort { A }
}
