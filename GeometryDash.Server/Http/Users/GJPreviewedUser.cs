using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Http.Users;

[Separator(Prop = ":", ListItem = "|"), Keyed]
public partial class GJPreviewedUser : GJUser
{
    [Index(9)]
    public uint? ShowcaseIconId { get; set; }

    [Index(14)]
    public GameMode? ShowcaseIconType { get; set; }
}
