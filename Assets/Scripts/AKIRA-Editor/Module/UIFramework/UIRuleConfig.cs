using System.Collections.Generic;
using AKIRA.Attribute;
using UnityEngine;

namespace AKIRA.Editor {
    /// <summary>
    /// UI命名配置
    /// </summary>
    [CreateAssetMenu(fileName = "UIRuleConfig", menuName = "AKIRA.Framework/Module/UIRuleConfig", order = 0)]
    public class UIRuleConfig : ScriptableObject {
        [Header("Generator Paths")]
        // 预制体路径
        [SerializeField]
        [SelectionPath]
        internal string prefabPath;
        // 脚本路径
        [SerializeField]
        [SelectionPath]
        internal string scriptPath;

        [Header("Name Rule Parts")]
        /// <summary>
        /// 规则
        /// </summary>
        [SerializeField]
        private UIControlRule[] rules;

        /// <summary>
        /// 忽略名单
        /// </summary>
        [SerializeField]
        private string[] ignores;

        [Space]
        [Header("RectTransform Inspector Extend")]
        /// <summary>
        /// 是否绘制删除按钮
        /// </summary>
        [SerializeField]
        internal bool drawDeleteBtn;
        /// <summary>
        /// 是否开启红点，仅针对按钮
        /// </summary>
        [SerializeField]
        internal bool enableRedDot;
        /// <summary>
        /// 红点预制体
        /// </summary>
        [SerializeField]
        internal GameObject reddotPrefab;

        /// <summary>
        /// <para>获得组件名称（不判断大小写）</para>
        /// <para>获取优先级，与排序有关</para>
        /// </summary>
        /// <param name="name1">UI组件命名名称</param>
        /// <param name="name2">UI规定组件名称</param>
        /// <returns>是否找到组件</returns>
        public bool TryGetControlName(string name1, out string name2) {
            foreach (var rule in rules) {
                foreach (var n in rule.ruleNames) {
                    if (name1.Contains(n)) {
                        name2 = rule.controlName;
                        return true;
                    }
                }
            }
            name2 = null;
            return false;
        }

        /// <summary>
        /// <para>获得组件名称（不判断大小写）</para>
        /// <para>参数重复问题，暂不使用</para>
        /// </summary>
        /// <param name="name">UI组件命名名称</param>
        /// <param name="names">UI规定组件名称 列表</param>
        /// <returns>是否找到组件</returns>
        public bool TryGetControlName(string name, out List<string> names) {
            names = new List<string>();
            foreach (var rule in rules)  {
                foreach (var n in rule.ruleNames) {
                    if (name.Contains(n)) {
                        names.Add(rule.controlName);
                        break;
                    }
                }
            }
            if (names.Count == 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 是否是忽略名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsIgnoreName(string name) {
            foreach (var ignore in ignores)
                if (name.Contains(ignore))
                    return true;
            return false;
        }

        /// <summary>
        /// 检查是否可适配
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool CheckMatchableControl(string name) {
            if (name.Contains("@"))
                return true;
            return false;
        }

    }
}