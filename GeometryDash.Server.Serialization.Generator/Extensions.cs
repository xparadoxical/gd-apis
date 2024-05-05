namespace GeometryDash.Server.Serialization.Generator;
public static class Extensions
{
    public static T Cast<T>(this object o)
        where T : class
        => (T)o;
}
