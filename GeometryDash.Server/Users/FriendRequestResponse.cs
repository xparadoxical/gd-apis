using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Users;

[Separator(Prop = ":", ListItem = "|"), Keyed]
public partial class FriendRequestResponse : PreviewedUserResponse
{
    [Index(31)]
    public Optional<FriendState>? FriendState { get; set; }

    [Index(32)]
    public uint? FriendRequestId { get; set; }

    [Index(35)]
    [Base64Encode]
    public string? FriendRequestMessage { get; set; }

    [Index(37)]
    public TimeSpan? FriendRequestAge { get; set; }

    [Index(38)]
    public byte? IncomingMessageCount { get; set; }

    [Index(39)]
    public byte? IncomingFriendRequestCount { get; set; }

    [Index(40)]
    public byte? NewFriendsCount { get; set; }

    [Index(41)]
    [Bool(True = "1", False = "")]
    public bool? IsNewFriendOrRequest { get; set; }
}
