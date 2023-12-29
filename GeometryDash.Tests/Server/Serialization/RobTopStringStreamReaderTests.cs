using System.Text;

using CommunityToolkit.HighPerformance.Buffers;

using Xunit.Abstractions;

namespace GeometryDash.Tests.Server.Serialization;
public class RobTopStringStreamReaderTests(ITestOutputHelper output)
{
    public List<(uint, string)> Read(string input, bool keyed)
    {
        output.WriteLine("'{0}'{1}", input, keyed ? " keyed" : "");

        var fields = new List<(uint, string)>();
        foreach (var (k, v) in new RobTopStringStreamReader(new MemoryStream(Encoding.UTF8.GetBytes(input)), new ArrayPoolBufferWriter<byte>(), keyed))
        {
            string s = Encoding.UTF8.GetString(v);
            output.WriteLine($"{k}: '{s}'");
            fields.Add((k, s));
        }
        return fields;
    }

    internal void TestOutputs(string input, bool keyed, object[] outputs)
    {
        var results = Read(input, keyed);

        var assertions = new List<Action>(results.Count + 1);

        assertions.Add(() => Assert.Equal(outputs.Length / 2, results.Count));
        for (int _i = 0; _i < results.Count; _i++)
        {
            var i = _i; //the place of variable declaration is where the closure is instantiated. _i is in the outer scope, so it needs to be copied for the lambda to get the current value of _i every iteration
            assertions.Add(() => Assert.Equal((Convert.ToUInt32(outputs[2 * i]), (string)outputs[2 * i + 1]), results[i]));
        }

        Assert.Multiple(assertions.ToArray());
    }

    //[Theory]
    [InlineData("3:a:2:b", new object[] { 3, "a", 2, "b" })]
    [InlineData("69::5:", new object[] { 69, "", 5, "" })]
    [InlineData("", new object[0])]
    public void Keyed_Works(string input, object[] outputs) => TestOutputs(input, true, outputs);

    //[Theory]
    [InlineData("a::c", new object[] { 0, "a", 1, "", 2, "c" })]
    [InlineData(":", new object[] { 0, "", 1, "" })]
    [InlineData("", new object[0])]
    public void Unkeyed_Works(string input, object[] outputs) => TestOutputs(input, false, outputs);

    //[Theory]
    [InlineData(":")]
    [InlineData("a:")]
    [InlineData("3:a:")]
    [InlineData("3:a:4")]
    [InlineData("1:::")]
    public void Keyed_Fails(string input) => Assert.ThrowsAny<Exception>(() => Read(input, true));
}
