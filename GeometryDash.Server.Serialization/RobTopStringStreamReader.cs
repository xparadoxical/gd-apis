using System.Text;

using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;

namespace GeometryDash.Server.Serialization;
public ref struct RobTopStringStreamReader(Stream input, IBuffer<byte> buffer, bool keyed = true)
{
    private bool _currentHasValue = false;
    private bool _expectingMore = false;

    public byte Separator { get; init; } = (byte)':'; //TODO ROS<byte> or remove the type

    /// <summary>Duck-typed IEnumerator implementation.</summary>
    public KeyValueSpanPair Current { get; private set; }

    /// <summary>Duck-typed IEnumerable implementation.</summary>
    public readonly RobTopStringStreamReader GetEnumerator() => this; //no way for >1 enumerator to be at different positions in one stream

    /// <summary>Duck-typed IEnumerator implementation.</summary>
    public bool MoveNext()
    {
        buffer.Clear();

        uint? key = null;

        bool end, onSeparator;
    readLoop: //PERF fill _buffer, use its .WrittenSpan, grow it with .GetSpan(newCapacity)
        while (true)
        {
            byte b = default;
            end = input.Read(new(ref b)) == 0;

            onSeparator = b == Separator;

            if (end || onSeparator)
                break;

            buffer.Write(b);
        }

        if (!end)
            _expectingMore = onSeparator;

        if (!_expectingMore && buffer.WrittenSpan.IsEmpty) //EOF and not expecting more data
            return false;

        if (keyed && key is null)
        {
            key = uint.Parse(Encoding.UTF8.GetString(buffer.WrittenSpan)); //TODO net8 use uint.Parse(ROS<byte>)
            buffer.Clear();
            goto readLoop;
        }
        else if (!keyed)
        {
            key = _currentHasValue ? Current.Key + 1 : 0;
        }

        Current = new KeyValueSpanPair(key!.Value, buffer.WrittenSpan);
        _currentHasValue = true;
        if (end)
            _expectingMore = false;
        return true;
    }
}
