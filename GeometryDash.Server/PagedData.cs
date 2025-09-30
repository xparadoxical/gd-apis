using GeometryDash.Server.Serialization;

namespace GeometryDash.Server;

public sealed record class PagedData<T>(T Data, PagingInfo Paging) : ISerializable<PagedData<T>>
    where T : ISerializable<T>
{
    public static PagedData<T> Deserialize(ReadOnlySpan<byte> input)
    {
        var separatorPos = input.LastIndexOf((byte)'#');

        var data = ServerSerializer.DeserializeSerializable<T>(input[..separatorPos]);
        var paging = ServerSerializer.DeserializeSerializable<PagingInfo>(input[(separatorPos + 1)..]);

        return new(data, paging);
    }
}
