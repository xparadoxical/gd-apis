using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Levels;

[Separator(Prop = ":", ListItem = "|"), Keyed]
public partial class ListResponse : BaseLevelResponse
{
    [Index(7)]
    public LevelDifficulty? ListDifficulty { get; set; }

    [Index(28)]
    public uint? UploadDate { get; set; }

    [Index(29)]
    public uint? UpdateDate { get; set; }

    [Index(49)]
    public uint? AccountId { get; set; }

    [Index(50)]
    public string? Username { get; set; }

    [Index(51)]
    [Separator(",")]
    public uint[]? LevelIds { get; set; }

    ///<summary>null when list not featured</summary>
    [Index(55)]
    [CoalesceToNull(0)]
    public byte? ListReward { get; set; }

    ///<summary>null when list not featured</summary>
    [Index(56)]
    [CoalesceToNull(0)]
    public byte? ListRewardRequirement { get; set; }
}
