namespace GeometryDash.Tests.Server;

public class PagingInfoTests : SerializationTest
{
    [Fact]
    public void Deserialize_Works()
    {
        TestDeserialization("1:1:1"u8.ToArray(), new PagingInfo(1, 1, 1));
    }

    //TODO required property handling
    //[Theory]
    //[InlineData("")]
    //[InlineData(":")]
    //[InlineData("::")]
    //[InlineData("0:0:+1")]
    //public void Parse_Fails(string input)
    //{
    //    Assert.Multiple(
    //        () => Assert.ThrowsAny<Exception>(() => PagingInfo.Parse(Utf8(input))),
    //        () => Assert.False(PagingInfo.TryParse(Utf8(input), null, out var _))
    //    );
    //}
}
