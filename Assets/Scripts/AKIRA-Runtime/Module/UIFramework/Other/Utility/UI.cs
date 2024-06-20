using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace AKIRA.UIFramework {
    /// <summary>
    /// UI 静态数据保存 + 扩展
    /// </summary>
    public static class UI {
        // 根节点
        public static GameObject ManagerGo { get; private set; }
        // Canvas
        public static Canvas Canvas { get; private set; }
        // UI Transform
        public static RectTransform Rect { get; private set; }

        // 视图
        public static Transform View { get; private set; }
        // 背景 最下层
        public static Transform Background { get; private set; }
        // 顶部 最上层
        public static Transform Top { get; private set; }

        // 视图大小
        public static Vector2 CanvasSize { get; private set; }

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
            View = Rect.Find("Root/View");
            Background = Rect.Find("Root/Background");
            Top = Rect.Find("Root/Top");

            CanvasSize = Canvas.GetComponent<CanvasScaler>().referenceResolution;
        }

        /// <summary>
        /// 获得UI Game Object父节点
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static GameObject GetParent(WinType type) {
            return type switch
            {
                WinType.Normal => View.gameObject,
                WinType.Interlude => Top.gameObject,
                WinType.Notify => Background.gameObject,
                _ => View.gameObject,
            };
        }
    }

    /// <summary>
    /// UI 树
    /// </summary>
    public static class UITree {
        /// <summary>
        /// 根节点
        /// </summary>
        /// <value></value>
        public static WinNode Root { get; private set; }
        
        #if UNITY_EDITOR
        /// <summary>
        /// 是否已经完成构建，运行时只在 UIManager 构建一次，但是编辑器下面板有需要会构建的情况下，给一个 bool 判断是否构建完成
        /// </summary>
        /// <value></value>
        public static bool IsFinishBuild { get; private set; } = false;
        #endif

        /// <summary>
        /// 构建 UI 树
        /// </summary>
        /// <param name="uis"></param>
        public static void Build(IEnumerable<UIComponent> uis) {
            #if UNITY_EDITOR
            if (IsFinishBuild)
                return;
            #endif

            Root = new(UI.ManagerGo.transform);
            
            WinNode topNode = new(UI.Top, Root);
            WinNode viewNode = new(UI.View, Root);
            WinNode backNode = new(UI.Background, Root);

            foreach (var ui in uis) {
                WinData data;
                #if UNITY_EDITOR
                if (!Application.isPlaying)
                    data = ui.GetType().GetCustomAttribute<WinAttribute>().Data;
                else
                #endif
                    data = UIDataManager.Instance.GetUIData(ui);

                WinNode node;
                if (data.parent == WinEnum.None) {
                    node = data.type switch {
                        WinType.Normal => new WinNode(ui, viewNode),
                        WinType.Interlude => new WinNode(ui, topNode),
                        WinType.Notify => new WinNode(ui, backNode),
                        _ => new WinNode(ui, viewNode),
                    };
                } else {
                    node = new WinNode(ui, FindNode(data.parent));
                }

                var fields = ui.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(field => field.FieldType.IsSubclassOf(typeof(UIComponentProp)) && field.GetCustomAttribute<UIControlAttribute>() != null);
                
                foreach (var field in fields)
                    #if UNITY_EDITOR
                    if (!Application.isPlaying)
                        BuildPropNode(field.FieldType.CreateInstance() as UIComponentProp, node);
                    else
                    #endif
                        BuildPropNode(field.GetValue(ui) as UIComponentProp, node);
            }

            #if UNITY_EDITOR
            IsFinishBuild = true;
            #endif

        }

        /// <summary>
        /// UI往下建立 Prop 节点
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="node"></param>
        private static void BuildPropNode(UIComponentProp prop, WinNode node) {
            var newNode = new WinNode(prop, node);
            var fields = prop.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(field => field.FieldType.IsSubclassOf(typeof(UIComponentProp)));
            foreach (var field in fields)
                #if UNITY_EDITOR
                if (!Application.isPlaying)
                    BuildPropNode(field.FieldType.CreateInstance() as UIComponentProp, newNode);
                else
                #endif
                    BuildPropNode(field.GetValue(prop) as UIComponentProp, newNode);
        }

        /// <summary>
        /// 寻找目标节点
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static WinNode FindNode(WinEnum target) => FindNode(Root, target);
        private static WinNode FindNode(WinNode cur, in WinEnum target) {
            if (cur.self != null) {
                WinData data;
                #if UNITY_EDITOR
                if (!Application.isPlaying)
                    data = (cur.self.GetType().GetCustomAttribute(typeof(WinAttribute)) as WinAttribute).Data;
                else
                #endif
                    data = UIDataManager.Instance.GetUIData(cur.self);
                
                if (data.self == target)
                    return cur;
            }
            
            foreach (var child in cur.children) {
                var res = FindNode(child, target);
                if (res != null)
                    return res;
            }

            return default;
        }
    
        /// <summary>
        /// 获得在树中整体的序列
        /// </summary>
        /// <param name="node">目标</param>
        /// <param name="ignoreProp">是否忽略Prop节点</param>
        /// <returns></returns>
        public static int GetSortingInTree(in WinNode node, bool ignoreProp) {
            int res = 0;
            GetSortingInTree(Root, in node, ignoreProp, ref res);
            return res;
        }
        private static bool GetSortingInTree(WinNode cur, in WinNode target, in bool ignoreProp, ref int res) {
            if (cur.Equals(target))
                return false;

            // 是否子节点全是Prop节点，如果是，总数 + 1
            bool isAllIgnore = true;
            foreach (var child in cur.children) {
                if (child.Equals(target))
                    return false;
                
                if (ignoreProp && child.IsPropNode())
                    continue;

                isAllIgnore = false;
                if (child.IsLastNode()) {
                    res++;
                } else {
                    if (!GetSortingInTree(child, in target, in ignoreProp, ref res))
                        return false;
                }
            }

            if (isAllIgnore)
                res++;
            
            return true;
        }

    }
}