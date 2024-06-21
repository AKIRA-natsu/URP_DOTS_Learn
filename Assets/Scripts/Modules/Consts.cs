namespace AKIRA
{
    public class Consts
    {
        #region 事件
        public class Event
        {
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
        public class Asset
        {
            /// <summary>
            /// UIManager
            /// </summary>
            public const string UIManager = "Assets/Res/MainBundle/Prefabs/UI/[UIManager].prefab";

            // Guide
            public const string Guide3DRoot = "Assets/Res/MainBundle/Prefabs/Guidence/[Guidence].prefab";       // 指引3D物体父类
            public const string GuideArrow = "Assets/Res/MainBundle/Prefabs/Guidence/Arrow.prefab";             // 指引3D箭头
            public const string GuideArrow2D = "Assets/Res/MainBundle/Prefabs/Guidence/Arrow2D.prefab";         // 指引2D箭头
        }
        #endregion

        #region 存储
        public class SaveKey
        {
            public const string Guide = "GuideXML.xml";
            public const string GuideIndexKey = "GuideIndexKey";
        }
        #endregion
    }
}