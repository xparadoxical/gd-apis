using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Levels;

[Separator(Prop = ":", ListItem = "|"), Keyed]
public partial class LevelResponse : LevelInfoResponse
{
    [Index(4)]
    public string? Data { get; set; } //TODO deserialize to raw xml level string?

    [Index(27)]
    [Xor("26364"), Base64Encode]
    public string? Password { get; set; }

    [Index(36)]
    public string? CapacityString { get; set; }

    [Index(40)]
    [Bool(True = "1", False = "0")]
    public Optional<bool> LowDetailMode { get; set; }

    [Index(52)]
    [Separator(",")]
    public uint[]? SongIds { get; set; }

    [Index(53)]
    [Separator(",")]
    public uint[]? SoundEffectIds { get; set; }

    [Index(57)]
    public Optional<uint> VerificationFrames { get; set; }
}
