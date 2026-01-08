using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Comments;
public class ColoredComment : Comment
{
    [SetsRequiredMembers]
    protected internal ColoredComment(ColoredCommentResponse response) : base(response)
    {
        Color = response.Color;
    }

    public required Color? Color { get; set; }
}
