using System.Diagnostics.CodeAnalysis;

using GeometryDash.Server.Http.temp;

namespace GeometryDash.Server;
public class LevelComment : ColoredComment
{
    [SetsRequiredMembers]
    protected internal LevelComment(GJLevelComment response) : base(response)
    {
        Spam = response.Spam!.Value;
        Percent = response.Percent;
    }

    public required bool Spam { get; set; }
    public required byte? Percent { get; set; }
}
