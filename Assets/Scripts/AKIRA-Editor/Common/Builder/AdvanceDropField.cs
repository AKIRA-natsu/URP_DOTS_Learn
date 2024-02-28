using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;

namespace AKIRA.Editor {
    /// <summary>
    /// 带全名
    /// </summary>
    public class FullAdvanceDropItem : AdvancedDropdownItem {
        public string fullName;

        public FullAdvanceDropItem(string name, string parent = null) : base(string.IsNullOrEmpty(name) ? "Null" : name) {
            if (string.IsNullOrEmpty(parent)) {
                fullName = name;
            } else {
                fullName = $"{parent}/{name}";
            }
        }
    }

    /// <summary>
    /// 下拉选择框
    /// </summary>
    public class AdvanceDropField : AdvancedDropdown {
        // 根节点名称
        private string rootName;
        // 选框值
        private IEnumerable<string> values;
        // 选中事件
        private Action<FullAdvanceDropItem> onSelectedCallback;

        // 缓存
        private static Dictionary<string, FullAdvanceDropItem> caches = new();

        public AdvanceDropField(string rootName, IEnumerable<string> values, Action<FullAdvanceDropItem> onSelectedCallback) : base(new()) {
            this.values = values;
            this.rootName = rootName;
            this.onSelectedCallback = onSelectedCallback;
        }

        protected override AdvancedDropdownItem BuildRoot() {
            if (caches.ContainsKey(rootName))
                return caches[rootName];

            var root = new FullAdvanceDropItem(rootName);
            foreach (var value in values)
                CreateDropDownItem(root, value);

            caches.Add(rootName, root);

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item) {
            base.ItemSelected(item);
            onSelectedCallback?.Invoke((FullAdvanceDropItem)item);
        }

        /// <summary>
        /// 实例化下拉按钮列表
        /// </summary>
        /// <param name="root"></param>
        /// <param name="value"></param>
        private void CreateDropDownItem(FullAdvanceDropItem root, string value) {
            FullAdvanceDropItem parent = root;
            foreach (var str in value.Split("/")) {
                var child = parent.children.SingleOrDefault(c => c.name.Equals(str));
                if (child == null) {
                    child = new FullAdvanceDropItem(str, parent == root ? "" : parent.fullName);
                    parent.AddChild(child);
                }
                parent = child as FullAdvanceDropItem;
            }
        }
    }
}