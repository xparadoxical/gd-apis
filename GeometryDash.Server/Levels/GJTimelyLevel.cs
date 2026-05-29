using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Levels;

[Separator(Prop = ":", ListItem = "|"), Keyed]
public partial class GJTimelyLevel : GJLevel
{
    [Index(41)]
    public ushort? TimelyNumber { get; set; }
}
