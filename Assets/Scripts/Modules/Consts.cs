// 资产
public class Assets {
    // Asset
    public const string Asset_InputAction = "Assets/Res/MainBundle/InputActions/InputAction.inputactions";
}

// 状态机
public class StateKey {
    public const string Move = "Move";
}

// 动画
public class Animation {
    public const string Idle = "Idle";
    public const string Move = "Move";
    public const string MoveToStop = "MoveToStop";
    public const string JumpStart = "JumpStart";
    public const string JumpLoop = "JumpLoop";
    public const string JumpEnd = "JumpEnd";
}

// 事件
public class Events {
    public const string OnGameQuit = "OnGameQuit";                          // 游戏退出
    public const string OnGamePaused = "OnGamePaused";                      // 游戏暂停/恢复    // 参数 bool:是否暂停
    public const string OnGameSaving = "OnGameSaving";                      // 游戏存档
    public const string OnGameSaving_Setting = "OnGameSaving_Setting";      // 游戏存档 设置
}

// 映射Actions名称
// asset 本身会修改同名，不用考虑同名的情况
public class InputActions {
    // Map
    public const string Player = "Player";
    public const string UI = "UI";

    // Player
    public const string Move = "Move";
    public const string Look = "Look";
    public const string Fire = "Fire";
    public const string Run = "Run";
    public const string Jump = "Jump";
    public const string Reload = "Reload";
    public const string Mutual = "Mutual";

    // UI
    public const string Navigate = "Navigate";
    public const string Submit = "Submit";
    public const string Cancel = "Cancel";
    public const string Point = "Point";
    public const string Click = "Click";
    public const string ScrollWheel = "ScrollWheel";
    public const string MiddleClick = "MiddleClick";
    public const string RightClick = "RightClick";
    public const string TrackedDevicePosition = "TrackedDevicePosition";
    public const string TrackedDeviceOrientation = "TrackedDeviceOrientation";

    // Player && UI
    public const string Pause = "Pause";

}
