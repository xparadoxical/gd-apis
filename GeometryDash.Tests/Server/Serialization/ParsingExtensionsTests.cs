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

    private enum ShortEnum : short { A = -1 }
    private enum UShortEnum : ushort { A }
}
