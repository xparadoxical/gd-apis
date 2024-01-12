using System.Text;

using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Users;
public class UserResponse : ISerializable<UserResponse>
{
    public required string UserName { get; set; }
    public uint? PlayerId { get; set; }
    public uint? Stars { get; set; }
    public ushort? Demons { get; set; }
    //5 missing
    /// <summary>Leaderboard type depends on context.</summary>
    public uint? LeaderboardPosition { get; set; }
    /// <summary>Player's leaderboard entry is highlighted if <c><see cref="AccountIdHighlight"/> == <see cref="AccountId"/></c></summary>
    public uint? AccountIdHighlight { get; set; }
    public ushort? CreatorPoints { get; set; }
    public uint? ShowcaseIconId { get; set; }
    public required byte PlayerColor1 { get; set; }
    public required byte PlayerColor2 { get; set; }
    //12 missing
    public ushort? SecretCoins { get; set; }
    public GameMode? ShowcaseIconType { get; set; }
    public bool? HasGlow { get; set; }
    public required uint AccountId { get; set; }
    public uint? UserCoins { get; set; }
    public PrivacyGroup? AllowMessagesFrom { get; set; }
    public bool? AllowFriendRequests { get; set; }
    public LinkedServiceProfile? YouTube { get; set; }
    public ushort? CubeId { get; set; }
    public ushort? ShipId { get; set; }
    public ushort? BallId { get; set; }
    public ushort? UfoId { get; set; }
    public ushort? WaveId { get; set; }
    public ushort? RobotId { get; set; }
    //public ushort? TrailId { get; set; } //27 never sent
    //28 == 15
    /// <summary>Probably equivalent to <c><see cref="AccountId"/> != 0</c></summary>
    public bool? IsRegistered { get; set; }
    public uint? GlobalLeaderboardPosition { get; set; }
    public FriendState? FriendState { get; set; }
    public uint? FriendRequestId { get; set; }
    //33-34 missing
    public string? FriendRequestMessage { get; set; }
    //36 missing
    public TimeSpan? FriendRequestAge { get; set; }
    public byte? IncomingMessageCount { get; set; }
    public byte? IncomingFriendRequestCount { get; set; }
    public byte? NewFriendsCount { get; set; }
    public bool? IsNewFriendOrRequest { get; set; }
    public TimeSpan? ScoreAge { get; set; }
    public ushort? SpiderId { get; set; }
    public LinkedServiceProfile? Twitter { get; set; }
    public LinkedServiceProfile? Twitch { get; set; }
    public uint? Diamonds { get; set; }
    //47 missing
    public byte? ExplosionId { get; set; }
    public ModeratorStatus? ModeratorStatus { get; set; }
    public PrivacyGroup? CommentHistoryPublicTo { get; set; }
    public byte? GlowColor { get; set; }
    public uint? Moons { get; set; }
    public ushort? SwingId { get; set; }
    public ushort? JetpackId { get; set; }

    public static SerializationOptions Options { get; } = new(true);
    public static SerializationLogic<UserResponse> SerializationLogic { get; } = new SerializationLogicBuilder<UserResponse>(43)
        .Deserializer(1, (input, inst) => inst.Value.UserName = Encoding.UTF8.GetString(input))
        .Deserializer(2, (input, inst) => inst.Value.PlayerId = input.Parse<uint>())
        .Deserializer(3, (input, inst) => inst.Value.Stars = input.Parse<uint>())
        .Deserializer(4, (input, inst) => inst.Value.Demons = input.Parse<ushort>())
        .Deserializer(6, (input, inst) => inst.Value.LeaderboardPosition = input.Parse<uint>())
        .Deserializer(7, (input, inst) => inst.Value.AccountIdHighlight = input.Parse<uint>())
        .Deserializer(8, (input, inst) => inst.Value.CreatorPoints = input.Parse<ushort>())
        .Deserializer(9, (input, inst) => inst.Value.ShowcaseIconId = input.Parse<uint>())
        .Deserializer(10, (input, inst) => inst.Value.PlayerColor1 = input.Parse<byte>())
        .Deserializer(11, (input, inst) => inst.Value.PlayerColor2 = input.Parse<byte>())
        .Deserializer(13, (input, inst) => inst.Value.SecretCoins = input.Parse<ushort>())
        .Deserializer(14, (input, inst) => inst.Value.ShowcaseIconType = input.ParseEnum<GameMode>())
        .Deserializer(15, (input, inst) => inst.Value.HasGlow = input.ParseBool('2', '0'))
        .Deserializer(16, (input, inst) => inst.Value.AccountId = input.Parse<uint>())
        .Deserializer(17, (input, inst) => inst.Value.UserCoins = input.Parse<uint>())
        .Deserializer(18, (input, inst) => inst.Value.AllowMessagesFrom = input.ParseEnum<PrivacyGroup>())
        .Deserializer(19, (input, inst) => inst.Value.AllowFriendRequests = input.ParseBool('0', '1'))
        .Deserializer(20, (input, inst) => inst.Value.YouTube = new(LinkedServiceProfile.YouTube, Encoding.UTF8.GetString(input)))
        .Deserializer(21, (input, inst) => inst.Value.CubeId = input.Parse<ushort>())
        .Deserializer(22, (input, inst) => inst.Value.ShipId = input.Parse<ushort>())
        .Deserializer(23, (input, inst) => inst.Value.BallId = input.Parse<ushort>())
        .Deserializer(24, (input, inst) => inst.Value.UfoId = input.Parse<ushort>())
        .Deserializer(25, (input, inst) => inst.Value.WaveId = input.Parse<ushort>())
        .Deserializer(26, (input, inst) => inst.Value.RobotId = input.Parse<ushort>())
        .Deserializer(28, (input, inst) => inst.Value.HasGlow = input.ParseBool('1', '0'))
        .Deserializer(29, (input, inst) => inst.Value.IsRegistered = input.ParseBool('1', '0'))
        .Deserializer(30, (input, inst) => inst.Value.GlobalLeaderboardPosition = input.Parse<uint>() is var i && i == 0 ? null : i)
        .Deserializer(31, (input, inst) => inst.Value.FriendState = input.Length is 0 ? Users.FriendState.None : input.ParseEnum<FriendState>())
        .Deserializer(32, (input, inst) => inst.Value.FriendRequestId = input.Parse<uint>())
        .Deserializer(35, (input, inst) => inst.Value.FriendRequestMessage = Base64.Decode(input))
        .Deserializer(37, (input, inst) => inst.Value.FriendRequestAge = input.ParseTimeSpan())
        .Deserializer(38, (input, inst) => inst.Value.IncomingMessageCount = input.Parse<byte>())
        .Deserializer(39, (input, inst) => inst.Value.IncomingFriendRequestCount = input.Parse<byte>())
        .Deserializer(40, (input, inst) => inst.Value.NewFriendsCount = input.Parse<byte>())
        .Deserializer(41, (input, inst) => inst.Value.IsNewFriendOrRequest = input.ParseBool('1'))
        .Deserializer(42, (input, inst) => inst.Value.ScoreAge = input.ParseTimeSpan())
        .Deserializer(43, (input, inst) => inst.Value.SpiderId = input.Parse<ushort>())
        .Deserializer(44, (input, inst) => inst.Value.Twitter = new(LinkedServiceProfile.Twitter, Encoding.UTF8.GetString(input)))
        .Deserializer(45, (input, inst) => inst.Value.Twitch = new(LinkedServiceProfile.Twitch, Encoding.UTF8.GetString(input)))
        .Deserializer(46, (input, inst) => inst.Value.Diamonds = input.Parse<uint>())
        .Deserializer(48, (input, inst) => inst.Value.ExplosionId = input.Parse<byte>())
        .Deserializer(49, (input, inst) => inst.Value.ModeratorStatus = input.ParseEnum<ModeratorStatus>())
        .Deserializer(50, (input, inst) => inst.Value.CommentHistoryPublicTo = input.ParseEnum<PrivacyGroup>())
        .Deserializer(51, (input, inst) => inst.Value.GlowColor = input.Parse<byte>())
        .Deserializer(52, (input, inst) => inst.Value.Moons = input.Parse<uint>())
        .Deserializer(53, (input, inst) => inst.Value.SwingId = input.Parse<ushort>())
        .Deserializer(54, (input, inst) => inst.Value.JetpackId = input.Parse<ushort>())
        .Build();
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
    RequestSent = 3,
    RequestReceived = 4
}

public readonly struct LinkedServiceProfile(Uri url, string userName) : IEquatable<LinkedServiceProfile>
{
    public readonly Uri Url = new(url, userName);
    public readonly string UserName = userName;

    public static readonly Uri YouTube = new("https://www.youtube.com/channel/");
    public static readonly Uri Twitch = new("https://twitter.com/");
    public static readonly Uri Twitter = new("https://www.twitch.tv/");

    public bool Equals(LinkedServiceProfile other) => Url == other.Url && UserName == other.UserName;

    public override bool Equals(object? obj) => obj is LinkedServiceProfile other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Url, UserName);

    public static bool operator ==(LinkedServiceProfile left, LinkedServiceProfile right) => left.Equals(right);

    public static bool operator !=(LinkedServiceProfile left, LinkedServiceProfile right) => !(left == right);
}

public enum ModeratorStatus : byte
{
    None,
    Moderator,
    ElderModerator
}
