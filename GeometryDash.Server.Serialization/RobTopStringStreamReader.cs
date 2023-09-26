using System.Text;

using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;

namespace GeometryDash.Server.Serialization;
public ref struct RobTopStringStreamReader
{
    private readonly Stream _stream;
    private readonly IBuffer<byte> _buffer;
    private readonly bool _keyed;

    private bool _currentHasValue = false;
    private bool _expectingMore = false;

    public RobTopStringStreamReader(Stream input, IBuffer<byte> buffer, bool keyed = true)
    {
        _stream = input;
        _buffer = buffer;
        _keyed = keyed;
    }

    public byte FieldSeparator { get; init; } = (byte)':';

    /// <summary>Duck-typed IEnumerator implementation.</summary>
    public Field Current { get; private set; }

    /// <summary>Duck-typed IEnumerable implementation.</summary>
    public readonly RobTopStringStreamReader GetEnumerator() => this; //no way for >1 enumerator to be at different positions in one stream

    /// <summary>Duck-typed IEnumerator implementation.</summary>
    public bool MoveNext()
    {
        _buffer.Clear();

        uint? key = null;

        bool end, onSeparator;
    readLoop: //PERF fill _buffer, use its .WrittenSpan, grow it with .GetSpan(newCapacity)
        while (true)
        {
            byte b = default;
            end = _stream.Read(new(ref b)) == 0;

            onSeparator = b == FieldSeparator;

            if (end || onSeparator)
                break;

            _buffer.Write(b);
        }

        if (!end)
            _expectingMore = onSeparator;

        if (!_expectingMore && _buffer.WrittenSpan.IsEmpty) //EOF and not expecting more data
            return false;

        if (_keyed && key is null)
        {
            key = uint.Parse(Encoding.UTF8.GetString(_buffer.WrittenSpan)); //TODO net8 use uint.Parse(ROS<byte>)
            _buffer.Clear();
            goto readLoop;
        }
        else if (!_keyed)
        {
            key = _currentHasValue ? Current.Key + 1 : 0;
        }

        Current = new Field(key!.Value, _buffer.WrittenSpan);
        _currentHasValue = true;
        if (end)
            _expectingMore = false;
        return true;
    }
}