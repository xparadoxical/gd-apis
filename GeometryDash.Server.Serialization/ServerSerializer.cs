using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;

using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace GeometryDash.Server.Serialization;
public sealed partial class ServerSerializer
{
    [ThreadStatic]
    private static Pool<IBuffer<byte>>? _buffers;
    private static Pool<IBuffer<byte>> Buffers
        => _buffers ??= new(() => new ArrayPoolBufferWriter<byte>(128));

    internal const bool InheritAttributes = false;

    public static unsafe T Deserialize<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T>
        (string input)
        => input.ToUtf8(&Deserialize<T>);

    public static unsafe T DeserializeSerializable<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T>
        (string input)
        where T : ISerializable<T>
        => input.ToUtf8(&DeserializeSerializable<T>);

    public static T DeserializeSerializable<T>(ReadOnlySpan<byte> input) where T : ISerializable<T>
        => T.Deserialize(input);

    public static T[] DeserializeArray<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T>(string input, byte itemSeparator = (byte)'|')
        => input.ToUtf8(span => DeserializeArray<T>(span, itemSeparator));

    public static T[] DeserializeArray<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T>(ReadOnlySpan<byte> input, byte itemSeparator = (byte)'|')
    {
        var ret = new T[System.MemoryExtensions.Count(input, itemSeparator) + 1];

        int i = 0;
        foreach (var value in input.Tokenize(itemSeparator))
        {
            ret[i++] = Deserialize<T>(value);
        }

        return ret;
    }

    public static T Deserialize<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T>
        (Stream s, IBuffer<byte>? buffer = null)
        where T : ISerializable<T>
    {
        var type = typeof(T);
        //TODO add hashing - ServerResponseStream

        var (keyed, fieldSeparator, _) = T.Options;
        var deserializers = T.SerializationLogic.Deserializers;

        var buf = buffer ?? Buffers.Rent();

        var t = Activator.CreateInstance<T>()!;
        foreach (var field in new RobTopStringStreamReader(s, buf, keyed) { Separator = fieldSeparator })
            deserializers[field.Key](field.Value, new(ref t));

        if (buffer is null)
            Buffers.Return(buf);

        return t;
    }
}
