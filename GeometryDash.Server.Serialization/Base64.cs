using System.Buffers;
using System.Text;

using Base64Lib = gfoidl.Base64.Base64;

namespace GeometryDash.Server.Serialization;
public static class Base64
{
    internal const int StackallocTreshold = 512;

    public static string Decode(ReadOnlySpan<byte> input)
    {
        input = input.TrimEnd((byte)'=');

        byte[]? decodedArray = null;
        scoped Span<byte> decoded;

        var decodedLength = Base64Lib.Url.GetMaxDecodedLength(input.Length);
        if (decodedLength > StackallocTreshold)
        {
            decodedArray = ArrayPool<byte>.Shared.Rent(decodedLength);
            decoded = decodedArray.AsSpan();
        }
        else
            decoded = stackalloc byte[StackallocTreshold];

        var status = Base64Lib.Url.Decode(input, decoded, out _, out var written);

        if (status != OperationStatus.Done)
        {
            if (decodedArray is not null)
                ArrayPool<byte>.Shared.Return(decodedArray);
            ThrowHelpers.OperationStatusUnsuccessful(status);
        }

        var str = Encoding.UTF8.GetString(decoded[..written]);

        if (decodedArray is not null)
            ArrayPool<byte>.Shared.Return(decodedArray);

        return str;
    }

    /// <summary>Decodes base64 input in-place.</summary>
    /// <returns>Number of bytes written to <paramref name="input"/>.</returns>
    public static int Decode(Span<byte> input)
    {
        input = input.TrimEnd((byte)'=');

        var status = Base64Lib.Url.Decode(input, input, out _, out var written);
        if (status != OperationStatus.Done)
            ThrowHelpers.OperationStatusUnsuccessful(status);

        return written;
    }

    public static int GetMaxDecodedLength(int encodedLength) => Base64Lib.Url.GetMaxDecodedLength(encodedLength);

    /// <returns>Number of bytes written to <paramref name="output"/>.</returns>
    public static int Decode(ReadOnlySpan<byte> input, Span<byte> output)
    {
        input = input.TrimEnd((byte)'=');

        var status = Base64Lib.Url.Decode(input, output, out _, out var written);
        if (status != OperationStatus.Done)
            ThrowHelpers.OperationStatusUnsuccessful(status);

        return written;
    }

    /// <summary>
    /// Decodes base64 input. The input can't contain any padding characters.
    /// </summary>
    public static OperationStatus DecodeCore(ReadOnlySpan<byte> input, Span<byte> output, out int consumed, out int written, bool isFinalBlock = true)
        => Base64Lib.Url.Decode(input, output, out consumed, out written, isFinalBlock);
}
