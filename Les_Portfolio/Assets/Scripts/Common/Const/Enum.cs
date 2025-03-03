

// 메인화면 스테이트
public enum MainState
{
    Loading,
    Start
}

#region Setting
// 언어 타입
public enum LanguageType
{
    Korean = 0,
    English,
    Japanese,
    Max,
}
// 카메라 뷰 타입
public enum CameraViewType
{
    FPSView = 0,
    QuarterView,
    ShoulderView
}
// 플레이어 조작 타입
public enum PlayerMoveType
{
    Joystick = 0,
    Touch,
}
#endregion

#region System
public enum GAMEDATA_STATE
{
    CONNECTDATAERROR,
    PROTOCOLERROR,
    NODATA,
    REQUESTSUCCESS
}
#endregion

#region Player
public enum PlayerAniState
{
    Default,
    Jump,
}
#endregion

#region Game
public enum DongleType
{
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight
}

#endregion
