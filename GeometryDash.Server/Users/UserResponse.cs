using GeometryDash.Server.Serialization;

namespace GeometryDash.Server.Users;

[Separator(Prop = ":", ListItem = "|"), Keyed]
public partial class UserResponse : ISerializable<UserResponse>
{
    [Index(1)]
    public string Username { get; set; }

    [Index(2)]
    public uint PlayerId { get; set; }

    [Index(10)]
    public byte PlayerColor1 { get; set; }

    [Index(11)]
    public byte PlayerColor2 { get; set; }

    [Index(15)]
    [Bool(True = "2", False = "0")]
    public bool? HasGlow { get; set; }

    [Index(28)] //28 == 15
    [Bool(True = "1", False = "0")]
    private bool HasGlowAlternate { set => HasGlow = value; }

    [Index(16)]
    [CoalesceToNull(0)]
    public uint? AccountId { get; set; }
}
