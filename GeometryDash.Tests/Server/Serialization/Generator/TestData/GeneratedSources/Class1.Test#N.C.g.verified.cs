//HintName: N.C.g.cs
namespace N;

partial class C : global::GeometryDash.Server.Serialization.ISerializable<C>
{
    public static C Deserialize(global::System.ReadOnlySpan<byte> input)
    {
        var ret = new C();
        uint key = 1;
        foreach (var value in global::CommunityToolkit.HighPerformance.ReadOnlySpanExtensions.Tokenize(input, '":"'))
        {
            switch (key)
            {
                case 2: DeserializeS(value); break;
            }
            key++;
        }
        return ret;
    }

    void DeserializeS(global::System.ReadOnlySpan<byte> input)
    {
        OnSDeserializing(input);
        //Xor { KeyExpr = "12345"u8 }
        //Base64 { }
        OnSDeserialized();
    }

    partial void OnSDeserializing(global::System.ReadOnlySpan<byte> input);

    partial void OnSDeserialized(string value);
}
