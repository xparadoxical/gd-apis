using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Users;

[Separator(Prop = ':', ListItem = '|'), Keyed]
public sealed partial class UserResponse : ISerializable<UserResponse>
{
    [Index(1)]
    public string Username { get; set; }

    [Index(2)]
    public uint? PlayerId { get; set; }

    [Index(3)]
    public uint? Stars { get; set; }

    [Index(4)]
    public ushort? Demons { get; set; }

    //5 missing

    /// <summary>Leaderboard type depends on context.</summary>
    [Index(6)]
    public uint? LeaderboardPosition { get; set; }

    /// <summary>Player's leaderboard entry is highlighted if <c><see cref="AccountIdHighlight"/> == <see cref="AccountId"/></c></summary>
    [Index(7)]
    public uint? AccountIdHighlight { get; set; }

    [Index(8)]
    public ushort? CreatorPoints { get; set; }

    [Index(9)]
    public uint? ShowcaseIconId { get; set; }

    [Index(10)]
    public byte PlayerColor1 { get; set; }

    [Index(11)]
    public byte PlayerColor2 { get; set; }

    //12 missing

    [Index(13)]
    public ushort? SecretCoins { get; set; }

    [Index(14)]
    public GameMode? ShowcaseIconType { get; set; }

    [Index(15)]
    [Bool('2', False = '0')]
    public bool? HasGlow { get; set; }

    [Index(16)]
    [CoalesceToNull(0)]
    public uint? AccountId { get; set; }

    [Index(17)]
    public uint? UserCoins { get; set; }

    [Index(18)]
    public PrivacyGroup? AllowMessagesFrom { get; set; }

    [Index(19)]
    [Bool('0', False = '1')]
    public bool? AllowFriendRequests { get; set; }

    [Index(20)]
    public string? YouTubeChannelId { get; set; }

    [Index(21)]
    public ushort? CubeId { get; set; }

    [Index(22)]
    public ushort? ShipId { get; set; }

    [Index(23)]
    public ushort? BallId { get; set; }

    [Index(24)]
    public ushort? UfoId { get; set; }

    [Index(25)]
    public ushort? WaveId { get; set; }

    [Index(26)]
    public ushort? RobotId { get; set; }

    //public ushort? TrailId { get; set; } //27 never sent

    //28 == 15

    public bool IsRegistered => AccountId is not null; //29, no point in deserializing

    [Index(30)]
    [CoalesceToNull(0)]
    public uint? GlobalLeaderboardPosition { get; set; }

    [Index(31)]
    [EmptyDefaultsTo(Users.FriendState.None)]
    public FriendState? FriendState { get; set; }

    [Index(32)]
    public uint? FriendRequestId { get; set; }

    //33-34 missing

    [Index(35)]
    [Base64Encode]
    public string? FriendRequestMessage { get; set; }

    //36 missing

    [Index(37)]
    public TimeSpan? FriendRequestAge { get; set; }

    [Index(38)]
    public byte? IncomingMessageCount { get; set; }

    [Index(39)]
    public byte? IncomingFriendRequestCount { get; set; }

    [Index(40)]
    public byte? NewFriendsCount { get; set; }

    [Index(41)]
    [Bool('1')]
    public bool? IsNewFriendOrRequest { get; set; }

    [Index(42)]
    public TimeSpan? ScoreAge { get; set; }

    [Index(43)]
    public ushort? SpiderId { get; set; }

    [Index(44)]
    public string? TwitterUsername { get; set; }

    [Index(45)]
    public string? TwitchUsername { get; set; }

    [Index(46)]
    public uint? Diamonds { get; set; }

    //47 missing

    [Index(48)]
    public byte? ExplosionId { get; set; }

    [Index(49)]
    public ModeratorStatus? ModeratorStatus { get; set; }

    [Index(50)]
    public PrivacyGroup? CommentHistoryPublicTo { get; set; }

    [Index(51)]
    public byte? GlowColor { get; set; }

    [Index(52)]
    public uint? Moons { get; set; }

    [Index(53)]
    public ushort? SwingId { get; set; }

    [Index(54)]
    public ushort? JetpackId { get; set; }
}

public enum GameMode : byte
{
    Cube,
    Ship,
    Ball,
    Ufo,
    Wave,
    Robot,
    Spider
}

public enum PrivacyGroup : byte
{
    Everyone,
    Friends,
    Private
}

public enum FriendState : byte
{
    None = 0,
    Friend = 1,
    //2 unknown
    RequestSent = 3,
    RequestReceived = 4
}

public enum ModeratorStatus : byte
{
    None,
    Moderator,
    ElderModerator,
    /// <summary>This value is visible only in <see cref="UserInfo"/>.</summary>
    LeaderboardModerator
}
