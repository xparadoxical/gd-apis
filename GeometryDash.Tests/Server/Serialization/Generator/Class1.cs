namespace GeometryDash.Tests.Server.Serialization.Generator;

public class Class1
{
    [Fact]
    public async Task Test()
    {
        var driver = GeneratorTestHelper.CreateDriverForTest();

        var result = driver.GetRunResult();

        Assert.Empty(result.Diagnostics);
        await Verify(result);
    }
}
