using System.Buffers;
using System.Collections.Immutable;

using CommunityToolkit.HighPerformance;

namespace GeometryDash.Server.Serialization;

public sealed record class SerializationOptions(bool Keyed, byte FieldSeparator = (byte)':', byte ListSeparator = (byte)'|')
{
    public SerializationOptions(bool keyed, char fieldSeparator, char listSeparator = '|')
        : this(keyed, (byte)fieldSeparator, (byte)listSeparator) { }
}

public delegate void Deserializer<T>(ReadOnlySpan<byte> input, Ref<T> instance);
public delegate void Serializer<T>(IBufferWriter<byte> output, Ref<T> instance);

/// <summary>A collection of serializing functions for every serializable field.</summary>
/// <typeparam name="T">The type to serialize.</typeparam>
public sealed record class SerializationLogic<T>(
    ImmutableDictionary<uint, Deserializer<T>> Deserializers, //TODO net8 use FrozenDictionary
    ImmutableDictionary<uint, Serializer<T>> Serializers);

public sealed class SerializationLogicBuilder<T>(uint fieldCount) where T : ISerializable<T>
{
    private readonly Dictionary<uint, Deserializer<T>> _deserializers = new((int)fieldCount);
    private uint _deserializerIndex = 0;
    private readonly Dictionary<uint, Serializer<T>> _serializers = new((int)fieldCount);
    private uint _serializerIndex = 0;

    public SerializationLogicBuilder<T> Deserializer(uint index, Deserializer<T> de)
    {
        _deserializers[index] = de;
        _deserializerIndex = index + 1;
        return this;
    }

    public SerializationLogicBuilder<T> Deserializer(Deserializer<T> de) => Deserializer(_deserializerIndex, de);

    public SerializationLogicBuilder<T> Serializer(uint index, Serializer<T> ser)
    {
        _serializers[index] = ser;
        _serializerIndex = index + 1;
        return this;
    }

    public SerializationLogicBuilder<T> Serializer(Serializer<T> ser) => Serializer(_serializerIndex, ser);

    public SerializationLogic<T> Build()
    {
#if !DEBUG
        if (_deserializers.Count != fieldCount)
            ThrowDeserializersCount();
        if (_serializers.Count != fieldCount)
            ThrowSerializersCount();
#endif

        return new(_deserializers.ToImmutableDictionary(), _serializers.ToImmutableDictionary());
    }

    private static void ThrowDeserializersCount()
        => throw new InvalidOperationException($"Count of deserializers does not match the specified field count.");

    private static void ThrowSerializersCount()
        => throw new InvalidOperationException($"Count of serializers does not match the specified field count.");
}
