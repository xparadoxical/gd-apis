using GeometryDash.Server.Http.Users;

namespace GeometryDash.Server;
public class FriendRequest
{
    protected internal FriendRequest(GJFriendRequest response)
    {
        OtherUser = new(response);

        Id = response.FriendRequestId!.Value;
        Message = response.FriendRequestMessage!;
        Age = response.FriendRequestAge!.Value;
        IsUnread = response.IsNewRequest!.Value;
    }

    public PreviewedUser OtherUser { get; set; }

    public uint Id { get; set; }
    public string Message { get; set; }
    public TimeSpan Age { get; set; }
    public bool IsUnread { get; set; }
}
