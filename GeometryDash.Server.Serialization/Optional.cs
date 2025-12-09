namespace GeometryDash.Server.Serialization;
public readonly struct Optional<T> where T : notnull
{
    public Optional(T value)
    {
        HasValue = true;
        Value = value;
    }

    public bool HasValue { get; }
    public T Value { get; }

    public static implicit operator T(Optional<T> optional) => optional.Value;
    public static implicit operator Optional<T>(T value) => new(value);
}
