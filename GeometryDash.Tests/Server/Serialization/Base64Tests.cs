namespace GeometryDash.Tests.Server.Serialization;
public class Base64Tests
{
    [Theory]
    [InlineData("dGVzdA", "test")]
    public void Decode_Works(string input, string expected)
        => ParseAndAssertEqual(input, expected);

    [Theory]
    [InlineData("dGVzdA=", "test")]
    [InlineData("dGVzdA==", "test")]
    [InlineData("dGVzdA=======", "test")]
    public void Decode_IgnoresPaddingChars(string input, string expected)
        => ParseAndAssertEqual(input, expected);

    private void ParseAndAssertEqual(string input, string expected)
    {
        var u8input = Utf8(input);

        var output = Base64.Decode(u8input);

        Assert.Equal(expected, output);
    }
}
