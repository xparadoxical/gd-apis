//HintName: N.D.g.cs
#nullable enable
namespace N;

partial class D : global::GeometryDash.Server.Serialization.ISerializable<D>
{
    public static D Deserialize(global::System.ReadOnlySpan<byte> input)
    {
        var ret = new D();
        foreach (var (key, value) in new global::GeometryDash.Server.Serialization.RobTopStringReader(input) { Separator = (byte)',' })
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
        Arr = global::GeometryDash.Server.Serialization.ServerSerializer.DeserializeArray<global::N.C>(input);

        OnArrDeserialized();
    }

    partial void OnArrDeserializing(global::PoolBuffers.PooledBuffer<byte> input);

    partial void OnArrDeserialized();
}
