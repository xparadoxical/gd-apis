//HintName: N.C.g.cs
#nullable enable
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
        //Xor { KeyExpr = "12345" }
        //Base64 { }
        byte[]? t0_decodedArray = null;
        scoped global::System.Span<byte> t0_decoded;
        
        var t0_maxLength = global::GeometryDash.Server.Serialization.Base64.GetMaxDecodedLength(input);
        if (t0_maxLength > 512)
        {
            t0_decodedArray = global::System.Buffers.ArrayPool<byte>.Shared.Rent(t0_maxLength);
            t0_decoded = global::System.MemoryExtensions.AsSpan(t0_decodedArray);
        }
        else
            t0_decoded = stackalloc byte[512];
        
        var t0_status = global::GeometryDash.Server.Serialization.Base64.DecodeCore(input, t0_decoded, out _, out var t0_written);
        
        if (t0_status != global::System.Buffers.OperationStatus.Done)
        {
            if (t0_decodedArray is not null)
                global::System.Buffers.ArrayPool<byte>.Shared.Return(t0_decodedArray);
            throw new global::System.ArgumentException($"Operation status indicates failure: {t0_status}");
        }
        
        var t0 = t0_decoded[..t0_written];
        OnSDeserialized(t0);
        if (t0_decodedArray is not null)
    global::System.Buffers.ArrayPool<byte>.Shared.Return(t0_decodedArray);
        OnSDeserialized();
    }

    partial void OnSDeserializing(global::System.ReadOnlySpan<byte> input);

    partial void OnSDeserialized(global::System.ReadOnlySpan<byte> output);

    partial void OnSDeserialized(string value);
}
