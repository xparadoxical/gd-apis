using System;

using GeometryDash.Server.Serialization;

using PoolBuffers;

namespace N;

[Separator(Prop = ":", ListItem = "|")]
internal partial class C
{
    [Index(1)]
    [Base64Encode, Xor("12345")]
    public string S { get; set; } = null!;

    public int Ignored { get; set; }

    [Index(2)]
    [Bool(True = "1")]
    public bool B1 { get; set; }

    [Index(4)] //intentional
    [Bool(True = "2", False = "1"), EmptyDefaultsTo(true)] //EDT before transformations
    public bool B2 { get; set; }

    [Index(3)]
    [Base64Encode, Gzip, CoalesceToNull("example")] //CTN after transformations
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

    [Index(9)]
    public D[] D1 { get; set; }

    [Index(10)]
    [Separator("#")]
    public D[] D2 { get; set; }

    [Index(11)]
    [Bool(True = "T")]
    public Optional<bool>? B3 { get; set; } //"T" -> true, "" -> no value, other value -> false, no occurence -> null

    [Index(12)]
    public Optional<int> O1 { get; set; }

    [Index(13)]
    public Optional<int>? O2 { get; set; }

    [Index(14)]
    [Separator(",")]
    public Optional<int[]> O3 { get; set; }

    [Index(15)]
    [Separator(",")]
    public Optional<int[]>? O4 { get; set; }
}

public class S : ISerializable<S>
{
    static S ISerializable<S>.Deserialize(ReadOnlySpan<byte> input) => new S();
}

[Keyed]
[Separator(Prop = ",", ListItem = ";")]
internal partial class D
{
    [Index(5)]
    [Separator("|")]
    public int[] Arr { get; set; }

    //IEnumerable, ICollection, IList, IReadOnlyX, IFrozenX
}
