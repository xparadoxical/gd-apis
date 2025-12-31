using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Users;

[Separator(Prop = ":", ListItem = "|"), Keyed]
public partial class PreviewedUserResponse : UserResponse
{
    [Index(9)]
    public uint? ShowcaseIconId { get; set; }

    [Index(14)]
    public GameMode? ShowcaseIconType { get; set; }
}
