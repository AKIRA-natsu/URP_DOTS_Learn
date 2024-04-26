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
            public const string Console = "Console";
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
    
        #region 事件
        public class Event {
            /// <summary>
            /// 加载完成
            /// </summary>
            public const string OnLoadCompleted = "OnLoadCompleted";
            /// <summary>
            /// 系统初始化完成
            /// </summary>
            public const string OnInitSystemCompleted = "OnInitSystemCompleted";
            /// <summary>
            /// 指引完成
            /// </summary>
            public const string OnGuidenceCompleted = "OnGuidenceCompleted";
            /// <summary>
            /// 命令行指令
            /// </summary>
            public const string OnConsoleCommand = "OnConsoleCommand";
        }
        #endregion

        #region 资源
        public class Asset {
            /// <summary>
            /// UIManager
            /// </summary>
            public const string UIManager = "Assets/Res/MainBundle/Prefabs/UI/[UIManager].prefab";

            /// <summary>
            /// 故意留一个空的
            /// </summary>
            public const string Null = "Null";

            // Guide
            public const string Guide3DRoot = "Assets/Res/MainBundle/Prefabs/Guidence/[Guidence].prefab";       // 指引3D物体父类
            public const string GuideArrow = "Assets/Res/MainBundle/Prefabs/Guidence/Arrow.prefab";             // 指引3D箭头
            public const string GuideArrow2D = "Assets/Res/MainBundle/Prefabs/Guidence/Arrow2D.prefab";         // 指引2D箭头

            // Character
            public const string Kiara = "Assets/Res/MainBundle/Prefabs/Character/Kiara.prefab";                 // 角色模型 blender

            // Scene
            public const string Environment = "Assets/Res/MainBundle/Prefabs/Scene/[Environment].prefab";       // 示例测试场景
        }
        #endregion

        #region 存储
        public class SaveKey {
            public const string Guide = "GuideXML.xml";
            public const string GuideIndexKey = "GuideIndexKey";
        }
        #endregion
    }
}
