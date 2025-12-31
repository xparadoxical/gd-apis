using GeometryDash.Server.Users;

namespace GeometryDash.Server.Users;

public enum GameMode : byte
{
    Cube,
    Ship,
    Ball,
    Ufo,
    Wave,
    Robot,
    Spider
}

public enum PrivacyGroup : byte
{
    Everyone,
    Friends,
    Private
}

public enum FriendState : byte
{
    None = 0,
    Friend = 1,
    //2 unknown
    RequestSent = 3,
    RequestReceived = 4
}

public enum ModeratorStatus : byte
{
    None,
    Moderator,
    ElderModerator,
    /// <summary>This value is visible only in <see cref="UserInfo"/>.</summary>
    LeaderboardModerator
}
