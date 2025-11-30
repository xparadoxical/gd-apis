using CommunityToolkit.HighPerformance.Enumerables;

namespace GeometryDash.Server.Serialization;
public sealed partial class ServerSerializer
{
    public static T DeserializeSerializable<T>(ReadOnlySpan<byte> input) where T : ISerializable<T>
        => T.Deserialize(input);

    public static T[] DeserializeArray<T>(ReadOnlySpan<byte> input, ReadOnlySpan<byte> itemSeparator) where T : ISerializable<T>
    {
        var ret = new T[System.MemoryExtensions.Count(input, itemSeparator) + 1];

        int i = 0;
        foreach (var value in new ReadOnlySpanTokenizerWithSpanSeparator<byte>(input, itemSeparator))
        {
            ret[i++] = DeserializeSerializable<T>(value);
        }

        return ret;
    }
}
