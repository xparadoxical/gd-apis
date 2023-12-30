using System.Text;

namespace GeometryDash.Server.Serialization;
public sealed class SerializationException(uint key, ReadOnlySpan<byte> value, Exception cause)
    : Exception($"An error occured when serializing key {key} with value '{Encoding.UTF8.GetString(value)}'.", cause);
