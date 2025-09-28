namespace GeometryDash.Server.Socials;
public class MessageInfo
{
    protected internal MessageInfo(MessageResponse response)
    {
        OtherUser = new(response.OtherUserAccountId, response.OtherUserPlayerId, response.OtherUsername);
        MessageId = response.MessageId;
        Title = response.Title;
        Age = response.Age;
        IsRead = response.IsRead;
        IsIncoming = response.IsIncoming;
    }

    public required MessageUser OtherUser { get; set; }
    public required uint MessageId { get; set; }
    public required string Title { get; set; }
    public required TimeSpan Age { get; set; }
    public required bool IsRead { get; set; }
    public required bool IsIncoming { get; set; }
}

public record struct MessageUser(uint AccountId, uint PlayerId, string Username);
