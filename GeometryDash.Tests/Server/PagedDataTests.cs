using GeometryDash.Server.Users;

namespace GeometryDash.Tests.Server;
public class PagedDataTests : SerializationTest
{
    [Fact]
    public void Deserialize_Works()
    {
        TestDeserialization<PagedData<UserResponse>>(
            "1:a:10:1:11:2:16:3#7:8:9"u8.ToArray(),
            new(new()
            {
                Username = "a",
                PlayerColor1 = 1,
                PlayerColor2 = 2,
                AccountId = 3
            },
            new(7, 8, 9)));
    }
}
