#region 게임 시스템

public enum FilePath
{
    PoupPath,
    CharacterPath,
    CharacterNamePath,
    StagePath,
}

public enum TimeType
{
    DAY,
    HOUR,
    MINUTE,
    SECOND,
}

public enum LanguageType
{
    NONE,

    EN,
    KR,
}

#endregion

#region 게임 플레이

// 각종 게임 모드
public enum GameModeType
{
    None,

    Normal_AI, // 기본 AI 대전 모드
    Normal_User, // 유저 대전 모드
}

// 게임 결과 상태
public enum GameResultType
{
    None,

    Win,
    Lose,
    Draw,
}

// 현재 인게임 플레이 상태
public enum GamePlayStateType
{
    None,

    GameStart,
    SelectFirstPlayer,
    ReadyToRoll,
    PlayMoveAction,
    GameEnd,
}

// 플레이어 애니메이션 상태
public enum PlayerAnimState
{
    Idle,
    Walk,
    Jump,
    Ladder_Up,
    Snake,
    Snake_Out,
    Landing,
    Six,
    Hammer_Rdy,
    Hammer_Atk,
    GoldenHammer_Rdy,
    GoldenHammer_Atk,
    Fall,
    Victory,
}

// 인게임 매니저 리프레쉬 타입
public enum InGameRefreshType
{
    All,

    GameStart,
    TurnChange,
    AdjustDice,
    AdjustPerk,
    RefreshItem,
    RefreshCamera,
    RefreshEquip,
}

// 인게임 인덱스 타입
public enum InGameIndexType
{
    All,

    Odd,
    Even,
}

// 인게임 사다리 블럭 타입
public enum InGameLadderType
{
    None,

    Wood_Ladder,
    Rope,
}

#endregion
