using System.Collections.Generic;
using AKIRA.Manager;

namespace AKIRA.UIFramework {
    /// <summary>
    /// UI 数据管理
    /// </summary>
    internal class UIDataManager : Singleton<UIDataManager> {
        private Dictionary<UIComponent, WinData> ComDataMap = new();

        private UIDataManager() {}

        /// <summary>
        /// 注册 UI
        /// </summary>
        /// <param name="com"></param>
        /// <param name="data"></param>
        public void Register(UIComponent com, WinData data) {
            if (ComDataMap.ContainsKey(com)) {
                $"ComMap contains {com}".Error();
                return;
            }
            ComDataMap[com] = data;
        }

        /// <summary>
        /// 获得 UI 数据
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public WinData GetUIData(UIComponent com) {
            if (!ComDataMap.ContainsKey(com)) {
                $"ComMap dont contain {com}".Error();
                return default;
            }
            return ComDataMap[com];
        }

        /// <summary>
        /// 移除 UI
        /// </summary>
        /// <param name="com"></param>
        public void Remove(UIComponent com) {
            if (!ComDataMap.ContainsKey(com)) {
                $"ComMap dont contain {com}".Error();
                return;
            }
            ComDataMap.Remove(com);
        }
    }
}