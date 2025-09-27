using System.Text;

using CommunityToolkit.HighPerformance;

namespace GeometryDash.Server.Serialization;
public ref struct RobTopStringReader(ReadOnlySpan<byte> input)
{
    private readonly ReadOnlySpan<byte> _span = input;

    private int _keyStart = 0;
    private int _keyEnd = 0;
    private int _valueStart = 0;
    private int _valueEnd = -1;

    public byte Separator { get; init; } = (byte)':'; //TODO song response uses "~|~"

    /// <summary>Duck-typed IEnumerator implementation.</summary>
    public readonly Prop Current => new(_span[_keyStart.._keyEnd].Parse<uint>(), _span[_valueStart.._valueEnd]);

    /// <summary>Duck-typed IEnumerable implementation.</summary>
    public readonly RobTopStringReader GetEnumerator() => this;

    /// <summary>Duck-typed IEnumerator implementation.</summary>
    public bool MoveNext()
    {
        if (_span.Length is 0 || _valueEnd >= _span.Length)
            return false;

        _keyStart = _valueEnd + 1;

        var i = _span[_keyStart..].IndexOf(Separator);
        if (i < 0)
            throw new FormatException($"Missing separator after key {Encoding.UTF8.GetString(_span[_keyStart..])}");

        _keyEnd = _keyStart + i;
        _valueStart = _keyEnd + 1; //_valueStart == _span.Length is fine and results in _vSt.._vEnd == n..n

        i = _span[_valueStart..].IndexOf(Separator);
        _valueEnd = i < 0 ? _span.Length : _valueStart + i;

        return true;
    }
}
