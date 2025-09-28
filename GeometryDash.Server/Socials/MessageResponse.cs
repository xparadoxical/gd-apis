using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Socials;

[Separator(Prop = ':', ListItem = '|'), Keyed]
public sealed partial class MessageResponse : ISerializable<MessageResponse>
{
    [Index(1)]
    public uint MessageId { get; set; }

    [Index(2)]
    public uint OtherUserAccountId { get; set; }

    [Index(3)]
    public uint OtherUserPlayerId { get; set; }

    [Index(4)]
    [Base64Encode]
    public string Title { get; set; }

    [Index(5)]
    [Xor("14251"), Base64Encode]
    public string? Message { get; set; }

    [Index(6)]
    public string OtherUserName { get; set; }

    [Index(7)]
    public TimeSpan Age { get; set; }

    [Index(8)]
    [Bool('1', False = '0')]
    public bool IsRead { get; set; }

    [Index(9)]
    [Bool('1', False = '0')]
    public bool IsIncoming { get; set; }
}
