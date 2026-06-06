namespace GeometryDash.Tests;
public class SerializationTest
{
    protected void TestDeserialization<T>(ReadOnlySpan<byte> input, T expectedOutput, SerializationContext? context = null) where T : ISerializable<T>
        => Assert.Equivalent(expectedOutput, ServerSerializer.DeserializeSerializable<T>(input, context), true);

    protected void TestDeserialization<T>(byte[][] inputs, T[] expectedOutputs, SerializationContext? context = null) where T : ISerializable<T>
    {
        Assert.Multiple(inputs.Zip(expectedOutputs)
            .Select(inOut => new Action(() => TestDeserialization(inOut.First, inOut.Second, context)))
            .ToArray());
    }

    protected void TestArrayDeserialization<T>(byte[] input, T[] expectedOutputs, SerializationContext? context = null) where T : ISerializable<T>
    {
        Assert.Multiple(ServerSerializer.DeserializeArray<T>(input, "|"u8, context)
            .Zip(expectedOutputs)
            .Select(inOut => new Action(() => Assert.Equivalent(inOut.Second, inOut.First, true)))
            .ToArray());
    }
}
