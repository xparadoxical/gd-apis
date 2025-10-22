namespace GeometryDash.Server.Levels;
public class MapPack
{
    protected internal MapPack(MapPackResponse response)
    {
        Id = response.Id;
        Name = response.Name!;
        LevelIds = response.LevelIds;
        Stars = response.Stars!.Value;
        Coins = response.Coins!.Value;
        Difficulty = response.Difficulty!.Value;
        TextColor = response.TextColor!;
        ProgressBarColor = response.ProgressBarColor!;
    }

    public ushort Id { get; set; }

    public string Name { get; set; }

    public string LevelIds { get; set; }

    public byte Stars { get; set; }

    public byte Coins { get; set; }

    public MapPackDifficulty Difficulty { get; set; }

    public Color TextColor { get; set; }

    public Color ProgressBarColor { get; set; }
}
