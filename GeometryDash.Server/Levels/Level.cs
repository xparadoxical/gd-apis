using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Levels;
public class Level : LevelInfo
{
    [SetsRequiredMembers]
    protected internal Level(LevelResponse response) : base(response)
    {
        Data = response.Data!;
        Password = response.Password;
        UploadDate = response.UploadDate!;
        UpdateDate = response.UpdateDate;
        CapacityString = response.CapacityString;
        LowDetailMode = response.LowDetailMode!.Value;
        SongIds = response.SongIds;
        SoundEffectIds = response.SoundEffectIds;
        VerificationFrames = response.VerificationFrames;
    }

    public required string Data { get; set; }
    public string? Password { get; set; }
    public required string UploadDate { get; set; }
    public string? UpdateDate { get; set; }
    public string? CapacityString { get; set; }
    public bool LowDetailMode { get; set; }
    public string? SongIds { get; set; }
    public string? SoundEffectIds { get; set; }
    public uint? VerificationFrames { get; set; } //TODO VersionGuard(2.2)
}
