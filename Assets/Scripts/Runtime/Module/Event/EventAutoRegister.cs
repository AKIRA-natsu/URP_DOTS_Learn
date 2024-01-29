using System;
using System.Collections.Generic;
using AKIRA.Manager;
using UnityEngine;

namespace AKIRA.Manager {
    /// <summary>
    /// 事件数据
    /// </summary>
    public interface IEventData {
        // 键值
        string Key { get; }
        // 事件
        void Callback(object data = null);
    }

    internal abstract class RegistAutoEvent : MonoBehaviour {
        // 事件列表
        protected Dictionary<string, Action<object>> events = new();

        protected void Register() {
            foreach (var e in events)
                EventSystem.Instance.AddEventListener(e.Key, e.Value);
        }

        protected void UnRegister() {
            foreach (var e in events)
                EventSystem.Instance.RemoveEventListener(e.Key, e.Value);
        }

        /// <summary>
        /// Add Component 之后才会调用，所以里面还是要注册一遍
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public void AddEvent(string key, Action<object> callback) {
            events.Add(key, callback);
            EventSystem.Instance.AddEventListener(key, events[key]);
        }

        /// <summary>
        /// Add Component 之后才会调用，所以里面还是要注册一遍
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public void AddEvent(string key, Action callback) {
            events.Add(key, _ => callback.Invoke());
            EventSystem.Instance.AddEventListener(key, events[key]);
        }

        /// <summary>
        /// Add Component 之后才会调用，所以里面还是要注册一遍
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public void AddEvent(IEventData data) {
            var key = data.Key;
            events.Add(key, data.Callback);
            EventSystem.Instance.AddEventListener(key, events[key]);
        }
    }

    internal class RegistEnabledEvent : RegistAutoEvent {
        private void OnEnable() => Register();
        private void OnDisable() => UnRegister();
    }

    internal class RegistDestroyEvent : RegistAutoEvent {
        private void Awake() => Register();
        private void OnDestroy() => UnRegister();
    }
}

/// <summary>
/// 事件系统扩展
/// </summary>
public static class EventAutoRegister { 
    /// <summary>
    /// 注册 Enabled/Disabled 事件
    /// </summary>
    /// <param name="component"></param>
    /// <param name="key"></param>
    /// <param name="callback"></param>
    public static void RegistEnabledEvent(this Component component, string key, Action<object> callback) {
        component.GetOrAddComponent<RegistEnabledEvent>().AddEvent(key, callback);
    }

    /// <summary>
    /// 注册 Enabled/Disabled 事件
    /// </summary>
    /// <param name="component"></param>
    /// <param name="key"></param>
    /// <param name="callback"></param>
    public static void RegistEnabledEvent(this Component component, string key, Action callback) {
        component.GetOrAddComponent<RegistEnabledEvent>().AddEvent(key, callback);
    }

    /// <summary>
    /// 注册 Enabled/Disabled 事件
    /// </summary>
    /// <param name="component"></param>
    /// <param name="key"></param>
    /// <param name="callback"></param>
    public static void RegistEnabledEvent<T>(this Component component, T data) where T : IEventData {
        component.GetOrAddComponent<RegistEnabledEvent>().AddEvent(data);
    }

    /// <summary>
    /// 注册 Awake/Destroy 事件
    /// </summary>
    /// <param name="component"></param>
    /// <param name="key"></param>
    /// <param name="callback"></param>
    public static void RegistDestroyEvent(this Component component, string key, Action<object> callback) {
        component.GetOrAddComponent<RegistDestroyEvent>().AddEvent(key, callback);
    }

    /// <summary>
    /// 注册 Awake/Destroy 事件
    /// </summary>
    /// <param name="component"></param>
    /// <param name="key"></param>
    /// <param name="callback"></param>
    public static void RegistDestroyEvent(this Component component, string key, Action callback) {
        component.GetOrAddComponent<RegistDestroyEvent>().AddEvent(key, callback);
    }

    /// <summary>
    /// 注册 Awake/Destroy 事件
    /// </summary>
    /// <param name="component"></param>
    /// <param name="key"></param>
    /// <param name="callback"></param>
    public static void RegistDestroyEvent<T>(this Component component, T data) where T : IEventData {
        component.GetOrAddComponent<RegistDestroyEvent>().AddEvent(data);
    }

}