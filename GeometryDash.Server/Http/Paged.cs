using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Http;

public sealed record class Paged<T>(T[] Data, PagingInfo Paging) : ISerializable<Paged<T>>
    where T : ISerializable<T>
{
    public static Paged<T> Deserialize(ReadOnlySpan<byte> input, SerializationContext? context)
    {
        var separatorPos = input.LastIndexOf("#"u8);

        var data = ServerSerializer.DeserializeArray<T>(input[..separatorPos], context);
        var paging = ServerSerializer.DeserializeSerializable<PagingInfo>(input[(separatorPos + 1)..], context);

        return new(data, paging);
    }

    static Paged<T>[] ISerializable<Paged<T>>.DeserializeArray(ReadOnlySpan<byte> input, SerializationContext? context)
        => throw new InvalidOperationException();
}
