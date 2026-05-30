using System.Diagnostics.CodeAnalysis;

using GeometryDash.Server.Http.Users;

namespace GeometryDash.Server;
public class LeaderboardUser : PreviewedUser
{
    [SetsRequiredMembers]
    protected internal LeaderboardUser(GJLeaderboardUser response) : base(response)
    {
        Stars = response.Stars!.Value;
        Demons = response.Demons!.Value;
        CreatorPoints = response.CreatorPoints!.Value;
        SecretCoins = response.SecretCoins!.Value;
        UserCoins = response.UserCoins!.Value;
        Moons = response.Moons!.Value;
        LeaderboardPosition = response.LeaderboardPosition!.Value;
        Diamonds = response.Diamonds;
    }

    public required uint Stars { get; set; }
    public required ushort Demons { get; set; }
    public required ushort CreatorPoints { get; set; }
    public required ushort SecretCoins { get; set; }
    public required uint UserCoins { get; set; }
    public required uint Moons { get; set; }
    public required uint LeaderboardPosition { get; set; }
    public uint? Diamonds { get; set; }
}
