namespace GeometryDash.Server.Serialization;

/// <summary>
/// A provider of reflection-less logic for serializing itself.
/// </summary>
public interface ISerializable<TSelf> where TSelf : ISerializable<TSelf>
{
    static abstract SerializationOptions Options { get; }

    static abstract SerializationLogic<TSelf> SerializationLogic { get; }
}
