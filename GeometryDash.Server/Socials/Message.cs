namespace GeometryDash.Server.Socials;
internal class Message : MessageInfo
{
    protected internal Message(MessageResponse response) : base(response)
        => Content = response.Message!;

    public required string Content { get; set; }
}
