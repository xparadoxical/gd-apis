using GeometryDash.Server.Users;

namespace GeometryDash.Server.Socials;
public class FriendRequest
{
    protected internal FriendRequest(FriendRequestResponse response)
    {
        OtherUser = new(response);

        Id = response.FriendRequestId!.Value;
        Message = response.FriendRequestMessage!;
        Age = response.FriendRequestAge!.Value;
        IsUnread = response.IsNewFriendOrRequest!.Value;
    }

    public PreviewedUser OtherUser { get; set; }

    public uint Id { get; set; }
    public string Message { get; set; }
    public TimeSpan Age { get; set; }
    public bool IsUnread { get; set; }
}
