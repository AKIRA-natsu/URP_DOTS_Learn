using System.Collections.Generic;
using UnityEngine;

namespace AKIRA.UIFramework {
    /// <summary>
    /// UI命名配置
    /// </summary>
    [CreateAssetMenu(fileName = "UIRuleConfig", menuName = "AKIRA.Framework/Module/UIRuleConfig", order = 0)]
    public class UIRuleConfig : ScriptableObject {
        /// <summary>
        /// 规则
        /// </summary>
        [SerializeField]
        private UIControlRule[] rules;

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
            // $"rules dont contain {name1}".Error();
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