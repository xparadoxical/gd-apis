using System.Diagnostics.CodeAnalysis;

using GeometryDash.Server.Http.Users;

namespace GeometryDash.Server;
public class ListUser : PreviewedUser
{
    [SetsRequiredMembers]
    protected internal ListUser(GJListUser response) : base(response)
        => AllowMessagesFrom = response.AllowMessagesFrom!.Value;

    public required PrivacyGroup AllowMessagesFrom { get; set; }
}
