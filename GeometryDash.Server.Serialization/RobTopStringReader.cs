using System.Text;

using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;

namespace GeometryDash.Server.Serialization;
public ref struct RobTopStringReader
{
    private readonly Stream _stream;
    private readonly IBuffer<byte> _buffer;
    private readonly bool _keyed;

    private bool _currentHasValue = false;
    private bool _expectingMore = false;

    public RobTopStringReader(Stream input, IBuffer<byte> buffer, bool keyed = true)
    {
        _stream = Stream.Synchronized(input); //TODO is there any point in making the stream synchronized?
        _buffer = buffer;
        _keyed = keyed;
    }

    public byte FieldSeparator { get; init; } = (byte)':';
    public Field Current { get; private set; }

    public readonly RobTopStringReader GetEnumerator() => this; //no way for >1 enumerator to be at different positions in one stream

    public bool MoveNext()
    {
        _buffer.Clear();

        uint? key = null;

        bool end = default, onSeparator = default;
    readLoop: //PERF fill _buffer, use its .WrittenSpan, grow it with .GetSpan(newCapacity)
        while (true)
        {
            int _b = _stream.ReadByte();
            end = _b == -1;

            byte b = (byte)_b;
            onSeparator = _b == FieldSeparator;

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

    public readonly ref struct Field
    {
        public readonly uint Key { get; }
        public readonly ReadOnlySpan<byte> Value { get; }

        internal Field(uint key, ReadOnlySpan<byte> value)
        {
            Key = key;
            Value = value;
        }

        public void Deconstruct(out uint key, out ReadOnlySpan<byte> value)
        {
            key = Key;
            value = Value;
        }

        public CommunityToolkit.HighPerformance.Enumerables.ReadOnlySpanTokenizer<byte> EnumerateListItems(byte itemSeparator)
            => Value.Tokenize(itemSeparator);
    }
}
