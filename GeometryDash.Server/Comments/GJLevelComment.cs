using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Comments;

[Separator(Prop = "~", ListItem = "|"), Keyed]
public partial class GJLevelComment : GJSpamComment
{
    [Index(10)]
    [CoalesceToNull(0)]
    public byte? Percent { get; set; }
}
