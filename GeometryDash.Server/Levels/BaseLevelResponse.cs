using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Levels;

[Separator(Prop = ":", ListItem = "|"), Keyed]
public partial class BaseLevelResponse
{
    [Index(1)]
    public uint LevelId { get; set; }

    [Index(2)]
    public string LevelName { get; set; }

    [Index(3)]
    [Base64Encode]
    public string Description { get; set; }

    [Index(5)]
    public byte Version { get; set; }

    [Index(10)]
    public uint Downloads { get; set; }

    [Index(14)]
    public uint Likes { get; set; }

    [Index(19)]
    public Optional<uint> FeaturedScore { get; set; }
}
