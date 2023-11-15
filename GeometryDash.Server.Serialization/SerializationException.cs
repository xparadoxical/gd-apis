using System.Text;

namespace GeometryDash.Server.Serialization;
public sealed class SerializationException : Exception
{
    public SerializationException(uint key, ReadOnlySpan<byte> value, Exception cause)
        : base($"An error occured when serializing key {key} with value '{Encoding.UTF8.GetString(value)}'.", cause)
    { }
}
