namespace GeometryDash.Tests;
public class SerializationTest
{
    protected void TestDeserialization<T>(byte[][] inputs, T[] expectedOutputs) where T : ISerializable<T>
    {
        Assert.Multiple(inputs.Zip(expectedOutputs)
            .Select(inOut => new Action(() => Assert.Equivalent(inOut.Second, ServerSerializer.DeserializeSerializable<T>(inOut.First), true)))
            .ToArray());
    }
}
