using System.Text;

using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Socials;
public sealed class MessageResponse : ISerializable<MessageResponse>
{
    public uint? MessageId { get; set; }
    public uint? OtherUserAccountId { get; set; }
    public uint? OtherUserPlayerId { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public string? OtherUserName { get; set; }
    public TimeSpan? Age { get; set; }
    public bool? IsRead { get; set; }
    public bool? IsIncoming { get; set; }

    public static SerializationOptions Options { get; } = new(true);
    public static SerializationLogic<MessageResponse> SerializationLogic { get; } = new SerializationLogicBuilder<MessageResponse>(9)
        .Deserializer(1, (input, inst) => inst.Value.MessageId = input.Parse<uint>())
        .Deserializer(2, (input, inst) => inst.Value.OtherUserAccountId = input.Parse<uint>())
        .Deserializer(3, (input, inst) => inst.Value.OtherUserPlayerId = input.Parse<uint>())
        .Deserializer(4, (input, inst) => inst.Value.Title = Base64.Decode(input))
        .Deserializer(5, (input, inst) =>
        {
            const int maxInputSize = 267; //max length of encoded data is 200
            Span<byte> buf = stackalloc byte[maxInputSize];
            input.CopyTo(buf);

            Xor.Apply(buf, Xor.Keys.Messages);
            inst.Value.Message = null!;//Base64.Decode(buf);
        })
        .Deserializer(6, (input, inst) => inst.Value.OtherUserName = Encoding.UTF8.GetString(input))
        .Deserializer(7, (input, inst) => inst.Value.Age = input.ParseTimeSpan())
        .Deserializer(8, (input, inst) => inst.Value.IsRead = input.ParseBool('1', '0'))
        .Deserializer(9, (input, inst) => inst.Value.IsIncoming = input.ParseBool('1', '0'))
        .Build();
}
