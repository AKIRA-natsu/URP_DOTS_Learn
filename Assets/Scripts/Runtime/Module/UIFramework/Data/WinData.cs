using System;

namespace AKIRA.UIFramework {
    /// <summary>
    /// UI 数据
    /// </summary>
    [Serializable]
    public struct WinData {
        /// <summary>
        /// UI 窗口
        /// </summary>
        public WinEnum self;
        /// <summary>
        /// UI 窗口 父亲
        /// </summary>
        public WinEnum parent;
        /// <summary>
        /// UI 路劲 Resources路劲下
        /// </summary>
        public string path;
        /// <summary>
        /// UI 类型
        /// </summary>
        public WinType @type;

        public WinData(WinEnum self, WinEnum parent, string path, WinType @type) {
            this.self = self;
            this.parent = parent;
            this.path = path;
            this.@type = @type;
        }

        public override string ToString() {
            return $"{self}: path => {path}, type => {@type}";
        }
    }
}