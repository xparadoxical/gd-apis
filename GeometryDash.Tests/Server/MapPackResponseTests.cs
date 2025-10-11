using GeometryDash.Server.Levels;

namespace GeometryDash.Tests.Server;
public class MapPackResponseTests : SerializationTest
{
    [Fact]
    public void Deserialize_Gauntlet_Works()
    {
        TestDeserialization<MapPackResponse>("1:1:3:27732941,28200611,27483789,28225110,27448202"u8,
            new()
            {
                Id = 1,
                LevelIds = "27732941,28200611,27483789,28225110,27448202"
            });
    }

    [Fact]
    public void Deserialize_MapPack_Works()
    {
        TestDeserialization<MapPackResponse>("1:65:2:Demon Pack 16:3:1668421,1703546,923264:4:10:5:2:6:6:7:255,255,0:8:255,255,0"u8,
            new()
            {
                Id = 65,
                Name = "Demon Pack 16",
                LevelIds = "1668421,1703546,923264",
                Stars = 10,
                Coins = 2,
                Difficulty = 6,
                TextColor = new() { Red = 255, Green = 255, Blue = 0 },
                ProgressBarColor = new() { Red = 255, Green = 255, Blue = 0 }
            });
    }
}
