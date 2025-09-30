using System.ComponentModel;

namespace GeometryDash.Server.Serialization;

/// <summary>Provides serialization logic.</summary>
public interface ISerializable<TSelf> where TSelf : ISerializable<TSelf>
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static abstract TSelf Deserialize(ReadOnlySpan<byte> input);
}
