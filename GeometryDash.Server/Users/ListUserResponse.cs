using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Users;

[Separator(Prop = ":", ListItem = "|"), Keyed]
public partial class ListUserResponse : PreviewedUserResponse
{
    [Index(18)]
    public PrivacyGroup? AllowMessagesFrom { get; set; }
}
