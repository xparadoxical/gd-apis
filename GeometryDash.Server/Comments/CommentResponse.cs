using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Comments;

[Separator(Prop = "~", ListItem = "|"), Keyed]
public partial class CommentResponse
{
    [Index(2)]
    [Base64Encode]
    public string Comment { get; set; }

    [Index(4)]
    public uint Likes { get; set; }

    [Index(6)]
    public uint Id { get; set; }

    [Index(9)]
    public TimeSpan Age { get; set; }
}
