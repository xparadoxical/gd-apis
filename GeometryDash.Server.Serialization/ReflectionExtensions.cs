using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Serialization;

public static unsafe class ReflectionExtensions
{
    public static bool TryMakeGenericType(this Type genericType, [NotNullWhen(true)] out Type? closedGenericType, params Type[] typeArguments) //TODO future: use params span
    {
        try
        {
            closedGenericType = genericType.MakeGenericType(typeArguments);
            return true;
        }
        catch
        {
            closedGenericType = default;
            return false;
        }
    }
}
