using GeometryDash.Server.Serialization;

namespace N;

[Separator(Prop = ":", ListItem = "|")]
internal partial class C
{
    public int I { get; set; }
    [Index(2)]
    [Base64Encoded, Xor("12345"u8)]
    public string S { get; set; }
}
