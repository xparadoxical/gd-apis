using GeometryDash.Server.Users;

namespace GeometryDash.Tests.Server;
public class PagedDataTests
{
    [Theory]
    [InlineData("1:a:10:1:11:2:16:3#7:8:9")]
    public void Parse_Works(string s)
    {
        var expected = new PagedData<UserResponse>()
        {
            Data = new()
            {
                Username = "a",
                PlayerColor1 = 1,
                PlayerColor2 = 2,
                AccountId = 3
            },
            Paging = new(7, 8, 9)
        };

        var parsed = PagedData<UserResponse>.Parse(Utf8(s));
        var success = PagedData<UserResponse>.TryParse(Utf8(s), ret: out var tryParsed);

        Assert.Multiple(() => Assert.Equivalent(expected, parsed, true),

            () => Assert.True(success),
            () => Assert.Equivalent(expected, tryParsed, true)
        );
    }
}
