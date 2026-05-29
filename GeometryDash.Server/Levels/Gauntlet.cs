namespace GeometryDash.Server.Levels;
public class Gauntlet
{
    protected internal Gauntlet(GJMapPack response)
    {
        Id = response.Id;
        LevelIds = response.LevelIds;
    }

    public ushort Id { get; set; }

    public uint[] LevelIds { get; set; }
}
