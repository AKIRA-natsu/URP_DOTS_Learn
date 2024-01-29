using System.Collections.Generic;
using UnityEngine;

namespace AKIRA.UIFramework {
    /// <summary>
    /// window 节点
    /// </summary>
    public sealed partial class WinNode {
        #region params
        // 父节点
        public WinNode parent;
        // 子节点列表
        public List<WinNode> children;

        // 自身
        public UIComponent self;
        // 自身，适用非UI脚本，，比如UI最上面的View，Top，Background三大节点和根节点
        public Transform self_Trans;
        #endregion

        #region property
        // 节点名称
        // 类型 和 预制体 名称是不一样的。。
        public string Name => self != null ? self.GetType().Name : self_Trans.name;
        #endregion

        #region constructor
        public WinNode(Transform self, WinNode parent = null) {
            Init(self, parent);
        }

        public WinNode(UIComponent self, WinNode parent = null) {
            this.self = self;
            Init(self.transform, parent);
        }

        private void Init(Transform self, WinNode parent) {
            children = new();
            this.parent = parent;
            this.self_Trans = self;

            if (parent != null)
                parent.children.Add(this);

            GetReddots();
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
        public bool IsLastNode() => children.Count == 0;
        /// <summary>
        /// 是否是UI节点
        /// </summary>
        /// <returns></returns>
        public bool IsWindowNode() => !IsPropNode();
        /// <summary>
        /// 是否是UI组件节点
        /// </summary>
        /// <returns></returns>
        public bool IsPropNode() => self != null && self.GetType().IsSubclassOf(typeof(UIComponentProp));
        #endregion

        #region hierarchy
        /// <summary>
        /// 是否是统一层级节点
        /// </summary>
        /// <param name="target"></param>
        public bool IsSameHierarchy(WinNode target) {
            if (target.parent == null || parent == null)
                return false;
            return target.parent == parent;
        }

        /// <summary>
        /// 是否当前节点的子节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsChildren(WinNode node) {
            return children.Contains(node);
        }

        /// <summary>
        /// 获得同级节点的排序
        /// </summary>
        /// <returns></returns>
        public int GetSorting() {
            if (parent == null)
                return 0;

            return parent.children.IndexOf(this);
        }

        /// <summary>
        /// 获得节点深度
        /// </summary>
        /// <returns></returns>
        public int GetDepth() => GetDepth(this, 0);
        private int GetDepth(WinNode cur, int depth) {
            if (cur.parent == default)
                return depth;
            return GetDepth(cur.parent, ++depth);
        }
        #endregion

        public override string ToString() {
            return $"UI Node {Name}";
        }

    }
}