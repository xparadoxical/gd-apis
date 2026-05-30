using GeometryDash.Server.Http.Users;
using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Http.temp;

[Separator(Prop = "~", ListItem = "|"), Keyed]
public partial class GJColoredComment : GJComment
{
    [Index(3)]
    public uint? PlayerId { get; set; }

    [Index(11)]
    public Optional<ModeratorStatus>? ModeratorStatus { get; set; }

    [Index(12)]
    public Color? Color { get; set; }
}
