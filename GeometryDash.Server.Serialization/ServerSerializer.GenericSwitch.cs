using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;

using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;
using static System.Reflection.BindingFlags;

namespace GeometryDash.Server.Serialization;
public partial class ServerSerializer
{
    /// <summary>Dictionary&lt;type handle, delegate*&gt;</summary>
    private static readonly Dictionary<nint, nuint> _serializationFunctionCache = new(/*TODO runtime count of cached types*/);

    private static readonly MethodInfo _deserializeSerializable = typeof(ServerSerializer)
        .GetMethod(nameof(DeserializeSerializable), Public | Static, new[] { typeof(ReadOnlySpan<byte>) }) ?? throw null!;
    private static readonly MethodInfo _deserializeNumber = typeof(ServerSerializer)
        .GetMethod(nameof(DeserializeNumber), NonPublic | Static) ?? throw null!;
    private static readonly MethodInfo _deserializeSpanParsable = typeof(ServerSerializer)
        .GetMethod(nameof(DeserializeSpanParsable), NonPublic | Static) ?? throw null!;
    private static readonly MethodInfo _deserializeArrayWithDefaultSeparator = typeof(ServerSerializer)
        .GetMethod(nameof(DeserializeArrayWithDefaultSeparator), NonPublic | Static) ?? throw null!;

    public static unsafe T Deserialize<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T>(ReadOnlySpan<byte> input)
    {
        var th = typeof(T).TypeHandle.Value;

        if (_serializationFunctionCache.TryGetValue(th, out var fn))
        {
            return ((delegate*<ReadOnlySpan<byte>, T>)fn)(input);
        }

        var newFn = CreateFunctionPointer<T>();
        _serializationFunctionCache[th] = (nuint)newFn;

        return newFn(input);
    }

    private static unsafe delegate*<ReadOnlySpan<byte>, T> CreateFunctionPointer<T>()
    {
        var t = typeof(T);

        MethodInfo method;

        //if (T is ISerializable<T>)
        if (typeof(ISerializable<>).TryMakeGenericType(out _, t))
            method = _deserializeSerializable;
        //else if (T is INumber<T>)
        else if (typeof(INumber<>).TryMakeGenericType(out _, t)) //TODO net8 use IUtf8SpanParsable instead
            method = _deserializeNumber;
        else if (typeof(IUtf8SpanParsable<>).TryMakeGenericType(out _, t))
            method = _deserializeSpanParsable;
        //else if (T is (new El)[])
        else if (t.IsArray && t.GetElementType() is Type el)
        {
            t = el;
            method = _deserializeArrayWithDefaultSeparator;
        }
        else
            throw new ArgumentException($"'{t.FullName}' is not a supported type.");

        return (delegate*<ReadOnlySpan<byte>, T>)method.MakeGenericMethod(t).MethodHandle.GetFunctionPointer();
    }

    private static T DeserializeNumber<T>(ReadOnlySpan<byte> input) where T : INumber<T> //TODO net8 use IUtf8SpanParsable instead
        => input.Parse<T>();

    private static T DeserializeSpanParsable<T>(ReadOnlySpan<byte> input) where T : IUtf8SpanParsable<T>
        => T.Parse(input, null);

    private static T[] DeserializeArrayWithDefaultSeparator<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T>(ReadOnlySpan<byte> input)
        => DeserializeArray<T>(input);
}
