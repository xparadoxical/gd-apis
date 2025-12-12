namespace GeometryDash.Server.Serialization;
public static class Xor
{
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
