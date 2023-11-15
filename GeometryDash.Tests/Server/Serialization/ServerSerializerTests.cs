using System.Buffers;
using System.Text;

namespace GeometryDash.Tests.Server.Serialization;
public class ServerSerializerTests
{
    [Fact]
    public void Deserialize_ParsesCorrectly()
    {
        Assert.True(ServerSerializer.Deserialize<Unkeyed>("3:asd") is { I: 3, S: "asd" });
        Assert.True(ServerSerializer.Deserialize<Keyed>("4:3:5:asd") is { I: 3, S: "asd" });
        Assert.True(ServerSerializer.Deserialize<Nested<Keyed>>("5~4:3:5:asd") is { I: 5, Inner: { I: 3, S: "asd" } });
    }

    [Fact]
    public void Deserialize_WithInvalidInput_PassesToCoreDeserializerWhichThrows()
    {
        var e = Assert.Throws<SerializationException>(() => ServerSerializer.Deserialize<NotSerializable>("1:2"));
        Assert.IsType<DirectoryNotFoundException>(e.InnerException);
    }

    [Fact]
    public void Deserialize_List_ParsesCorrectly()
    {
        Assert.True(ServerSerializer.Deserialize<int[]>("3|4") is [3, 4]);
        Assert.True(ServerSerializer.Deserialize<Unkeyed[]>("3:asd|4:qwe") is [{ I: 3, S: "asd" }, { I: 4, S: "qwe" }]);
    }

    [Fact]
    public void Deserialize_WithUnsupportedType_Throws()
    {
        Assert.Throws<ArgumentException>(() => ServerSerializer.Deserialize<object>(""));
        Assert.Throws<ArgumentException>(() => ServerSerializer.Deserialize<object[]>(""));
    }

    private class NotSerializable : ISerializable<NotSerializable>
    {
        public static SerializationOptions Options { get; } = new(false);
        public static SerializationLogic<NotSerializable> SerializationLogic { get; }
            = new SerializationLogicBuilder<NotSerializable>(1)
            .Deserializer((_, _) => throw new DirectoryNotFoundException())
            .Serializer((_, _) => throw new DirectoryNotFoundException())
            .Build();
    }
}

internal class Unkeyed : ISerializable<Unkeyed>
{
    public required int I { get; set; }
    public required string S { get; set; }

    public static SerializationOptions Options { get; } = new(false);
    public static SerializationLogic<Unkeyed> SerializationLogic { get; } = new SerializationLogicBuilder<Unkeyed>(2)
        .Deserializer((input, inst) => inst.Value.I = input.Parse<int>())
        .Deserializer((input, inst) => inst.Value.S = Encoding.UTF8.GetString(input))
        .Serializer((output, inst) => output.WriteUtf8(inst.Value.I))
        .Serializer((output, inst) => output.Write(Encoding.UTF8.GetBytes(inst.Value.S)))
        .Build();
}

internal class Keyed : ISerializable<Keyed>
{
    public required int I { get; set; }
    public required string S { get; set; }

    public static SerializationOptions Options { get; } = new(true);
    public static SerializationLogic<Keyed> SerializationLogic { get; } = new SerializationLogicBuilder<Keyed>(2)
        .Deserializer(4, (input, inst) => inst.Value.I = input.Parse<int>())
        .Deserializer((input, inst) => inst.Value.S = Encoding.UTF8.GetString(input))
        .Serializer(4, (output, inst) => output.WriteUtf8(inst.Value.I))
        .Serializer((output, inst) => output.Write(Encoding.UTF8.GetBytes(inst.Value.S)))
        .Build();
}

internal class Nested<TInner> : ISerializable<Nested<TInner>> where TInner : ISerializable<TInner>
{
    public required int I { get; set; }
    public required TInner Inner { get; set; }

    public static SerializationOptions Options { get; } = new(false, '~');
    public static SerializationLogic<Nested<TInner>> SerializationLogic { get; } = new SerializationLogicBuilder<Nested<TInner>>(2)
        .Deserializer((input, inst) => inst.Value.I = input.Parse<int>())
        .Deserializer((input, inst) => inst.Value.Inner = ServerSerializer.Deserialize<TInner>(input))
        .Serializer((output, inst) => output.WriteUtf8(inst.Value.I))
        .Serializer((output, inst) => throw null)
        .Build();
}
