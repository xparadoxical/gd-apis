using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Users;
public class LeaderboardUser : UserWithStatistics
{
    [SetsRequiredMembers]
    protected internal LeaderboardUser(UserResponse response) : base(response)
    {
        LeaderboardPosition = response.LeaderboardPosition!.Value;
        Diamonds = response.Diamonds;
    }

    public required uint LeaderboardPosition { get; set; }
    public uint? Diamonds { get; set; }
}
