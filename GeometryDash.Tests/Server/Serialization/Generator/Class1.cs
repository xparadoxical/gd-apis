using Ver = GeometryDash.Tests.Server.Serialization.Generator.SerializerGeneratorVerifier;

namespace GeometryDash.Tests.Server.Serialization.Generator;
public class Class1
{
    [Fact]
    public async void M()
    {
        await Ver.Verify(["Test.cs"], ["N.C.g.cs"]);
    }
}
