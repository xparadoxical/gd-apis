using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Users;

[Separator(Prop = ":", ListItem = "|"), Keyed]
public partial class GJLeaderboardUser : GJUserStats
{
    [Index(6)]
    public uint? LeaderboardPosition { get; set; }
}
