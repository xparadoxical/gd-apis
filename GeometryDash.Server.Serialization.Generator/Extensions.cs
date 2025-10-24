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
}
