//HintName: N.Derived.g.cs
#nullable enable
namespace N;

partial class Derived : global::GeometryDash.Server.Serialization.ISerializable<Derived>
{
    public new static Derived Deserialize(global::System.ReadOnlySpan<byte> input)
    {
        var ret = new Derived();
        var key = 1u;
        foreach (var value in new global::CommunityToolkit.HighPerformance.Enumerables.ReadOnlySpanTokenizerWithSpanSeparator<byte>(input, "|"u8))
        {
            try
            {
                PropertySelector(key, value, ret);
            }
            catch (global::System.Exception ex)
            {
                throw new global::GeometryDash.Server.Serialization.SerializationException(key, ex);
            }
            key++;
        }
        return ret;
    }

    internal static void PropertySelector(uint key, global::System.ReadOnlySpan<byte> value, Derived ret)
    {
        switch (key)
        {
            case 100: ret.DeserializeNewProp(value); break;
            default: global::N.C.PropertySelector(key, value, ret); break;
        }
    }

    void DeserializeNewProp(global::System.ReadOnlySpan<byte> input)
    {
        NewProp = global::GeometryDash.Server.Serialization.ParsingExtensions.Parse<int>(input);

        OnNewPropDeserialized();
    }

    partial void OnNewPropDeserializing(global::PoolBuffers.PooledBuffer<byte> input);

    partial void OnNewPropDeserialized();
}
