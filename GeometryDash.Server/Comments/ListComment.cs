using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Comments;
public class ListComment : ColoredComment
{
    [SetsRequiredMembers]
    protected internal ListComment(ListCommentResponse response) : base(response)
    {
        Spam = response.Spam!.Value;
    }

    public required bool Spam { get; set; }
}
