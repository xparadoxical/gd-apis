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
            try
            {
                PropertySelector(key, value, ret);
            }
            catch (global::System.Exception ex)
            {
                throw new global::GeometryDash.Server.Serialization.SerializationException(key, ex);
            }
        }
        return ret;
    }

    internal static bool PropertySelector(uint key, global::System.ReadOnlySpan<byte> value, D ret)
    {
        switch (key)
        {
            case 5: ret.DeserializeArr(value); return true;
            //don't throw on unrecognized keys to maintain forward-compat //TODO option to disable (for server api monitoring)
        }
        return false;
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
