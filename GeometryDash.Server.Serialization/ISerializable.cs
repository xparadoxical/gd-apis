using System.ComponentModel;

namespace GeometryDash.Server.Serialization;

//TODO replace with a source generator
/// <summary>
/// Provides reflection-less logic for serializing.
/// </summary>
public interface ISerializable<TSelf> where TSelf : ISerializable<TSelf>
{
    public static virtual SerializationOptions Options { get; } //TODO remove

    public static virtual SerializationLogic<TSelf> SerializationLogic { get; } //TODO remove

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static virtual TSelf Deserialize(ReadOnlySpan<byte> input) => throw new NotImplementedException(); //TODO remove body when SG will be usable
}
