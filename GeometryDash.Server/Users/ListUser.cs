using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Users;
public class ListUser : PreviewedUser
{
    [SetsRequiredMembers]
    protected internal ListUser(GJListUser response) : base(response)
        => AllowMessagesFrom = response.AllowMessagesFrom!.Value;

    public required PrivacyGroup AllowMessagesFrom { get; set; }
}
