using CommunityToolkit.HighPerformance;

namespace GeometryDash.Server.Serialization;
public readonly ref struct KeyValueSpanPair
{
    public readonly uint Key { get; }
    public readonly ReadOnlySpan<byte> Value { get; }

    internal KeyValueSpanPair(uint key, ReadOnlySpan<byte> value)
    {
        Key = key;
        Value = value;
    }

    public void Deconstruct(out uint key, out ReadOnlySpan<byte> value)
    {
        key = Key;
        value = Value;
    }

    public CommunityToolkit.HighPerformance.Enumerables.ReadOnlySpanTokenizer<byte> EnumerateListItems(byte itemSeparator)
        => Value.Tokenize(itemSeparator);
}
