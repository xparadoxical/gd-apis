using CommunityToolkit.HighPerformance.Enumerables;

namespace GeometryDash.Server.Serialization;
public sealed partial class ServerSerializer
{
    public static T DeserializeSerializable<T>(ReadOnlySpan<byte> input, SerializationContext? context = null) where T : ISerializable<T>
        => T.Deserialize(input, context);

    public static T[] DeserializeArray<T>(ReadOnlySpan<byte> input, ReadOnlySpan<byte> itemSeparator, SerializationContext? context = null) where T : ISerializable<T>
    {
        var ret = new T[System.MemoryExtensions.Count(input, itemSeparator) + 1];

        int i = 0;
        foreach (var value in new ReadOnlySpanTokenizerWithSpanSeparator<byte>(input, itemSeparator))
        {
            ret[i++] = DeserializeSerializable<T>(value, context);
        }

        return ret;
    }
}
