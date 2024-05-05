using System.ComponentModel;

namespace GeometryDash.Server.Serialization;

//TODO replace with a source generator
/// <summary>
/// Provides reflection-less logic for serializing.
/// </summary>
public interface ISerializable<TSelf> where TSelf : ISerializable<TSelf>
{
    static abstract SerializationOptions Options { get; }

    static abstract SerializationLogic<TSelf> SerializationLogic { get; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static virtual TSelf Deserialize(ReadOnlySpan<byte> input) => throw new NotImplementedException(); //TODO remove body when SG will be usable
}
