using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Comments;
public class ListComment : ColoredComment
{
    [SetsRequiredMembers]
    protected internal ListComment(CommentResponse response) : base(response)
    {
        Spam = response.Spam!.Value;
    }

    public required bool Spam { get; set; }
}
