using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Levels;
public class LevelInfo
{
    [SetsRequiredMembers]
    protected internal LevelInfo(LevelResponse response)
    {
        (OfficialSong, CustomSongId) = ((OfficialSong?, uint?))(response.CustomSongId switch
        { //force NRE in case of unexpected nulls
            0 => (response.OfficialSong!.Value, null),
            _ => (null, response.CustomSongId!.Value)
        });

        Difficulty = response.IsDemon!.Value switch
        {
            false => (LevelDifficulty)((sbyte)response.DifficultyVariantIndex!.Value / 10),
            true => response.DemonDifficulty switch
            {
                DemonDifficulty.Easy => LevelDifficulty.EasyDemon,
                DemonDifficulty.Medium => LevelDifficulty.MediumDemon,
                DemonDifficulty.Hard => LevelDifficulty.HardDemon,
                DemonDifficulty.Insane => LevelDifficulty.InsaneDemon,
                DemonDifficulty.Extreme => LevelDifficulty.ExtremeDemon,
                _ => throw new UnreachableException($"demon diff {response.DemonDifficulty}")
            }
        };

        Id = response.LevelId;
        Name = response.LevelName;
        Description = response.Description;
        Version = response.Version;
        PlayerId = response.PlayerId!.Value;
        Downloads = response.Downloads;
        GameVersion = response.GameVersion!.Value;
        Likes = response.Likes;
        Length = response.Length!.Value;
        Stars = response.Stars!.Value;
        OriginalLevelId = response.OriginalLevelId;
        TwoPlayer = response.TwoPlayer!.Value;
        Coins = response.Coins!.Value;
        CoinsVerified = response.CoinsVerified!.Value!;
        RequestedStars = response.RequestedStars!.Value;
        EpicRating = response.EpicRating;
        ObjectCount = response.ObjectCount;
        EditorTime = response.EditorTime;
        EditorTimeOfPreviousCopies = response.EditorTimeOfPreviousCopies;
    }

    public required uint Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    /// <summary>Level updates increment this.</summary>
    public byte Version { get; set; } = 1;
    public required uint PlayerId { get; set; }
    public required uint Downloads { get; set; }
    /// <summary><see langword="null"/> when a custom song is used.</summary>
    public required OfficialSong? OfficialSong { get; set; }
    public required GameVersion GameVersion { get; set; } //TODO is this really nullable?
    public uint Likes { get; set; } = 0;
    public required LevelLength Length { get; set; } //TODO nullable?
    public required uint Stars { get; set; }
    public required LevelDifficulty Difficulty { get; set; }
    public bool IsDemon => Difficulty >= LevelDifficulty.EasyDemon;
    public bool IsRated => this is { Stars: > 0, Difficulty: not LevelDifficulty.NA };
    public uint FeaturedScore { get; set; } = 0;
    public bool IsFeatured => FeaturedScore != 0;
    public uint? OriginalLevelId { get; set; }
    public bool IsOriginal => OriginalLevelId is null;
    public bool TwoPlayer { get; set; } = false;
    /// <summary><see langword="null"/> when an <see cref="OfficialSong"/> is used.</summary>
    public uint? CustomSongId { get; set; }
    public required byte Coins { get; set; }
    public required bool CoinsVerified { get; set; }
    public byte RequestedStars { get; set; } = 0;
    public SpecialFeatureType EpicRating { get; set; } = SpecialFeatureType.None;
    //TODO which gameversion started storing the below properties?
    /// <summary><see langword="null"/> for older levels.</summary>
    public ushort? ObjectCount { get; set; }
    /// <summary><see langword="null"/> for older levels.</summary>
    public uint? EditorTime { get; set; }
    /// <summary><see langword="null"/> for older levels.</summary>
    public uint? EditorTimeOfPreviousCopies { get; set; }
}
