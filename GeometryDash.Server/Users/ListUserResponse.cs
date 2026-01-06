using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Users;

[Separator(Prop = ":", ListItem = "|"), Keyed]
public partial class ListUserResponse : PreviewedUserResponse
{
    [Index(18)]
    public PrivacyGroup? AllowMessagesFrom { get; set; }

    [Index(41)]
    [Bool(True = "1", False = "")]
    public bool? IsNewFriend { get; set; }
}
