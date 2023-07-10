using System.Collections.Generic;
using AKIRA;
using AKIRA.Manager;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AKIRA.Manager {
    public class LogSystem : Singleton<LogSystem> {
        // 配置文件
        private LogConfig config;
        // 颜色表
        private Dictionary<string, (Color, bool)> ColorMap = new Dictionary<string, (Color, bool)>();

        // 是否详细
        private bool fully;
        
        protected LogSystem() {
            config = GameData.Path.LogConfig.Load<LogConfig>();
            fully = config?.logfully ?? true;

            // 默认四种
            ColorMap.Add(GameData.Log.Default, (Color.white, true));
            ColorMap.Add(GameData.Log.Success, (Color.green, true));
            ColorMap.Add(GameData.Log.Warn, (Color.yellow, true));
            ColorMap.Add(GameData.Log.Error, (Color.red, true));
        }

        public void Log(object message, string key, Object context = null) {
            var data = GetData(key);
            if (!data.logable)
                return;
#if UNITY_EDITOR
            Debug.Log(AddFullTag(message, key).Colorful(data.color), context);
#else
            // 非编辑器下不用富文本扩展
            Debug.Log(AddFullTag(message, key), context);
#endif
        }

        public void Warn(object message, Object context = null) {
            Debug.LogWarning(AddFullTag(message, GameData.Log.Warn), context);
        }

        public void Error(object message, Object context = null) {
            Debug.LogError(AddFullTag(message, GameData.Log.Error), context);
        }

        /// <summary>
        /// 获得颜色
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private (Color color, bool logable) GetData(string key) {
            if (ColorMap.ContainsKey(key)) {
                return ColorMap[key];
            } else {
                var color = config.GetData(key);
                ColorMap.Add(key, color);
                return color;
            }
        }

        /// <summary>
        /// 添加详细日志开头
        /// </summary>
        /// <param name="message"></param>
        /// <param name="key"></param>
        private string AddFullTag(object message, string key) {
            if (!fully)
                return $"<b>{message}</b>";
            else
                return $"<b>{key}日志： {message}</b>";
        }
    }
}

public static class LogExtend {
    /// <summary>
    /// 日志
    /// </summary>
    /// <param name="message"></param>
    /// <param name="key"></param>
    /// <param name="context">聚焦到Hierarchy，Debug.Log第二个参数</param>
    public static void Log(this object message, string key = GameData.Log.Default, Object context = null) {
        LogSystem.Instance.Log(message, key, context);
    }

    /// <summary>
    /// 日志
    /// </summary>
    /// <param name="message"></param>
    /// <param name="context">聚焦到Hierarchy，Debug.Log第二个参数</param>
    public static void Warn(this object message, Object context = null) {
        LogSystem.Instance.Warn(message, context);
    }

    /// <summary>
    /// 日志
    /// </summary>
    /// <param name="message"></param>
    /// <param name="context">聚焦到Hierarchy，Debug.Log第二个参数</param>
    public static void Error(this object message, Object context = null) {
        LogSystem.Instance.Error(message, context);
    }

}

