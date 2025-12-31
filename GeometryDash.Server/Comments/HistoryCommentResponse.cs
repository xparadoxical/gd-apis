using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Comments;

[Separator(Prop = "~", ListItem = "|"), Keyed]
public partial class HistoryCommentResponse : ColoredCommentResponse
{
    [Index(1)]
    public uint? LevelId { get; set; }

    [Index(10)]
    [CoalesceToNull(0)]
    public byte? Percent { get; set; }
}
