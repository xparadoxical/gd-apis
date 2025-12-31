using GeometryDash.Server.Serialization;
using GeometryDash.Server.Users;

namespace GeometryDash.Server.Comments;

[Separator(Prop = "~", ListItem = "|"), Keyed]
public partial class ColoredCommentResponse : CommentResponse
{
    [Index(3)]
    public uint? PlayerId { get; set; }

    [Index(11)]
    public Optional<ModeratorStatus>? ModeratorStatus { get; set; }

    [Index(12)]
    public Color? Color { get; set; }
}
