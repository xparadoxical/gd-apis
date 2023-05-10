using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GeometryDash.Server.Serialization;
internal static class ValidationHelpers
{
    //BENCH move throw statements to a helper?
    internal static void ThrowIfSpanNullOrEmpty<T>(ReadOnlySpan<T> span, [CallerArgumentExpression(nameof(span))] string? paramName = null)
    {
        if (Unsafe.IsNullRef(ref MemoryMarshal.GetReference(span)))
            throw new ArgumentNullException(paramName);
        else if (span.IsEmpty)
            throw new ArgumentException("The value cannot be an empty string.", paramName);
    }

    internal static bool IsSpanNullOrEmpty<T>(ReadOnlySpan<T> span)
        => span.IsEmpty;
}
