using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Users;
public class UserWithStatistics : PreviewedUser
{
    [SetsRequiredMembers]
    protected internal UserWithStatistics(UserResponse response) : base(response)
    {
        Stars = response.Stars!.Value;
        Demons = response.Demons!.Value;
        CreatorPoints = response.CreatorPoints!.Value;
        SecretCoins = response.SecretCoins!.Value;
        UserCoins = response.UserCoins!.Value;
    }

    public required uint Stars { get; set; }
    public required ushort Demons { get; set; }
    public required ushort CreatorPoints { get; set; }
    public required ushort SecretCoins { get; set; }
    public required uint UserCoins { get; set; }
    public required uint Moons { get; set; }
}
