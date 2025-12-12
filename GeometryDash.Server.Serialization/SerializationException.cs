using System.Text;

namespace GeometryDash.Server.Serialization;
public sealed class SerializationException(uint key, Exception cause)
    : Exception($"An error occured when serializing key {key}.", cause);
