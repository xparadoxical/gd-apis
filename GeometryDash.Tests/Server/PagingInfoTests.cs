using System.Globalization;

namespace GeometryDash.Tests.Server;

public class PagingInfoTests
{
    [Fact]
    public void Parse_NonNullFormatProvider_IsIgnored()
    {
        var input = Utf8("1:1:1");
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

        PagingInfo output = default;
        Assert.Multiple(
            () => Assert.Equal(expected, PagingInfo.Parse(Utf8(input))),

            () => Assert.True(PagingInfo.TryParse(Utf8(input), null, out output)),
            () => Assert.Equal(expected, output)
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData(":")]
    [InlineData("::")]
    [InlineData("0:0:+1")]
    public void Parse_Fails(string input)
    {
        Assert.Multiple(
            () => Assert.ThrowsAny<Exception>(() => PagingInfo.Parse(Utf8(input))),
            () => Assert.False(PagingInfo.TryParse(Utf8(input), null, out var _))
        );
    }
}
