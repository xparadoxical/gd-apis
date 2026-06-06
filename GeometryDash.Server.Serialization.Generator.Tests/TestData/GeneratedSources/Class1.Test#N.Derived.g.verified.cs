//HintName: N.Derived.g.cs
#nullable enable
namespace N;

partial class Derived : global::GeometryDash.Server.Serialization.ISerializable<Derived>
{
    public new static Derived Deserialize(global::System.ReadOnlySpan<byte> input, global::GeometryDash.Server.Serialization.SerializationContext? context)
    {
        var ret = new Derived();
        var key = 1u;
        foreach (var value in new global::CommunityToolkit.HighPerformance.Enumerables.ReadOnlySpanTokenizerWithSpanSeparator<byte>(input, global::GeometryDash.Server.Serialization.SerializationContextExtensions.GetPropertySeparatorOrDefault<Derived>(context, "|"u8)))
        {
            try
            {
                PropertySelector(key, value, ret, context);
            }
            catch (global::System.Exception ex)
            {
                throw new global::GeometryDash.Server.Serialization.SerializationException(key, ex);
            }
            key++;
        }
        return ret;
    }

    public new static Derived[] DeserializeArray(global::System.ReadOnlySpan<byte> input, global::GeometryDash.Server.Serialization.SerializationContext? context)
    {
        var sep = global::GeometryDash.Server.Serialization.SerializationContextExtensions.GetListSeparatorOrDefault<Derived>(context, ","u8);
        var ret = new Derived[global::System.MemoryExtensions.Count(input, sep) + 1];
        var i = 0;
        foreach (var value in new global::CommunityToolkit.HighPerformance.Enumerables.ReadOnlySpanTokenizerWithSpanSeparator<byte>(input, sep))
            ret[i++] = Deserialize(value, context);
        return ret;
    }

    internal static void PropertySelector(uint key, global::System.ReadOnlySpan<byte> value, Derived ret, global::GeometryDash.Server.Serialization.SerializationContext? context)
    {
        switch (key)
        {
            case 100: ret.DeserializeNewProp(value, context); break;
            default: global::N.C.PropertySelector(key, value, ret, context); break;
        }
    }

    void DeserializeNewProp(global::System.ReadOnlySpan<byte> input, global::GeometryDash.Server.Serialization.SerializationContext? context)
    {
        NewProp = global::GeometryDash.Server.Serialization.ParsingExtensions.Parse<int>(input);

        OnNewPropDeserialized();
    }

    partial void OnNewPropDeserializing(global::PoolBuffers.PooledBuffer<byte> input);

    partial void OnNewPropDeserialized();
}
