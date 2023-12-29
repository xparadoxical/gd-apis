using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using GeometryDash.Server.Serialization;

namespace GeometryDash.Server;
public class PagedData<T> : IUtf8SpanParsable<PagedData<T>>
    where T : ISerializable<T>
{
    public required T Data { get; init; }
    public required PagingInfo Paging { get; init; }

    public static PagedData<T> Parse(ReadOnlySpan<byte> input, IFormatProvider? provider = null)
    {
        if (!TryParse(input, provider, out var ret))
            throw new FormatException("The input span was not in a correct format.");

        return ret;
    }

    public static bool TryParse(ReadOnlySpan<byte> input, [Optional, DefaultParameterValue(null)] IFormatProvider? provider, [MaybeNullWhen(false)] out PagedData<T> ret)
    {
        var hash = input.LastIndexOf((byte)'#');
        if (!PagingInfo.TryParse(input[(hash + 1)..], result: out var info))
        {
            ret = default;
            return false;
        }

        ret = new()
        {
            Data = ServerSerializer.Deserialize<T>(input[..hash]),
            Paging = info
        };
        return true;
    }
}
