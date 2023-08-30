using System;
using System.Collections.Generic;

namespace AKIRA.Manager {
    /// <summary>
    /// 事件中心
    /// </summary>
    public class EventSystem : Singleton<EventSystem> {
        /// <summary>
        /// 事件表
        /// </summary>
        private Dictionary<string, Action<object>> EventMap = new Dictionary<string, Action<object>>();
        /// <summary>
        /// 被触发的事件表
        /// </summary>
        private Queue<(string key, object data)> TriggerMap = new Queue<(string key, object data)>();
        /// <summary>
        /// 是否触发中
        /// </summary>
        private bool triggering = false;

        protected EventSystem() {}

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public void AddEventListener(string key, Action<object> action) {
            if (String.IsNullOrEmpty(key) || action == null)
                return;

            if (EventMap.ContainsKey(key))
                EventMap[key] += action;
            else
                EventMap[key] = action;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public void RemoveEventListener(string key, Action<object> action) {
            if (String.IsNullOrEmpty(key) || action == null)
                return;

            if (EventMap.ContainsKey(key))
                EventMap[key] -= action;
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void TriggerEvent(string key, object value = null) {
            if (String.IsNullOrEmpty(key))
                return;

            TriggerMap.Enqueue((key, value));
            TriggerEvent();
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        private void TriggerEvent() {
            if (TriggerMap.Count != 0 && !triggering) {
                // 限制进入
                triggering = true;
                var value = TriggerMap.Dequeue();
                var key = value.key;
                // 防止中途清除事件，如果移除跳过
                if (EventMap.ContainsKey(key)) {
                    $"Trigger Event {key}".Log(GameData.Log.Event);
                    EventMap[key]?.Invoke(value.data);
                }
                triggering = false;
                // 继续下一个事件
                TriggerEvent();
            }
        }

        /// <summary>
        /// 清除事件
        /// </summary>
        /// <param name="key"></param>
        public void ClearEvent(string key) {
            if (String.IsNullOrEmpty(key))
                return;

            if (EventMap.ContainsKey(key))
                EventMap.Remove(key);
        }

        /// <summary>
        /// 清空事件
        /// </summary>
        public void ClearEvent() {
            EventMap.Clear();
        }
    }
}