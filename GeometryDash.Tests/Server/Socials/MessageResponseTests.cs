using GeometryDash.Server.Socials;

namespace GeometryDash.Tests.Server.Socials;
public class MessageResponseTests : SerializationTest
{
    [Fact]
    public void Deserialization_Works()
    {
        TestDeserialization<MessageResponse>([
                "6:maximo21009:3:245093986:2:29292372:1:83299152:4:ZQ==:8:1:9:0:7:1 year"u8.ToArray(),
                "6:maximo21009:3:245093986:2:29292372:1:83299152:4:ZQ==:8:1:9:0:5:WV0=:7:1 year"u8.ToArray()
            ], [
                new()
                {
                    MessageId = 83299152,
                    OtherUserAccountId = 29292372,
                    OtherUserPlayerId = 245093986,
                    Title = "e",
                    OtherUserName = "maximo21009",
                    Age = TimeSpan.FromDays(365),
                    IsRead = true,
                    IsIncoming = false
                },
                new()
                {
                    MessageId = 83299152,
                    OtherUserAccountId = 29292372,
                    OtherUserPlayerId = 245093986,
                    Title = "e",
                    Message = "hi",
                    OtherUserName = "maximo21009",
                    Age = TimeSpan.FromDays(365),
                    IsRead = true,
                    IsIncoming = false
                }
            ]);
    }
}
