using System.Buffers;
using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;

using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace GeometryDash.Server.Serialization;
public sealed class ServerSerializer
{
    [ThreadStatic]
    private static Pool<IBuffer<byte>>? _buffers;
    private static Pool<IBuffer<byte>> Buffers
        => _buffers ??= new(() => new ArrayPoolBufferWriter<byte>(128));

    internal const bool InheritAttributes = false;

    public static unsafe T Deserialize<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T>
        (string input)
        where T : ISerializable<T>
        => input.ToUtf8(&Deserialize<T>);

    public static T Deserialize<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T>
        (ReadOnlySpan<byte> input)
        where T : ISerializable<T>
    {
        var type = typeof(T);
        //TODO add hashing

        var (keyed, fieldSeparator, _) = T.Options;
        var (deserializers, _) = T.SerializationLogic;

        var t = Activator.CreateInstance<T>()!;
        if (keyed)
        {
            foreach (var field in new RobTopStringReader(input) { FieldSeparator = fieldSeparator })
                deserializers[field.Key](field.Value, new(ref t));
        }
        else
        {
            uint key = 0;
            foreach (var value in input.Tokenize(fieldSeparator))
                deserializers[key++](value, new(ref t));
        }

        return t;
    }

    public static T Deserialize<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T>
        (Stream s, IBuffer<byte>? buffer = null)
        where T : ISerializable<T>
    {
        var type = typeof(T);
        //TODO add hashing - ServerResponseStream

        var (keyed, fieldSeparator, _) = T.Options;
        var (deserializers, _) = T.SerializationLogic;

        var buf = buffer ?? Buffers.Rent();

        var t = Activator.CreateInstance<T>()!;
        foreach (var field in new RobTopStringStreamReader(s, buf, keyed) { FieldSeparator = fieldSeparator })
            deserializers[field.Key](field.Value, new(ref t));

        if (buffer is null)
            Buffers.Return(buf);

        return t;
    }
}
