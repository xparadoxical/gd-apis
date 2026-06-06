//HintName: N.D.g.cs
#nullable enable
namespace N;

partial class D : global::GeometryDash.Server.Serialization.ISerializable<D>
{
    public static D Deserialize(global::System.ReadOnlySpan<byte> input, global::GeometryDash.Server.Serialization.SerializationContext? context)
    {
        var ret = new D();
        foreach (var (key, value) in new global::GeometryDash.Server.Serialization.RobTopStringReader(input) { Separator = global::GeometryDash.Server.Serialization.SerializationContextExtensions.GetPropertySeparatorOrDefault<D>(context, ","u8) })
        {
            try
            {
                PropertySelector(key, value, ret, context);
            }
            catch (global::System.Exception ex)
            {
                throw new global::GeometryDash.Server.Serialization.SerializationException(key, ex);
            }
        }
        return ret;
    }

    public static D[] DeserializeArray(global::System.ReadOnlySpan<byte> input, global::GeometryDash.Server.Serialization.SerializationContext? context)
    {
        var sep = global::GeometryDash.Server.Serialization.SerializationContextExtensions.GetListSeparatorOrDefault<D>(context, ";"u8);
        var ret = new D[global::System.MemoryExtensions.Count(input, sep) + 1];
        var i = 0;
        foreach (var value in new global::CommunityToolkit.HighPerformance.Enumerables.ReadOnlySpanTokenizerWithSpanSeparator<byte>(input, sep))
            ret[i++] = Deserialize(value, context);
        return ret;
    }

    internal static void PropertySelector(uint key, global::System.ReadOnlySpan<byte> value, D ret, global::GeometryDash.Server.Serialization.SerializationContext? context)
    {
        switch (key)
        {
            case 5: ret.DeserializeArr(value, context); break;
        }
    }

    void DeserializeArr(global::System.ReadOnlySpan<byte> input, global::GeometryDash.Server.Serialization.SerializationContext? context)
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
