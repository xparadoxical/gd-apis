using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Users;

[Separator(Prop = ":", ListItem = "|"), Keyed]
public partial class UserStatsResponse : PreviewedUserResponse
{
    [Index(3)]
    public uint? Stars { get; set; }

    [Index(4)]
    public ushort? Demons { get; set; }

    [Index(8)]
    public ushort? CreatorPoints { get; set; }

    [Index(13)]
    public ushort? SecretCoins { get; set; }

    [Index(17)]
    public uint? UserCoins { get; set; }

    [Index(46)]
    public uint? Diamonds { get; set; }

    [Index(52)]
    public uint? Moons { get; set; }
}
