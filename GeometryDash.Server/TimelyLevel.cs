using System.Diagnostics.CodeAnalysis;

using GeometryDash.Server.Http.Levels;

namespace GeometryDash.Server;
public class TimelyLevel : Level
{
    [SetsRequiredMembers]
    protected internal TimelyLevel(GJTimelyLevel response) : base(response)
    {
        Number = response.TimelyNumber!.Value;
    }

    public required uint Number { get; set; }
}
