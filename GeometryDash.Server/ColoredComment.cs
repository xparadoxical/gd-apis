using System.Diagnostics.CodeAnalysis;

using GeometryDash.Server.Http.temp;

namespace GeometryDash.Server;
public class ColoredComment : Comment
{
    [SetsRequiredMembers]
    protected internal ColoredComment(GJColoredComment response) : base(response)
    {
        Color = response.Color;
    }

    public required Color? Color { get; set; }
}
