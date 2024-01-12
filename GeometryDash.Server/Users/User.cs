using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Users;
public class User
{
    [SetsRequiredMembers]
    protected internal User(UserResponse response)
    {
        UserName = response.UserName;
        PlayerId = response.PlayerId!.Value;
        PlayerColor1 = response.PlayerColor1;
        PlayerColor2 = response.PlayerColor2;
        HasGlow = response.HasGlow!.Value;
        AccountId = response.AccountId;
    }

    public required string UserName { get; set; }
    public required uint PlayerId { get; set; }
    public required byte PlayerColor1 { get; set; }
    public required byte PlayerColor2 { get; set; }
    public required bool HasGlow { get; set; }
    public required uint AccountId { get; set; }
}
