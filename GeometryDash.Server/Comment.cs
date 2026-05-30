using System.Diagnostics.CodeAnalysis;

using GeometryDash.Server.Http.temp;

namespace GeometryDash.Server;
public class Comment
{
    [SetsRequiredMembers]
    protected internal Comment(GJComment response)
    {
        Content = response.Comment;
        Likes = response.Likes;
        Id = response.Id;
        Age = response.Age;
    }

    public required string Content { get; set; }
    public required uint Likes { get; set; }
    public required uint Id { get; set; }
    public required TimeSpan Age { get; set; }
}
