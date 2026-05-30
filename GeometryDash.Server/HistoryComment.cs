using System.Diagnostics.CodeAnalysis;

using GeometryDash.Server.Http.temp;

namespace GeometryDash.Server;
public class HistoryComment : ColoredComment
{
    [SetsRequiredMembers]
    protected internal HistoryComment(GJHistoryComment response) : base(response)
    {
        LevelId = response.LevelId!.Value;
        Percent = response.Percent!.Value;
    }

    public required uint LevelId { get; set; }
    public required byte Percent { get; set; }
}
