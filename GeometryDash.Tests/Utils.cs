using System.Text;

namespace GeometryDash.Tests;
internal static class Utils
{
    internal static ReadOnlySpan<byte> Utf8(string s) => Encoding.UTF8.GetBytes(s).AsSpan();
}
