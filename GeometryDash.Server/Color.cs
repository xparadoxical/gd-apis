using GeometryDash.Server.Serialization;

namespace GeometryDash.Server;

[Separator(Prop = ",", ListItem = "|")]
public sealed partial class Color
{
    [Index(1)]
    public byte Red { get; set; }
    [Index(2)]
    public byte Green { get; set; }
    [Index(3)]
    public byte Blue { get; set; }
}
