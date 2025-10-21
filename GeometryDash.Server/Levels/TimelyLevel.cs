using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Levels;
public class TimelyLevel : Level
{
    [SetsRequiredMembers]
    protected internal TimelyLevel(LevelResponse response) : base(response)
    {
        Number = response.TimelyNumber!.Value;
    }

    public required uint Number { get; set; }
}
