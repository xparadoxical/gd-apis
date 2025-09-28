namespace GeometryDash.Tests;
public class SerializationTest
{
    protected void TestDeserialization<T>(byte[][] inputs, T[] expectedOutputs) where T : ISerializable<T>
    {
        Assert.Multiple(inputs.Zip(expectedOutputs)
            .Select(inOut => new Action(() => Assert.Equivalent(inOut.Item2, ServerSerializer.DeserializeSerializable<T>(inOut.Item1))))
            .ToArray());
    }
}
