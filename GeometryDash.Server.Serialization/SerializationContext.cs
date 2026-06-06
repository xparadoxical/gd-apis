namespace GeometryDash.Server.Serialization;
public sealed class SerializationContext
{
    private Dictionary<Type, byte[]> propertySeparators = new();
    private Dictionary<Type, byte[]> listSeparators = new();

    public SerializationContext WithListSeparator<T>(ReadOnlySpan<byte> sep) where T : ISerializable<T>
    {
        listSeparators[typeof(T)] = sep.ToArray();
        return this;
    }

    public SerializationContext WithPropertySeparator<T>(ReadOnlySpan<byte> sep) where T : ISerializable<T>
    {
        propertySeparators[typeof(T)] = sep.ToArray();
        return this;
    }

    public bool TryGetListSeparator<T>(out ReadOnlySpan<byte> sep) where T : ISerializable<T>
    {
        if (listSeparators.TryGetValue(typeof(T), out var value))
        {
            sep = value;
            return true;
        }

        sep = default;
        return false;
    }

    public bool TryGetPropertySeparator<T>(out ReadOnlySpan<byte> sep) where T : ISerializable<T>
    {
        if (propertySeparators.TryGetValue(typeof(T), out var value))
        {
            sep = value;
            return true;
        }

        sep = default;
        return false;
    }
}

public static class SerializationContextExtensions
{
    public static ReadOnlySpan<byte> GetListSeparatorOrDefault<T>(this SerializationContext? context, ReadOnlySpan<byte> defaultSep) where T : ISerializable<T>
        => context?.TryGetListSeparator<T>(out var sep) is true ? sep : defaultSep;

    public static ReadOnlySpan<byte> GetPropertySeparatorOrDefault<T>(this SerializationContext? context, ReadOnlySpan<byte> defaultSep) where T : ISerializable<T>
        => context?.TryGetPropertySeparator<T>(out var sep) is true ? sep : defaultSep;
}
