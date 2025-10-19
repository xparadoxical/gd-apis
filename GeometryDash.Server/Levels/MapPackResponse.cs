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
    public string LevelIds { get; set; } //TODO uint[]

    [Index(4)]
    public byte? Stars { get; set; }

    [Index(5)]
    public byte? Coins { get; set; }

    [Index(6)]
    public MapPackDifficulty? Difficulty { get; set; }

    [Index(7)]
    public Color? TextColor { get; set; }

    [Index(8)]
    public Color? ProgressBarColor { get; set; }
}

public enum MapPackDifficulty : byte
{
    Auto = 0,
    Easy = 1,
    Normal = 2,
    Hard = 3,
    Harder = 4,
    Insane = 5,
    HardDemon = 6,
    EasyDemon = 7,
    MediumDemon = 8,
    InsaneDemon = 9,
    ExtremeDemon = 10
}
