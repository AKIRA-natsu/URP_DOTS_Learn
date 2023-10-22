namespace AKIRA {
    /// <summary>
    /// <para>游戏整体数据存储</para>
    /// <para>可以通过Editor的Tools/AKIRA.Framework/GameDataWindow进行修改</para>
    /// </summary>
    public class GameData {
        #region 程序集
        public class DLL {
            /// <summary>
            /// 默认程序集
            /// </summary>
            public const string Default = "Assembly-CSharp";
            /// <summary>
            /// AKIRA Runtime
            /// </summary>
            public const string AKIRA_Runtime = "AKIRA.Core.Runtime";
            /// <summary>
            /// AKIRA Editor
            /// </summary>
            public const string AKIRA_Editor = "AKIRA.Core.Editor";
        }
        #endregion

        #region 路径
        public class Path {
            /// <summary>
            /// 日志配置文件
            /// </summary>
            public const string LogConfig = "Configs/LogConfig";
            /// <summary>
            /// 模块下载配置
            /// </summary>
            public const string ModuleDownloadConfig = "Configs/ModuleDownloadConfig";
            /// <summary>
            /// UI配置
            /// </summary>
            public const string UIConfig = "Configs/UIRuleConfig";
            /// <summary>
            /// ab包配置
            /// </summary>
            public const string AssetBundleConfig = "Configs/AssetBundleConfig";
        }
        #endregion

        #region 日志
        public class Log {
            /// <summary>
            /// 默认
            /// </summary>
            public const string Default = "Default";
            /// <summary>
            /// 成功
            /// </summary>
            public const string Success = "Success";
            /// <summary>
            /// 警告
            /// </summary>
            public const string Warn = "Warn";
            /// <summary>
            /// 错误
            /// </summary>
            public const string Error = "Error";
            /// <summary>
            /// 编辑器
            /// </summary>
            public const string Editor = "Editor";
            /// <summary>
            /// 游戏状态
            /// </summary>
            public const string GameState = "GameState";
            /// <summary>
            /// 事件
            /// </summary>
            public const string Event = "Event";
            /// <summary>
            /// 资源
            /// </summary>
            public const string Source = "Source";
            /// <summary>
            /// 指引
            /// </summary>
            public const string Guide = "Guide";
            /// <summary>
            /// UI
            /// </summary>
            public const string UI = "UI";
            /// <summary>
            /// AI
            /// </summary>
            public const string AI = "AI";
            /// <summary>
            /// 作弊
            /// </summary>
            public const string Cheat = "Cheat";
            /// <summary>
            /// 测试
            /// </summary>
            public const string Test = "Test";
        }
        #endregion
    
        #region 更新组
        public class Group {
            /// <summary>
            /// 默认
            /// </summary>
            public const string Default = "Default";
            /// <summary>
            /// UI
            /// </summary>
            public const string UI = "UI";
            /// <summary>
            /// 其他
            /// </summary>
            public const string Other = "Other";
            /// <summary>
            /// 组件
            /// </summary>
            public const string Component = "Component";
            /// <summary>
            /// 人形
            /// </summary>
            public const string Animator = "Animator";
            /// <summary>
            /// 动画
            /// </summary>
            public const string Animation = "Aniamtion";
        }
        #endregion
    
        #region 摄像机
        public class Camera {
            /// <summary>
            /// 主
            /// </summary>
            public const string Main = "Main";
            /// <summary>
            /// 子
            /// </summary>
            public const string Sub = "Sub";
            /// <summary>
            /// 渲染
            /// </summary>
            public const string Render = "Render";
            /// <summary>
            /// UI
            /// </summary>
            public const string UI = "UI";
        }
        #endregion
    }
}
