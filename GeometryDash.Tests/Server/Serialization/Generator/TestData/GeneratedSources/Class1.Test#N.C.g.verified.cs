//HintName: N.C.g.cs
#nullable enable
namespace N;

partial class C : global::GeometryDash.Server.Serialization.ISerializable<C>
{
    public static C Deserialize(global::System.ReadOnlySpan<byte> input)
    {
        var ret = new C();
        var key = 1;
        foreach (var value in new global::CommunityToolkit.HighPerformance.Enumerables.ReadOnlySpanTokenizerWithSpanSeparator<byte>(input, ":"u8))
        {
            switch (key)
            {
                case 1: ret.DeserializeS(value); break;
                case 2: ret.DeserializeB1(value); break;
                case 4: ret.DeserializeB2(value); break;
                case 3: ret.DeserializeZip(value); break;
                case 5: ret.DeserializeI(value); break;
                case 6: ret.DeserializeTime(value); break;
                case 7: ret.DeserializeE(value); break;
                case 8: ret.DeserializeNested(value); break;
            }
            key++;
        }
        return ret;
    }

    void DeserializeS(global::System.ReadOnlySpan<byte> input)
    {
        var buffer = new global::PoolBuffers.PooledBuffer<byte>(input);
        
        OnSDeserializing(buffer);

        global::GeometryDash.Server.Serialization.Xor.Apply(buffer.DataSpan, "12345"u8);

        buffer.EnsureCapacity(global::GeometryDash.Server.Serialization.Base64.GetMaxDecodedLength(buffer.Length));
        buffer.Length = global::GeometryDash.Server.Serialization.Base64.Decode(buffer.DataSpan);

        OnSDeserialized(buffer);

        S = global::System.Text.Encoding.UTF8.GetString(buffer.DataSpan);

        buffer.Dispose();
        OnSDeserialized();
    }

    partial void OnSDeserializing(global::PoolBuffers.PooledBuffer<byte> input);

    partial void OnSDeserialized(global::PoolBuffers.PooledBuffer<byte> output);

    partial void OnSDeserialized();

    void DeserializeB1(global::System.ReadOnlySpan<byte> input)
    {
        B1 = global::GeometryDash.Server.Serialization.ParsingExtensions.ParseBool(input, '1');

        OnB1Deserialized();
    }

    partial void OnB1Deserializing(global::PoolBuffers.PooledBuffer<byte> input);

    partial void OnB1Deserialized();

    void DeserializeB2(global::System.ReadOnlySpan<byte> input)
    {
        if (input.IsEmpty)
            B2 = true;
        else
            B2 = global::GeometryDash.Server.Serialization.ParsingExtensions.ParseBool(input, '2', '1');

        OnB2Deserialized();
    }

    partial void OnB2Deserializing(global::PoolBuffers.PooledBuffer<byte> input);

    partial void OnB2Deserialized();

    void DeserializeZip(global::System.ReadOnlySpan<byte> input)
    {
        var buffer = new global::PoolBuffers.PooledBuffer<byte>(input);
        
        OnZipDeserializing(buffer);

        var t1_length = global::GeometryDash.Server.Serialization.Gzip.GetDecompressedLength(buffer.DataSpan);
        var t1_output = global::System.Buffers.ArrayPool<byte>.Shared.Rent((int)t1_length);
        global::GeometryDash.Server.Serialization.Gzip.Decompress(buffer.DataSpan, t1_output);
        buffer.Length = t1_output.Length;
        global::System.MemoryExtensions.AsSpan(t1_output).CopyTo(buffer.DataSpan);
        global::System.Buffers.ArrayPool<byte>.Shared.Return(t1_output);

        buffer.EnsureCapacity(global::GeometryDash.Server.Serialization.Base64.GetMaxDecodedLength(buffer.Length));
        buffer.Length = global::GeometryDash.Server.Serialization.Base64.Decode(buffer.DataSpan);

        OnZipDeserialized(buffer);

        Zip = global::System.Text.Encoding.UTF8.GetString(buffer.DataSpan);

        if (Zip is "example")
            Zip = null;

        buffer.Dispose();
        OnZipDeserialized();
    }

    partial void OnZipDeserializing(global::PoolBuffers.PooledBuffer<byte> input);

    partial void OnZipDeserialized(global::PoolBuffers.PooledBuffer<byte> output);

    partial void OnZipDeserialized();

    void DeserializeI(global::System.ReadOnlySpan<byte> input)
    {
        I = global::GeometryDash.Server.Serialization.ParsingExtensions.Parse<uint>(input);

        OnIDeserialized();
    }

    partial void OnIDeserializing(global::PoolBuffers.PooledBuffer<byte> input);

    partial void OnIDeserialized();

    void DeserializeTime(global::System.ReadOnlySpan<byte> input)
    {
        var buffer = new global::PoolBuffers.PooledBuffer<byte>(input);
        
        OnTimeDeserializing(buffer);

        Time = global::GeometryDash.Server.Serialization.ParsingExtensions.ParseTimeSpan(buffer.DataSpan);

        buffer.Dispose();
        OnTimeDeserialized();
    }

    partial void OnTimeDeserializing(global::PoolBuffers.PooledBuffer<byte> input);

    partial void OnTimeDeserialized();

    void DeserializeE(global::System.ReadOnlySpan<byte> input)
    {
        E = global::GeometryDash.Server.Serialization.ParsingExtensions.ParseEnum<global::System.StringSplitOptions>(input);

        OnEDeserialized();
    }

    partial void OnEDeserializing(global::PoolBuffers.PooledBuffer<byte> input);

    partial void OnEDeserialized();

    void DeserializeNested(global::System.ReadOnlySpan<byte> input)
    {
        Nested = global::GeometryDash.Server.Serialization.ServerSerializer.DeserializeSerializable<global::N.S>(input);

        OnNestedDeserialized();
    }

    partial void OnNestedDeserializing(global::PoolBuffers.PooledBuffer<byte> input);

    partial void OnNestedDeserialized();
}
