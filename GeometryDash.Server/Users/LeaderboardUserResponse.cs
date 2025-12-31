using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Users;

[Separator(Prop = ":", ListItem = "|"), Keyed]
public partial class LeaderboardUserResponse : UserStatsResponse
{
    [Index(6)]
    public uint? LeaderboardPosition { get; set; }
}
