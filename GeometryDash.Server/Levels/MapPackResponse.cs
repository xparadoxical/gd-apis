using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Levels;

[Separator(Prop = ':', ListItem = '|'), Keyed]
public sealed partial class MapPackResponse
{
    [Index(1)]
    public ushort Id { get; set; }

    [Index(2)]
    public string? Name { get; set; }

    [Index(3)]
    public string LevelIds { get; set; }

    [Index(4)]
    public byte? Stars { get; set; }

    [Index(5)]
    public byte? Coins { get; set; }

    [Index(6)]
    public byte? Difficulty { get; set; }

    [Index(7)]
    public Color? TextColor { get; set; }

    [Index(8)]
    public Color? ProgressBarColor { get; set; }
}
