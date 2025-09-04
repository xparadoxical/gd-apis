using System.Buffers.Binary;
using System.IO.Compression;

namespace GeometryDash.Server.Serialization;

public static class Gzip
{
    public static uint GetDecompressedLength(ReadOnlySpan<byte> data)
    {
        if (data[0..2] is not [0x1f, 0x8b])
            throw new ArgumentException("Data is not gzip.");

        return BinaryPrimitives.ReadUInt32LittleEndian(data[^4..]);
    }

    public static unsafe uint Decompress(ReadOnlySpan<byte> input, Span<byte> output)
    {
        fixed (byte* pInput = input)
        {
            using var inputStream = new UnmanagedMemoryStream(pInput, input.Length);
            using var stream = new GZipStream(inputStream, CompressionMode.Decompress);
            var written = stream.Read(output);
            return (uint)written;
        }
    }
}
