using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Users;

[Separator(Prop = ":", ListItem = "|"), Keyed]
public partial class UserInfoResponse : UserStatsResponse
{
    [Index(18)]
    public PrivacyGroup? AllowMessagesFrom { get; set; }

    [Index(19)]
    [Bool(True = "0", False = "1")]
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

    [Index(30)]
    [CoalesceToNull(0)]
    public uint? GlobalLeaderboardPosition { get; set; }

    [Index(43)]
    public ushort? SpiderId { get; set; }

    [Index(44)]
    public string? TwitterUsername { get; set; }

    [Index(45)]
    public string? TwitchUsername { get; set; }

    [Index(48)]
    public byte? ExplosionId { get; set; }

    [Index(49)]
    public ModeratorStatus? ModeratorStatus { get; set; }

    [Index(50)]
    public PrivacyGroup? CommentHistoryPublicTo { get; set; }

    [Index(51)]
    public byte? GlowColor { get; set; }

    [Index(53)]
    public ushort? SwingId { get; set; }

    [Index(54)]
    public ushort? JetpackId { get; set; }
}
