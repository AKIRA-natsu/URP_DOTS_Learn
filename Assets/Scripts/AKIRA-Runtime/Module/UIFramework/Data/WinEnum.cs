namespace AKIRA.UIFramework {
    /// <summary>
    /// UI窗口，自动添加/移除，但不排序
    /// </summary>
    public enum WinEnum {
        /// <summary>
        /// 空
        /// </summary>
        None = 100,
        /// <summary>
        /// 主菜单
        /// </summary>
        Menu,
        /// <summary>
        /// 主界面
        /// </summary>
        Main,
        /// <summary>
        /// 暂停界面
        /// </summary>
        Pause,
        /// <summary>
        /// 暂停界面，背景Mask
        /// </summary>
        PauseBack,
        /// <summary>
        /// 设置界面
        /// </summary>
        Setting,
        /// <summary>
        /// 指引界面
        /// </summary>
        Guide,
        /// <summary>
        /// 收集界面
        /// </summary>
        Collect,
        /// <summary>
        /// 作弊指令界面
        /// </summary>
        Console,
        /// <summary>
        /// 过渡界面
        /// </summary>
        Transition,
        Skill,                      // 技能
        Bag,                        // 背包
        Dialog,                     // 对话
        Choose,                     // 选择
        Save,                       // 保存
    }
}
