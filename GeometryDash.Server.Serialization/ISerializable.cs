namespace GeometryDash.Server.Serialization;

//TODO replace with a source generator
/// <summary>
/// Provides reflection-less logic for serializing.
/// </summary>
public interface ISerializable<TSelf> where TSelf : ISerializable<TSelf>
{
    static abstract SerializationOptions Options { get; }

    static abstract SerializationLogic<TSelf> SerializationLogic { get; }
}
