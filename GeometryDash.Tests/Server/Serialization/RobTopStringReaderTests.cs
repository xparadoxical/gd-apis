using System.Text;

using Xunit.Abstractions;

namespace GeometryDash.Tests.Server.Serialization;
public class RobTopStringReaderTests(ITestOutputHelper output)
{
    public List<(uint, string)> Read(string input, bool keyed)
    {
        output.WriteLine("'{0}'{1}", input, keyed ? " keyed" : "");

        var props = new List<(uint, string)>();
        foreach (var (k, v) in new RobTopStringReader(Encoding.UTF8.GetBytes(input)) { Separator = ":"u8 })
        {
            string s = Encoding.UTF8.GetString(v);
            output.WriteLine($"{k}: '{s}'");
            props.Add((k, s));
        }
        return props;
    }

    internal void TestOutputs(string input, bool keyed, object[] outputs)
    {
        var results = Read(input, keyed);

        var assertions = new List<Action>(results.Count + 1);

        assertions.Add(() => Assert.Equal(outputs.Length / 2, results.Count));
        for (int i = 0; i < results.Count; i++)
        {
            var j = i; //the place of variable declaration is where the closure is instantiated. _i is in the outer scope, so it needs to be copied for the lambda to get the current value of _i every iteration
            assertions.Add(() => Assert.Equal((Convert.ToUInt32(outputs[2 * j]), (string)outputs[2 * j + 1]), results[j]));
        }

        Assert.Multiple(assertions.ToArray());
    }

    [Theory]
    [InlineData("3:a:2:b", new object[] { 3, "a", 2, "b" })]
    [InlineData("69::5:", new object[] { 69, "", 5, "" })]
    [InlineData("", new object[0])]
    public void Works(string input, object[] outputs) => TestOutputs(input, true, outputs);

    [Theory]
    [InlineData(":")]
    [InlineData("a:")]
    [InlineData("3:a:")]
    [InlineData("3:a:4")]
    [InlineData("1:::")]
    public void Fails(string input) => Assert.ThrowsAny<Exception>(() => Read(input, true));
}
