using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Levels;

[Separator(Prop = ":", ListItem = "|"), Keyed]
public partial class TimelyLevelResponse : LevelResponse
{
    [Index(41)]
    public ushort? TimelyNumber { get; set; }
}
