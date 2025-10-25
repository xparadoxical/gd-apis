namespace GeometryDash.Server.Serialization.Generator;
public static class Extensions
{
    public static T Cast<T>(this object o) where T : class => (T)o;

    public static U Convert<T, U>(this T t, Func<T, U> converter) => converter(t);

    public static T? SingleOrNullable<T>(this IEnumerable<T> e, Func<T, bool> predicate) where T : struct
    {
        T? result = null;

        foreach (var v in e)
        {
            if (!predicate(v))
                continue;

            if (result is not null)
                throw new InvalidOperationException("Sequence contains more than one element");

            result = v;
        }

        return result;
    }

    public static void MultiFirst<T>(this IEnumerable<T> e, params (Func<T, bool> predicate, Action<T> success)[] queries)
    {
        Span<bool> hitFlags = stackalloc bool[queries.Length];
        foreach (var item in e)
        {
            for (int i = 0; i < queries.Length; i++)
            {
                if (hitFlags[i])
                    continue;
                var (predicate, success) = queries[i];
                if (predicate(item))
                {
                    hitFlags[i] = true;
                    success(item);
                }
            }
        }
    }
}
