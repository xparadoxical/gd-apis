using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Comments;

[Separator(Prop = "~", ListItem = "|"), Keyed]
public partial class SpamCommentResponse : ColoredCommentResponse
{
    [Index(7)]
    [Bool(True = "1", False = "0")]
    public bool? Spam { get; set; }
}
