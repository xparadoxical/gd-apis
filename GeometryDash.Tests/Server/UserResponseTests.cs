using GeometryDash.Server.Users;

namespace GeometryDash.Tests.Server;
public class UserResponseTests : SerializationTest
{
    [Fact]
    public void SerializationLogic_getGJScores_Works()
    {
        TestArrayDeserialization<UserResponse>("""
            1:Robergamer100:2:11386212:13:149:17:2692:6:15209:9:24:10:12:11:12:14:3:15:0:16:2577515:3:10658:8:0:46:8375:4:213|1:SweetNight:2:10115453:13:149:17:800:6:15210:9:17:10:17:11:12:14:6:15:2:16:1625356:3:10658:8:0:46:13601:4:757
            """u8.ToArray(), [
                new()
                {
                    Username = "Robergamer100",
                    PlayerId = 11386212,
                    Stars = 10658,
                    Demons = 213,
                    LeaderboardPosition = 15209,
                    CreatorPoints = 0,
                    ShowcaseIconId = 24,
                    PlayerColor1 = 12,
                    PlayerColor2 = 12,
                    SecretCoins = 149,
                    ShowcaseIconType = GameMode.Ufo,
                    HasGlow = false,
                    AccountId = 2577515,
                    UserCoins = 2692,
                    Diamonds = 8375
                },
                new()
                {
                    Username = "SweetNight",
                    PlayerId = 10115453,
                    Stars = 10658,
                    Demons = 757,
                    LeaderboardPosition = 15210,
                    CreatorPoints = 0,
                    ShowcaseIconId = 17,
                    PlayerColor1 = 17,
                    PlayerColor2 = 12,
                    SecretCoins = 149,
                    ShowcaseIconType = GameMode.Spider,
                    HasGlow = true,
                    AccountId = 1625356,
                    UserCoins = 800,
                    Diamonds = 13601
                }
            ]);
    }

    [Fact]
    public void SerializationLogic_getGJUserInfo_Works()
    {
        TestDeserialization<UserResponse>("""
            1:ViPriN:2:1078150:13:149:17:3555:10:11:11:13:3:33486:46:31038:4:1122:8:281:18:1:19:1:50:2:20:UCUwapObI2gw2Tovu5oj-wng:21:133:22:42:23:32:24:29:25:30:26:16:28:1:43:11:48:1:30:0:16:2795:31::44:vipringd:45:viprin:49:2:29:1
            """u8.ToArray(),
            new()
            {
                Username = "ViPriN",
                PlayerId = 1078150,
                Stars = 33486,
                Demons = 1122,
                CreatorPoints = 281,
                PlayerColor1 = 11,
                PlayerColor2 = 13,
                SecretCoins = 149,
                AccountId = 2795,
                UserCoins = 3555,
                AllowMessagesFrom = PrivacyGroup.Friends,
                AllowFriendRequests = false,
                YouTubeChannelId = "UCUwapObI2gw2Tovu5oj-wng",
                CubeId = 133,
                ShipId = 42,
                BallId = 32,
                UfoId = 29,
                WaveId = 30,
                RobotId = 16,
                HasGlow = true,
                GlobalLeaderboardPosition = null,
                FriendState = FriendState.None,
                SpiderId = 11,
                TwitterUsername = "vipringd",
                TwitchUsername = "viprin",
                Diamonds = 31038,
                ExplosionId = 1,
                ModeratorStatus = ModeratorStatus.ElderModerator,
                CommentHistoryPublicTo = PrivacyGroup.Private
            });
    }

    [Fact]
    public void SerializationLogic_getGJUserList_Works()
    {
        TestArrayDeserialization<UserResponse>("""
            1:Afterfive:2:3543535:9:39:10:20:11:15:14:1:15:2:16:2235541:18:0:41:|1:EndorphinexPL:2:17208997:9:6:10:12:11:3:14:5:15:2:16:5116312:18:0:41:1
            """u8.ToArray(),
            [new()
            {
                Username = "Afterfive",
                PlayerId = 3543535,
                ShowcaseIconId = 39,
                PlayerColor1 = 20,
                PlayerColor2 = 15,
                ShowcaseIconType = GameMode.Ship,
                HasGlow = true,
                AccountId = 2235541,
                AllowMessagesFrom = PrivacyGroup.Everyone,
                IsNewFriendOrRequest = false
            },
            new()
            {
                Username = "EndorphinexPL",
                PlayerId = 17208997,
                ShowcaseIconId = 6,
                PlayerColor1 = 12,
                PlayerColor2 = 3,
                ShowcaseIconType = GameMode.Robot,
                HasGlow = true,
                AccountId = 5116312,
                AllowMessagesFrom = PrivacyGroup.Everyone,
                IsNewFriendOrRequest = true
            }
        ]);
    }

    [Fact]
    public void SerializationLogic_getGJComments_Works()
    {
        TestDeserialization<UserResponse>(
            "1~VaXeN~9~1~10~0~11~3~14~0~15~0~16~7121876"u8.ToArray()
                .Select(c => c == (byte)'~' ? (byte)':' : c).ToArray(), //TODO separator overriding
            new()
            {
                Username = "VaXeN",
                ShowcaseIconId = 1,
                PlayerColor1 = 0,
                PlayerColor2 = 3,
                ShowcaseIconType = GameMode.Cube,
                HasGlow = false,
                AccountId = 7121876
            });
    }

    [Fact]
    public void SerializationLogic_getGJFriendRequests_Works()
    {
        TestDeserialization<PagedData<UserResponse>>([
                "1:EndorphinexPL:2:17208997:9:6:10:12:11:3:14:5:15:2:16:5116312:32:60220714:35:dGVzdA==:41::37:7 minutes#0:0:20"u8.ToArray()
            ], [
                new(new()
                {
                    Username = "EndorphinexPL",
                    PlayerId = 17208997,
                    ShowcaseIconId = 6,
                    PlayerColor1 = 12,
                    PlayerColor2 = 3,
                    ShowcaseIconType = GameMode.Robot,
                    HasGlow = true,
                    AccountId = 5116312,
                    FriendRequestId = 60220714,
                    FriendRequestMessage = "test",
                    FriendRequestAge = TimeSpan.FromMinutes(7),
                    IsNewFriendOrRequest = false
                },
                new(0, 0, 20))
            ]);
    }
}
