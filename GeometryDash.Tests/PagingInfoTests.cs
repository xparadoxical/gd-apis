using System.Globalization;

namespace GeometryDash.Tests;

public class PagingInfoTests
{
    [Theory]
    [InlineData(1, 4294967295, 0)]
    public void ToString_Works(uint results, uint pageIndex, uint pageSize)
    {
        var info = new PagingInfo(results, pageIndex, pageSize);
        var expected = string.Join(':', results, pageIndex, pageSize);

        Assert.Equal(expected, info.ToString());
    }

    [Fact]
    public void Parse_NonNullFormatProvider_IsIgnored()
    {
        var input = "1:1:1";
        IFormatProvider provider = CultureInfo.InvariantCulture;

        PagingInfo.Parse(input, provider);
        Assert.True(PagingInfo.TryParse(input, provider, out var _));
    }

    [Theory]
    [InlineData(1, 1, 1)]
    public void Parse_Works(uint results, uint pageIndex, uint pageSize)
    {
        var input = string.Join(':', results, pageIndex, pageSize);
        var expected = new PagingInfo(results, pageIndex, pageSize);

        Assert.Equal(expected, PagingInfo.Parse(input));

        Assert.True(PagingInfo.TryParse(input, null, out var output));
        Assert.Equal(expected, output);
    }

    [Theory]
    [InlineData((string?)null)]
    [InlineData("")]
    [InlineData(":")]
    [InlineData("::")]
    [InlineData("0:0:+1")]
    public void Parse_WithInvalidInputs_Fails(string input)
    {
        Assert.ThrowsAny<Exception>(() => PagingInfo.Parse(input));
        Assert.False(PagingInfo.TryParse(input, null, out var _));
    }
}