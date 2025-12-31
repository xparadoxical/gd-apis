namespace GeometryDash.Server.Levels;

public enum NonDemonDifficulty : sbyte
{
    Auto = -10,
    NA = 0,
    Easy = 10,
    Normal = 20,
    Hard = 30,
    Harder = 40,
    Insane = 50
}

public enum DemonDifficulty : byte
{
    Easy = 3,
    Medium = 4,
    Hard = 0,
    Insane = 5,
    Extreme = 6
}

public enum LevelLength : byte
{
    Tiny = 0,
    Short = 1,
    Medium = 2,
    Long = 3,
    XL = 4
}

public enum LevelDifficulty : sbyte
{
    NA = -1,
    Auto = 0,
    Easy = 1,
    Normal = 2,
    Hard = 3,
    Harder = 4,
    Insane = 5,
    EasyDemon = 6,
    MediumDemon = 7,
    HardDemon = 8,
    InsaneDemon = 9,
    ExtremeDemon = 10
}

public enum SpecialFeatureType : byte
{
    None = 0,
    Epic = 1,
    Legendary = 2,
    Mythic = 3
}
