using System.Collections;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace GeometryDash.Server.Serialization;
public sealed class Pool<T>(Func<T> defaultFactory) where T : class
{
    private readonly Func<T>? _factory = defaultFactory;

    /// <summary>Value true means free to rent.</summary>
    private readonly OrderedDictionary _instances = new();

    public T Rent() => Rent(_factory!);

    public T Rent(Func<T> factory)
    {
        T t;
        foreach (DictionaryEntry entry in _instances)
            if ((bool)entry.Value!)
            {
                t = Unsafe.As<T>(entry.Key)!;
                _instances[t] = false;
                return t;
            }

        if (factory is null)
            return ThrowNoFactory();

        t = factory();
        _instances.Add(t, false);
        return t;
    }

    public void Return(T t) => _instances[t] = true;

    private static T ThrowNoFactory()
        => throw new InvalidOperationException("No instance is available and a default factory function was not provided.");
}
