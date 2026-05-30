using System.Diagnostics.CodeAnalysis;

using GeometryDash.Server.Http.temp;

namespace GeometryDash.Server;
public class ListComment : ColoredComment
{
    [SetsRequiredMembers]
    protected internal ListComment(GJListComment response) : base(response)
    {
        Spam = response.Spam!.Value;
    }

    public required bool Spam { get; set; }
}
