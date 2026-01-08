using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Levels;
public class Level : LevelInfo
{
    [SetsRequiredMembers]
    protected internal Level(LevelResponse response) : base(response)
    {
        Data = response.Data!;
        Password = response.Password;
        UploadedAgo = response.UploadedAgo!.Value;
        UpdatedAgo = response.UpdatedAgo;
        CapacityString = response.CapacityString;
        LowDetailMode = response.LowDetailMode.HasValue ? response.LowDetailMode : false;
        SongIds = response.SongIds;
        SoundEffectIds = response.SoundEffectIds;
        VerificationFrames = !response.VerificationFrames.HasValue ? null
            : response.VerificationFrames.Value is 0 ? null : response.VerificationFrames.Value;
    }

    public required string Data { get; set; }
    public string? Password { get; set; }
    public required TimeSpan UploadedAgo { get; set; }
    public TimeSpan? UpdatedAgo { get; set; }
    public string? CapacityString { get; set; }
    public bool LowDetailMode { get; set; }
    public uint[]? SongIds { get; set; }
    public uint[]? SoundEffectIds { get; set; }
    public uint? VerificationFrames { get; set; } //TODO VersionGuard(2.2)
}
