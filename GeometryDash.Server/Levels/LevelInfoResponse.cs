using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Levels;

[Separator(Prop = ":", ListItem = "|"), Keyed]
public partial class LevelInfoResponse : BaseLevelResponse
{
    [Index(6)]
    public uint? PlayerId { get; set; }

    [Index(8)]
    [Bool(True = "10", False = "0")]
    public bool? DifficultyDenominator { get; set; }

    [Index(9)]
    public NonDemonDifficulty? DifficultyVariantIndex { get; set; }

    [Index(12)]
    public OfficialSong? OfficialSong { get; set; }

    [Index(13)]
    public GameVersion? GameVersion { get; set; }

    [Index(15)]
    public LevelLength? Length { get; set; }

    [Index(17)]
    [Bool(True = "1", False = "")]
    public bool? IsDemon { get; set; }

    [Index(18)]
    public uint? Stars { get; set; }

    [Index(25)]
    [Bool(True = "1", False = "")]
    public bool? Auto { get; set; }

    [Index(30)]
    [CoalesceToNull(0)]
    public uint? OriginalLevelId { get; set; }

    [Index(31)]
    [Bool(True = "1", False = "0")]
    public bool? TwoPlayer { get; set; }

    [Index(35)]
    [CoalesceToNull(0)]
    public uint? CustomSongId { get; set; }

    [Index(37)]
    public byte? Coins { get; set; }

    [Index(38)]
    [Bool(True = "1", False = "0")]
    public bool? CoinsVerified { get; set; }

    [Index(39)]
    public byte? RequestedStars { get; set; }

    [Index(42)]
    public SpecialFeatureType EpicRating { get; set; }

    [Index(43)]
    public DemonDifficulty? DemonDifficulty { get; set; }

    [Index(45)]
    [CoalesceToNull(0)]
    public ushort? ObjectCount { get; set; }

    [Index(46)]
    public Optional<uint> EditorTime { get; set; }

    [Index(47)]
    public Optional<uint> EditorTimeOfPreviousCopies { get; set; }
}
