using System.Buffers;
using System.Text;

namespace GeometryDash.Server.Serialization;
public static class Xor
{
    /// <summary>
    /// Performs binary XOR on <paramref name="input"/> with the cyclic <paramref name="key"/>.
    /// For each input <see cref="byte"/>, the next byte of the key is used, starting the key over whenever its end is reached.
    /// </summary>
    /// <returns>The result converted to a UTF-16 <see cref="string"/>.</returns>
    public static string Apply(ReadOnlySpan<byte> input, ReadOnlySpan<byte> key)
    {
        //TODO (string.Create or stackalloc/arraypool) + Utf8.ToUtf16 chain?
        var buf = ArrayPool<byte>.Shared.Rent(input.Length);

        for (int i = 0; i < input.Length; i++)
            buf[i] = (byte)(input[i] ^ key[i % key.Length]);

        var ret = Encoding.UTF8.GetString(buf.AsSpan(..input.Length));
        ArrayPool<byte>.Shared.Return(buf);
        return ret;
    }

    /// <summary>
    /// Performs binary XOR on each <see cref="byte"/> of <paramref name="input"/> with <paramref name="key"/>.
    /// </summary>
    /// <returns>The result converted to a UTF-16 <see cref="string"/>.</returns>
    public static string Apply(ReadOnlySpan<byte> input, byte key)
        => Apply(input, new ReadOnlySpan<byte>(ref key));

    /// <summary>
    /// Performs binary XOR on <paramref name="bytesToApplyTo"/> with the cyclic <paramref name="key"/>.
    /// For each input <see cref="byte"/>, the next byte of the key is used, starting the key over whenever its end is reached.
    /// </summary>
    public static void Apply(Span<byte> bytesToApplyTo, ReadOnlySpan<byte> key)
    {
        for (int i = 0; i < bytesToApplyTo.Length; i++)
            bytesToApplyTo[i] = (byte)(bytesToApplyTo[i] ^ key[i % key.Length]);
    }

    /// <summary>
    /// Performs binary XOR on each <see cref="byte"/> of <paramref name="bytesToApplyTo"/> with <paramref name="key"/>.
    /// </summary>
    public static void Apply(Span<byte> bytesToApplyTo, byte key)
        => Apply(bytesToApplyTo, new ReadOnlySpan<byte>(ref key));

    /// <summary>XOR keys used by the game.</summary>
    public static class Keys
    {
        public static ReadOnlySpan<byte> Messages => "14251"u8;
    }
}
