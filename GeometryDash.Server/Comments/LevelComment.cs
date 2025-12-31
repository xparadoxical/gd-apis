using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Comments;
public class LevelComment : ColoredComment
{
    [SetsRequiredMembers]
    protected internal LevelComment(LevelCommentResponse response) : base(response)
    {
        Spam = response.Spam!.Value;
        Percent = response.Percent;
    }

    public required bool Spam { get; set; }
    public required byte? Percent { get; set; }
}
