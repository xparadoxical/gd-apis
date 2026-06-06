namespace GeometryDash.Server.Serialization;
public sealed partial class ServerSerializer
{
    public static T DeserializeSerializable<T>(ReadOnlySpan<byte> input, SerializationContext? context = null) where T : ISerializable<T>
        => T.Deserialize(input, context);

    public static T[] DeserializeArray<T>(ReadOnlySpan<byte> input, SerializationContext? context = null) where T : ISerializable<T>
        => T.DeserializeArray(input, context);
}
