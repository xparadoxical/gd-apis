namespace GeometryDash.Server.Socials;
public class MessageInfo
{
    protected internal MessageInfo(MessageResponse response)
    {
        OtherUser = new(response.OtherUserAccountId!.Value, response.OtherUserPlayerId!.Value, response.OtherUserName!);
        MessageId = response.MessageId!.Value;
        Title = response.Title!;
        Age = response.Age!.Value;
        IsRead = response.IsRead!.Value;
        IsIncoming = response.IsIncoming!.Value;
    }

    public required MessageUser OtherUser { get; set; }
    public required uint MessageId { get; set; }
    public required string Title { get; set; }
    public required TimeSpan Age { get; set; }
    public required bool IsRead { get; set; }
    public required bool IsIncoming { get; set; }
}

public record struct MessageUser(uint AccountId, uint PlayerId, string UserName);
