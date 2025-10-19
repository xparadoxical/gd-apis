using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Levels;

[Separator(Prop = ':', ListItem = '|')]
public sealed partial class LevelResponse
{
    [Index(1)]
    public uint LevelId { get; set; }

    [Index(2)]
    public string LevelName { get; set; }

    [Index(3)]
    [Base64Encode]
    public string Description { get; set; }

    [Index(4)]
    public string? Data { get; set; } //TODO deserialize to raw xml level string?

    [Index(5)]
    public byte Version { get; set; }

    [Index(6)]
    public uint? PlayerId { get; set; }

    [Index(7)]
    public LevelDifficulty? Difficulty { get; set; }

    [Index(8)]
    //TODO [Bool("10", False = "0")]
    public int? DifficultyDenominator { get; set; }

    [Index(9)]
    public NonDemonDifficulty? DifficultyVariantIndex { get; set; }

    [Index(10)]
    public uint Downloads { get; set; }

    //11 never sent

    [Index(12)]
    public OfficialSong? OfficialSong { get; set; }

    [Index(13)]
    public GameVersion? GameVersion { get; set; }

    [Index(14)]
    public uint Likes { get; set; }

    [Index(15)]
    public LevelLength? Length { get; set; }

    //16 never sent

    [Index(17)]
    [Bool('1')]
    public bool? IsDemon { get; set; }

    [Index(18)]
    public uint? Stars { get; set; }

    public bool IsFeatured => FeaturedScore != 0;

    [Index(19)]
    [EmptyDefaultsTo(0)] //featured lists
    public uint FeaturedScore { get; set; }

    //20-24 unknown

    [Index(25)]
    [Bool('1')]
    public uint? Auto { get; set; }

    //26 never sent

    [Index(27)]
    [Xor("26364"), Base64Encode]
    public string? Password { get; set; }

    /// <summary>Either<TimeSpan, uint> DateOrUnixTimestamp</summary>
    [Index(28)]
    public string? UploadDate { get; set; } //TODO

    /// <summary>Either<TimeSpan, uint> DateOrUnixTimestamp</summary>
    [Index(29)]
    public string? UpdateDate { get; set; } //TODO

    [Index(30)]
    [CoalesceToNull(0)]
    public uint? OriginalLevelId { get; set; }

    [Index(31)]
    [Bool('1', False = '0')]
    public bool? TwoPlayer { get; set; }

    //32-34 unknown

    [Index(35)]
    [CoalesceToNull(0)]
    public uint? CustomSongId { get; set; }

    [Index(36)]
    public string? CapacityString { get; set; }

    [Index(37)]
    public byte? Coins { get; set; }

    [Index(38)]
    [Bool('1', False = '0')]
    public bool? CoinsVerified { get; set; }

    [Index(39)]
    public byte? RequestedStars { get; set; }

    [Index(40)]
    [Bool('1', False = '0'), EmptyDefaultsTo(false)]
    public bool? LowDetailMode { get; set; }

    [Index(41)]
    public ushort? TimelyNumber { get; set; }

    [Index(42)]
    public SpecialFeatureType EpicRating { get; set; }

    [Index(43)]
    public DemonDifficulty? DemonDifficulty { get; set; }

    //TODO 44 never sent? check if any gauntlet level contains it
    //[Index(44)]
    //public bool? IsGauntlet { get; set; }

    [Index(45)]
    [CoalesceToNull(0)]
    public ushort? ObjectCount { get; set; }

    [Index(46)]
    [EmptyDefaultsTo(null)]
    public uint? EditorTime { get; set; }

    [Index(47)]
    [EmptyDefaultsTo(null)]
    public uint? EditorTimeOfPreviousCopies { get; set; }

    //48 never sent

    [Index(49)]
    public uint? AccountId { get; set; }

    [Index(50)]
    public string? Username { get; set; }

    [Index(51)]
    public string? LevelIds { get; set; } //TODO deserialize to uint[]

    [Index(52)]
    public string? SongIds { get; set; } //TODO deserialize to uint[]

    [Index(53)]
    public string? SoundEffectIds { get; set; } //TODO deserialize to uint[]

    //54 never sent

    ///<summary>null when list not featured</summary>
    [Index(55)]
    [CoalesceToNull(0)]
    public byte? ListReward { get; set; }

    ///<summary>null when list not featured</summary>
    [Index(56)]
    [CoalesceToNull(0)]
    public byte? ListRewardRequirement { get; set; }

    [Index(57)]
    [EmptyDefaultsTo(null)/*<=2.0*/, CoalesceToNull(0)/*2.1*/]
    public uint? VerificationFrames { get; set; }
}

public enum NonDemonDifficulty : byte
{
    Unrated = 0,
    Easy = 10,
    Normal = 20,
    Hard = 30,
    Harder = 40,
    Insane = 50
}

public enum DemonDifficulty : byte
{
    Easy = 3,
    Medium = 4,
    Hard = 0,
    Insane = 5,
    Extreme = 6
}

public enum LevelLength : byte
{
    Tiny = 0,
    Short = 1,
    Medium = 2,
    Long = 3,
    XL = 4
}

public enum LevelDifficulty : sbyte
{
    NA = -1,
    Auto = 0,
    Easy = 1,
    Normal = 2,
    Hard = 3,
    Harder = 4,
    Insane = 5,
    EasyDemon = 6,
    MediumDemon = 7,
    HardDemon = 8,
    InsaneDemon = 9,
    ExtremeDemon = 10
}

public enum SpecialFeatureType : byte
{
    None = 0,
    Epic = 1,
    Legendary = 2,
    Mythic = 3
}
