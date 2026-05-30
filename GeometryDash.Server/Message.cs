using GeometryDash.Server.Http.Socials;

namespace GeometryDash.Server;
internal class Message : MessageInfo
{
    protected internal Message(GJMessage response) : base(response)
        => Content = response.Message!;

    public required string Content { get; set; }
}
