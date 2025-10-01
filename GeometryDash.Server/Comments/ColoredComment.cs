using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Comments;
public class ColoredComment : Comment
{
    [SetsRequiredMembers]
    protected internal ColoredComment(CommentResponse response) : base(response)
    {
        Color = response.Color;
    }

    public required CommentColor? Color { get; set; }
}
