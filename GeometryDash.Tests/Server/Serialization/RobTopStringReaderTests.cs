using System.Text;

using Xunit.Abstractions;

namespace GeometryDash.Tests.Server.Serialization;
public class RobTopStringReaderTests
{
    private readonly ITestOutputHelper _output;

    public RobTopStringReaderTests(ITestOutputHelper output) => _output = output;

    public List<(uint, string)> Read(string input, bool keyed)
    {
        _output.WriteLine("'{0}'{1}", input, keyed ? " keyed" : "");

        var fields = new List<(uint, string)>();
        foreach (var (k, v) in new RobTopStringReader(Encoding.UTF8.GetBytes(input)))
        {
            string s = Encoding.UTF8.GetString(v);
            _output.WriteLine($"{k}: '{s}'");
            fields.Add((k, s));
        }
        return fields;
    }

    internal void TestOutputs(string input, bool keyed, object[] outputs)
    {
        var results = Read(input, keyed);

        Assert.Equal(outputs.Length / 2, results.Count);
        for (int i = 0; i < results.Count; i++)
        {
            Assert.Equal((Convert.ToUInt32(outputs[2 * i]), (string)outputs[2 * i + 1]), results[i]);
        }
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