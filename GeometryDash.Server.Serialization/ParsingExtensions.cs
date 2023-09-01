using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.Unicode;

using CommunityToolkit.HighPerformance;

namespace GeometryDash.Server.Serialization;
public static class ParsingExtensions
{
    public static T Parse<T>(this ReadOnlySpan<byte> input) where T : INumberBase<T>
    {
        if (TryParse(input, NumberStyles.None, null, out T result))
            return result;

        throw new FormatException("The span was not in a correct format.");
    }

    internal static Span<byte> Format<T>(this T value, Span<byte> output) where T : INumberBase<T>
    {
        if (TryFormat(value, output, out var written, default, null))
            return output[..written];

        throw new ArgumentException("Not enough space in the output buffer.");
    }

    public static void WriteUtf8<T>(this IBufferWriter<byte> writer, T value) where T : INumberBase<T>
    {
        Span<byte> buf = stackalloc byte[48];
        writer.Write(value.Format(buf));
    }

    #region .NET 8 IUtf8SpanParsable, IUtf8SpanFormattable polyfills
    //TODO net8 remove this
    //https://github.com/dotnet/runtime/blob/9ab160404032b5220900e94c044ff7323e7c31a9/src/libraries/System.Private.CoreLib/src/System/Numerics/INumberBase.cs#L418-L512

    private static bool TryParse<T>(ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out T result)
        where T : INumberBase<T>
    {
        // Convert text using stackalloc for <= 256 characters and ArrayPool otherwise

        char[]? utf16TextArray;
        scoped Span<char> utf16Text;
        int textMaxCharCount = Encoding.UTF8.GetMaxCharCount(utf8Text.Length);

        if (textMaxCharCount < 256)
        {
            utf16TextArray = null;
            utf16Text = stackalloc char[256];
        }
        else
        {
            utf16TextArray = ArrayPool<char>.Shared.Rent(textMaxCharCount);
            utf16Text = utf16TextArray.AsSpan(0, textMaxCharCount);
        }

        OperationStatus utf8TextStatus = Utf8.ToUtf16(utf8Text, utf16Text, out _, out int utf16TextLength, replaceInvalidSequences: false);

        if (utf8TextStatus != OperationStatus.Done)
        {
            result = default;
            return false;
        }
        utf16Text = utf16Text.Slice(0, utf16TextLength);

        // Actual operation

        bool succeeded = T.TryParse(utf16Text, style, provider, out result);

        // Return rented buffers if necessary

        if (utf16TextArray != null)
        {
            ArrayPool<char>.Shared.Return(utf16TextArray);
        }

        return succeeded;
    }

    private static bool TryFormat<T>(T @this, Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        where T : INumberBase<T>
    {
        char[]? utf16DestinationArray;
        scoped Span<char> utf16Destination;
        int destinationMaxCharCount = Encoding.UTF8.GetMaxCharCount(utf8Destination.Length);

        if (destinationMaxCharCount < 256)
        {
            utf16DestinationArray = null;
            utf16Destination = stackalloc char[256];
        }
        else
        {
            utf16DestinationArray = ArrayPool<char>.Shared.Rent(destinationMaxCharCount);
            utf16Destination = utf16DestinationArray.AsSpan(0, destinationMaxCharCount);
        }

        if (!@this.TryFormat(utf16Destination, out int charsWritten, format, provider))
        {
            if (utf16DestinationArray != null)
            {
                // Return rented buffers if necessary
                ArrayPool<char>.Shared.Return(utf16DestinationArray);
            }

            bytesWritten = 0;
            return false;
        }

        // Make sure we slice the buffer to just the characters written
        utf16Destination = utf16Destination.Slice(0, charsWritten);

        OperationStatus utf8DestinationStatus = Utf8.FromUtf16(utf16Destination, utf8Destination, out _, out bytesWritten, replaceInvalidSequences: false);

        if (utf16DestinationArray != null)
        {
            // Return rented buffers if necessary
            ArrayPool<char>.Shared.Return(utf16DestinationArray);
        }

        if (utf8DestinationStatus == OperationStatus.Done)
        {
            return true;
        }

        if (utf8DestinationStatus != OperationStatus.DestinationTooSmall)
        {
            ThrowHelpers.OperationStatusUnsuccessful(utf8DestinationStatus);
        }

        bytesWritten = 0;
        return false;
    }
    #endregion
}
