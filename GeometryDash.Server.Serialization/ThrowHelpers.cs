namespace GeometryDash.Server.Serialization;
internal static class ThrowHelpers
{
    internal static void ThrowFormatProviderNotSupported()
        => throw new NotSupportedException("IFormatProvider is not supported");
}
