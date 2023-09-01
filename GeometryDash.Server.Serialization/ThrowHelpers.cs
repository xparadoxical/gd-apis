using System.Buffers;

namespace GeometryDash.Server.Serialization;
internal static class ThrowHelpers
{
    internal static void OperationStatusUnsuccessful(OperationStatus status)
        => throw new ArgumentException($"Operation status indicates failure: {status}");
}
