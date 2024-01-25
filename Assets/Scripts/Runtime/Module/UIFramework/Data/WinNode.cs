using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AKIRA.UIFramework {
    /// <summary>
    /// window 节点
    /// </summary>
    public class WinNode {
        // 父节点
        public WinNode parent;
        // 子节点列表
        public List<WinNode> children;

        // 自身
        public UIComponent self;
        // 自身，适用非UI脚本，，比如UI最上面的View，Top，Background三大节点和根节点
        public Transform self_Trans;

        #region constructor
        public WinNode(Transform self, WinNode parent = null) {
            children = new();
            this.parent = parent;
            this.self_Trans = self;

            if (parent != null)
                parent.children.Add(this);
        }

        public WinNode(UIComponent self, WinNode parent = null) {
            children = new();
            this.parent = parent;
            this.self = self;
            this.self_Trans = self.transform;

            if (parent != null)
                parent.children.Add(this);
        }
        #endregion

        #region node type check
        /// <summary>
        /// 是否是根节点 [UIManager]
        /// </summary>
        /// <returns></returns>
        public bool IsRootNode() => parent == null && IsTopNode();
        /// <summary>
        /// 是否是顶层节点 [Top], [View], [Background]
        /// </summary>
        /// <returns></returns>
        public bool IsTopNode() => self == null && self_Trans != null;
        /// <summary>
        /// 是否是最后一个节点
        /// </summary>
        /// <returns></returns>
        public bool IsLastNode() => children.Count(child => child.IsWindowNode()) == 0;
        /// <summary>
        /// 是否是UI节点
        /// </summary>
        /// <returns></returns>
        public bool IsWindowNode() => !IsPropNode();
        /// <summary>
        /// 是否是UI组件节点
        /// </summary>
        /// <returns></returns>
        public bool IsPropNode() => self.GetType().IsSubclassOf(typeof(UIComponentProp));
        #endregion

        /// <summary>
        /// 寻找目标节点
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public WinNode FindNode(WinEnum target) {
            WinData? data;
            #if UNITY_EDITOR
            if (!Application.isPlaying)
                data = (self.GetType().GetCustomAttribute(typeof(WinAttribute)) as WinAttribute).Data;
            else
            #endif
                data = UIDataManager.Instance.GetUIData(self);

            // 判断是否是空
            if (data == null)
                return default;

            // 获得指定类型
            if (data?.self == target)
                return this;
            
            // 遍历子节点获得类型
            foreach (var child in children) {
                if (child.IsPropNode())
                    continue;
                var res = child.FindNode(target);
                if (res != default)
                    return res;
            }

            return default;
        }

        public override string ToString() {
            return $"UI Node {(self == null ? self_Trans : self)}";
        }

    }
}