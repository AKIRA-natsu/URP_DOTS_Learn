namespace AKIRA.UIFramework {
    /// <summary>
    /// UI类型
    /// </summary>
    public enum WinType {
        /// <summary>
        /// 空
        /// </summary>
        None = 200,
        /// <summary>
        /// 正常UI 隐藏游戏本体
        /// </summary>
        Normal,
        /// <summary>
        /// 过场 与游戏并存 并且可以操控（过场动画等）
        /// </summary>
        Interlude,
        /// <summary>
        /// 过场 与游戏并存 屏幕一边通知
        /// </summary>
        Notify,
    }
}
