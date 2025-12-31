using GeometryDash.Server.Comments;
using GeometryDash.Server.Users;

namespace GeometryDash.Tests.Server;
public class CommentResponseTests : SerializationTest
{
    [Fact]
    public void Deserialize_getGJAccountComments_Works()
    {
        TestDeserialization<CommentResponse>("2~eWF5IGkgY29tcGxldGVkIGN1YnN0ZXAhISE=~4~12~9~6 years~6~15118679"u8,
            new()
            {
                Age = TimeUnit.Year * 6,
                Comment = "yay i completed cubstep!!!",
                Id = 15118679,
                Likes = 12
            });
    }

    [Fact]
    public void Deserialize_getGJCommentHistory_Works()
    {
        TestDeserialization<HistoryCommentResponse>("2~Q2FuIHlvdSBoYW5kbGUgdGhlIEthcHBhPw==~1~7485599~3~16~4~164649~10~0~9~8 years~6~42602304~11~2~12~75,255,75"u8,
            new()
            {
                Age = TimeUnit.Year * 8,
                Color = new() { Red = 75, Green = 255, Blue = 75 },
                Comment = "Can you handle the Kappa?",
                Id = 42602304,
                LevelId = 7485599,
                Likes = 164649,
                ModeratorStatus = ModeratorStatus.ElderModerator,
                PlayerId = 16
            });
    }

    [Fact]
    public void Deserialize_getGJCommentsForLevel_Works()
    {
        TestDeserialization<LevelCommentResponse>([
                "2~TGlrZSBTaSBIYWJsYXMgRXNwYW5pb2wgTWkgUGFyY2UgOid2~3~276721746~4~114~7~1~10~0~9~4 months~6~17125045"u8.ToArray(),
                "2~V2hlcmUncyB0aGUgUmVhcGVyIExldmlhdGhhbj8=~3~247848956~4~37~7~0~10~0~9~1 week~6~32920511"u8.ToArray()
            ], [
                new()
                {
                    Age = TimeUnit.Month * 4,
                    Comment = "Like Si Hablas Espaniol Mi Parce :'v",
                    Id = 17125045,
                    Likes = 114,
                    PlayerId = 276721746,
                    Spam = true,
                    Percent = null // Key 10~0 present in input, but CoalesceToNull(0) makes it null
                },
                new()
                {
                    Age = TimeUnit.Week,
                    Comment = "Where's the Reaper Leviathan?",
                    Id = 32920511,
                    Likes = 37,
                    PlayerId = 247848956,
                    Spam = false,
                    Percent = null // Key 10~0 present in input, but CoalesceToNull(0) makes it null
                }
            ]);
    }
}
