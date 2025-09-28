using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Users;
public class UserInfo : User
{
    [SetsRequiredMembers]
    protected internal UserInfo(UserResponse response) : base(response)
    {
        Stars = response.Stars!.Value;
        Demons = response.Demons!.Value;
        CreatorPoints = response.CreatorPoints!.Value;
        SecretCoins = response.SecretCoins!.Value;
        UserCoins = response.UserCoins!.Value;
        AllowMessagesFrom = response.AllowMessagesFrom!.Value;
        AllowFriendRequests = response.AllowFriendRequests!.Value;
        YouTube = new(LinkedServiceProfile.YouTube, response.YouTubeChannelId!);
        CubeId = response.CubeId!.Value;
        ShipId = response.ShipId!.Value;
        BallId = response.BallId!.Value;
        UfoId = response.UfoId!.Value;
        WaveId = response.WaveId!.Value;
        RobotId = response.RobotId!.Value;
        GlobalLeaderboardPosition = response.GlobalLeaderboardPosition;
        SpiderId = response.SpiderId!.Value;
        Twitter = new(LinkedServiceProfile.Twitter, response.TwitterUsername!);
        Twitch = new(LinkedServiceProfile.Twitch, response.TwitterUsername!);
        Diamonds = response.Diamonds!.Value;
        ExplosionId = response.ExplosionId!.Value;
        ModeratorStatus = response.ModeratorStatus!.Value;
        CommentHistoryPublicTo = response.CommentHistoryPublicTo!.Value;
        GlowColor = response.GlowColor!.Value;
        Moons = response.Moons!.Value;
        SwingId = response.SwingId!.Value;
        JetpackId = response.JetpackId!.Value;
    }

    public required uint Stars { get; set; }
    public required ushort Demons { get; set; }
    public required ushort CreatorPoints { get; set; }
    public required ushort SecretCoins { get; set; }
    public required uint UserCoins { get; set; }
    public required PrivacyGroup AllowMessagesFrom { get; set; }
    public required bool AllowFriendRequests { get; set; }
    public required LinkedServiceProfile YouTube { get; set; }
    public required ushort CubeId { get; set; }
    public required ushort ShipId { get; set; }
    public required ushort BallId { get; set; }
    public required ushort UfoId { get; set; }
    public required ushort WaveId { get; set; }
    public required ushort RobotId { get; set; }
    public required bool IsRegistered { get; set; }
    public required uint? GlobalLeaderboardPosition { get; set; }
    public required ushort SpiderId { get; set; }
    public required LinkedServiceProfile Twitter { get; set; }
    public required LinkedServiceProfile Twitch { get; set; }
    public required uint Diamonds { get; set; }
    public required byte ExplosionId { get; set; }
    public required ModeratorStatus ModeratorStatus { get; set; }
    public required PrivacyGroup CommentHistoryPublicTo { get; set; }
    public required byte GlowColor { get; set; }
    public required uint Moons { get; set; }
    public required ushort SwingId { get; set; }
    public required ushort JetpackId { get; set; }
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
