//HintName: N.D.g.cs
#nullable enable
namespace N;

partial class D : global::GeometryDash.Server.Serialization.ISerializable<D>
{
    public static D Deserialize(global::System.ReadOnlySpan<byte> input)
    {
        var ret = new D();
        foreach (var (key, value) in new global::GeometryDash.Server.Serialization.RobTopStringReader(input) { Separator = ","u8 })
        {
            switch (key)
            {
                case 5: ret.DeserializeArr(value); break;
            }
        }
        return ret;
    }

    void DeserializeArr(global::System.ReadOnlySpan<byte> input)
    {
        var ret = new int[global::System.MemoryExtensions.Count(input, "|"u8) + 1];
        int i = 0;
        foreach (var value in new global::CommunityToolkit.HighPerformance.Enumerables.ReadOnlySpanTokenizerWithSpanSeparator<byte>(input, "|"u8))
            ret[i++] = global::GeometryDash.Server.Serialization.ParsingExtensions.Parse<int>(value);
        Arr = ret;

        OnArrDeserialized();
    }

    partial void OnArrDeserializing(global::PoolBuffers.PooledBuffer<byte> input);

    partial void OnArrDeserialized();
}
