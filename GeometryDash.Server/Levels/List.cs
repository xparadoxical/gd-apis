using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Levels;
internal class List
{
    [SetsRequiredMembers]
    protected internal List(LevelResponse response)
    {
        Featured = response.FeaturedScore switch
        {
            0 => false,
            1 => true,
            _ => throw new UnreachableException($"FeaturedScore {response.FeaturedScore}")
        };

        Id = response.LevelId;
        Name = response.LevelName;
        Description = response.Description;
        Version = response.Version;
        Difficulty = response.ListDifficulty!.Value;
        Downloads = response.Downloads;
        Likes = response.Likes;
        UploadDate = response.UploadDate!;
        UpdateDate = response.UpdateDate;
        AccountId = response.AccountId!.Value;
        Username = response.Username!;
        LevelIds = response.LevelIds!;
        DiamondReward = response.ListReward;
        CompletionCountRequirement = response.ListRewardRequirement;
    }

    public required uint Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public byte Version { get; set; } = 1;
    public required LevelDifficulty Difficulty { get; set; }
    public uint Downloads { get; set; }
    public uint Likes { get; set; }
    [MemberNotNull(nameof(DiamondReward), nameof(CompletionCountRequirement))]
    public bool Featured { get; set; }
    public required string UploadDate { get; set; }
    public string? UpdateDate { get; set; }
    public required uint AccountId { get; set; }
    public required string Username { get; set; }
    public required string LevelIds { get; set; }
    public byte? DiamondReward { get; set; }
    public byte? CompletionCountRequirement { get; set; }
}
