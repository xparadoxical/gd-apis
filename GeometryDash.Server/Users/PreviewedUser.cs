using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Users;
public class PreviewedUser : User
{
    [SetsRequiredMembers]
    protected internal PreviewedUser(UserResponse response) : base(response)
    {
        ShowcaseIconId = response.ShowcaseIconId!.Value;
        ShowcaseIconType = response.ShowcaseIconType!.Value;
    }

    public required uint ShowcaseIconId { get; set; }
    public required GameMode ShowcaseIconType { get; set; }
}
