using System;

using GeometryDash.Server.Serialization;

using PoolBuffers;

namespace N;

[Separator(Prop = ':', ListItem = '|')]
internal partial class C
{
    [Index(1)]
    [Base64Encoded, Xor("12345")]
    public string S { get; set; } = null!;

    public int Ignored { get; set; }

    [Index(2)]
    [Bool('1')]
    public bool B1 { get; set; }

    [Index(4)] //intentional
    [Bool('2', False = '1'), EmptyDefaultsTo(true)] //EDT before transformations
    public bool B2 { get; set; }

    [Index(3)]
    [Base64Encoded, Gzip, CoalesceToNull("example")] //CTN after transformations
    public string? Zip { get; set; }

    [Index(5)]
    public uint I { get; set; }

    [Index(6)]
    public TimeSpan Time { get; set; }

    [Index(7)]
    public StringSplitOptions E { get; set; }

    [Index(8)]
    public S Nested { get; set; } = null!;

    partial void OnTimeDeserializing(PooledBuffer<byte> input) => throw new NotImplementedException();
}

public class S : ISerializable<S>
{
    static S ISerializable<S>.Deserialize(ReadOnlySpan<byte> input) => new S();
}

[Keyed]
[Separator(Prop = ',', ListItem = ';')]
internal partial class D
{
    [Index(5)]
    public C[] Arr { get; set; }

    //IEnumerable, ICollection, IList, IReadOnlyX, IFrozenX
}
