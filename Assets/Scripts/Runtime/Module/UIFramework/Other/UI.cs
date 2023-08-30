using UnityEngine;

namespace AKIRA.UIFramework {
    public static class UI {
        // UIManager路径
        internal const string UIManagerPath = "Prefabs/UI/[UIManager]";
        // 根节点
        public static GameObject ManagerGo { get; private set; }
        // Canvas
        public static Canvas Canvas { get; private set; }
        // UI Transform
        public static RectTransform Rect { get; private set; }

        // 视图
        public static GameObject View { get; private set; }
        // 背景 最下层
        public static GameObject Background { get; private set; }
        // 顶部 最上层
        public static GameObject Top { get; private set; }

        private static Camera uiCamera;
        /// <summary>
        /// UI摄像机
        /// </summary>
        /// <value></value>
        public static Camera UICamera {
            get {
                if (uiCamera == null)
                    uiCamera = ManagerGo.GetComponentInChildren<Camera>();
                return uiCamera;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="root">根节点</param>
        public static void Initialize(GameObject root) {
            ManagerGo = root;
            Canvas = ManagerGo.GetComponentInChildren<Canvas>();
            Rect = Canvas.GetComponent<RectTransform>();
            View = Rect.Find("Root/View").gameObject;
            Background = Rect.Find("Root/Background").gameObject;
            Top = Rect.Find("Root/Top").gameObject;
        }
    }
}