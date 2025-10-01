using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Comments;
public class Comment
{
    [SetsRequiredMembers]
    protected internal Comment(CommentResponse response)
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
