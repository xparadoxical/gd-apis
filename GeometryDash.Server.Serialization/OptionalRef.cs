namespace GeometryDash.Server.Serialization;
public readonly ref struct OptionalRef<T> where T : allows ref struct
{
    public OptionalRef(T value)
    {
        HasValue = true;
        Value = value;
    }

    public bool HasValue { get; }
    public T Value { get; }

    public static implicit operator T(OptionalRef<T> o) => o.Value;
    public static implicit operator OptionalRef<T>(T value) => new(value);
}
