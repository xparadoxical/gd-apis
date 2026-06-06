namespace GeometryDash.Server.Serialization.Generator.Tests;

public class Class1
{
    [Fact]
    public async Task Test()
    {
        var data = GeneratorTestHelper.CreateDriverForTest();
        var result = data.Driver.GetRunResult();

        await GeneratorTestHelper.Verify(result, data);
    }
}
