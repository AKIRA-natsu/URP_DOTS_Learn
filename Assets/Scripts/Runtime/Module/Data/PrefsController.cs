using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AKIRA.Manager {
    /// <summary>
    /// 本地存储管理器
    /// </summary>
    public class PrefsController : IController {
        private readonly Dictionary<string, IInfo> infos = new();

        public async Task Initialize() {
            await Task.Yield();
            var types = typeof(IInfo).Name.GetConfigTypeByInterface();
            foreach (var type in types)
                infos.Add(type.Name, InfoUtils.Read(type));
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T ReadInfo<T>() where T : class, IInfo {
            return infos[typeof(T).Name] as T;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void SaveInfo<T>() where T : IInfo {
            infos[typeof(T).Name].Save();
        }
    }
}

/// <summary>
/// 存储接口
/// </summary>
public interface IInfo { }

/// <summary>
/// 存储扩展
/// </summary>
internal static class InfoUtils {
    public static void Save(this IInfo info) {
        info.GetType().Name.Save(JsonConvert.SerializeObject(info));
    }

    public static T Read<T>() where T : class, IInfo => Read(typeof(T)) as T;

    public static IInfo Read(Type type) {
        var json = type.Name.GetString();
        if (string.IsNullOrEmpty(json))
            return type.CreateInstance<IInfo>();
        else
            try {
                return JsonConvert.DeserializeObject<IInfo>(json);
            } catch {
                return type.CreateInstance<IInfo>();
            }
    }
}