using GeometryDash.Server.Serialization;
using GeometryDash.Server.Users;

namespace GeometryDash.Server.Comments;

[Separator(Prop = "~", ListItem = "|"), Keyed]
public sealed partial class CommentResponse
{
    [Index(1)]
    public uint? LevelId { get; set; }

    [Index(2)]
    [Base64Encode]
    public string Comment { get; set; }

    [Index(3)]
    public uint? PlayerId { get; set; }

    [Index(4)]
    public uint Likes { get; set; }

    [Index(6)]
    public uint Id { get; set; }

    [Index(7)]
    [Bool('1', False = '0')]
    public bool? Spam { get; set; }

    [Index(9)]
    public TimeSpan Age { get; set; }

    [Index(10)]
    [CoalesceToNull(0)]
    public byte? Percent { get; set; }

    [Index(11)]
    [EmptyDefaultsTo(Users.ModeratorStatus.None)]
    public ModeratorStatus ModeratorStatus { get; set; }

    [Index(12)]
    public Color? Color { get; set; }
}
