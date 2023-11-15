namespace GeometryDash.Server.Serialization;
public static class EnumTypeInfo<T> where T : struct, Enum
{
    public static readonly TypeCode TypeCode = Type.GetTypeCode(typeof(T));
}
