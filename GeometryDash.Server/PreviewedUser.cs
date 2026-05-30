using System.Diagnostics.CodeAnalysis;

using GeometryDash.Server.Http.Users;

namespace GeometryDash.Server;
public class PreviewedUser : User
{
    [SetsRequiredMembers]
    protected internal PreviewedUser(GJPreviewedUser response) : base(response)
    {
        ShowcaseIconId = response.ShowcaseIconId!.Value;
        ShowcaseIconType = response.ShowcaseIconType!.Value;
    }

    public required uint ShowcaseIconId { get; set; }
    public required GameMode ShowcaseIconType { get; set; }
}
