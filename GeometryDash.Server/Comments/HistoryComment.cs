using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Comments;
public class HistoryComment : ColoredComment
{
    [SetsRequiredMembers]
    protected internal HistoryComment(CommentResponse response) : base(response)
    {
        LevelId = response.LevelId!.Value;
        Percent = response.Percent!.Value;
    }

    public required uint LevelId { get; set; }
    public required byte Percent { get; set; }
}
