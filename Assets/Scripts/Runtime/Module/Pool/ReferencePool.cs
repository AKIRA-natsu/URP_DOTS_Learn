using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace AKIRA.Manager {
    /// <summary>
    /// <para>引用池</para>
    /// </summary>
    public class ReferencePool : Singleton<ReferencePool> {

        private Dictionary<string, PoolBase> ReferenceMap = new Dictionary<string, PoolBase>();
        
        private ReferencePool() {}

        /// <summary>
        /// 获得引用对象
        /// </summary>
        /// <param name="target"></param>
        /// <param name="data">Pool 唤醒参数</param>
        /// <typeparam name="K"></typeparam>
        /// <returns></returns>
        public K Instantiate<K>(object data = null) where K : IPool, new() {
            var name = typeof(K).Name;
            if (ReferenceMap.ContainsKey(name)) {
                return (ReferenceMap[name] as RPool<K>).Instantiate(data);
            } else {
                var rpool = new RPool<K>().Init();
                ReferenceMap.Add(name, rpool);
                return rpool.Instantiate(data);
            }
        }

        /// <summary>
        /// <para>回收/销毁对象</para>
        /// </summary>
        /// <param name="data">Pool 回收参数</param>
        /// <typeparam name="K"></typeparam>
        public void Destory<K>(K @class, object data = null) where K : IPool, new() {
            var name = typeof(K).Name;
            if (ReferenceMap.ContainsKey(name)) {
                (ReferenceMap[name] as RPool<K>).Destroy(@class);
            } else {
                var rpool = new RPool<K>().Init();
                ReferenceMap.Add(name, rpool);
                rpool.Destroy(@class, data);
            }
        }

        /// <summary>
        /// 释放所有 <see cref="K" />
        /// </summary>
        /// <typeparam name="K"></typeparam>
        public void Destory<K>() where K : IPool, new() {
            var name = typeof(K).Name;
            if (ReferenceMap.ContainsKey(name)) {
                (ReferenceMap[name] as RPool<K>).Free();
                ReferenceMap.Remove(name);
                $"释放所有 {name} 的引用".Log(GameData.Log.Warn);
            }
        }
    }

    /// <summary>
    /// 引用池协助
    /// </summary>
    public static class ReferenceHelp {
        private static Dictionary<Component, List<IPool>> ComponentReferenceMap = new();
        /// <summary>
        /// 只读，面板使用
        /// </summary>
        public static IReadOnlyDictionary<Component, List<IPool>> ReadOnlyComponentReferenceMap => ComponentReferenceMap;

        /// <summary>
        /// 为Component添加对象引用
        /// </summary>
        /// <param name="component"></param>
        /// <param name="data">Pool 唤醒参数</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Attach<T>(this Component component, object data = null) where T : IPool, new() {
            var refer = ReferencePool.Instance.Instantiate<T>(data);
            if (ComponentReferenceMap.ContainsKey(component)) {
                ComponentReferenceMap[component].Add(refer);
            } else {
                ComponentReferenceMap.Add(component, new() { refer });
            }
            return refer;
        }

        /// <summary>
        /// 为 <see cref="component" /> 移除 <see cref="T" /> 引用
        /// </summary>
        /// <param name="component"></param>
        /// <param name="data">Pool 回收参数</param>
        /// <typeparam name="T"></typeparam>
        public static bool Detach<T>(this Component component, object data = null) where T : IPool, new() {
            bool result = false;
            if (ComponentReferenceMap.ContainsKey(component)) {
                var componentList = ComponentReferenceMap[component].Where(refer => refer is T);
                result = componentList.Count() != 0;
                if (result) {
                    // 回收
                    foreach (var refer in componentList) {
                        ReferencePool.Instance.Destory((T)refer, data);
                    }
                    // 移除键值
                    if (ComponentReferenceMap[component].Count == 0) {
                        ComponentReferenceMap.Remove(component);
                    }
                } else {
                    $"{component}不包含{typeof(T)}引用".Log(GameData.Log.Warn);
                }
            }
            return result;
        }

        /// <summary>
        /// 为 <see cref="component" /> 移除对象引用（指定）
        /// </summary>
        /// <param name="component"></param>
        /// <param name="refer"></param>
        /// <param name="data">Pool 回收参数</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Detach<T>(this Component component, T refer, object data = null) where T : IPool, new() {
            if (ComponentReferenceMap.ContainsKey(component)) {
                var componentList = ComponentReferenceMap[component];
                if (componentList.Contains(refer)) {
                    componentList.Remove(refer);
                    ReferencePool.Instance.Destory(refer, data);
                    if (componentList.Count == 0) {
                        ComponentReferenceMap.Remove(component);
                    }
                    return true;
                } else {
                    $"{component}不包含{typeof(T)}引用".Log(GameData.Log.Error);
                    return false;
                }
            } else {
                return false;
            }
        }

        /// <summary>
        /// 获得Component的 T 引用（多个中的第一个，正常应该只Attach一个引用）
        /// </summary>
        /// <param name="component"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetReference<T>(this Component component) where T : IPool, new() {
            if (ComponentReferenceMap.ContainsKey(component)) {
                return (T)ComponentReferenceMap[component].FirstOrDefault(refer => refer is T);
            } else {
                return default;
            }
        }

        /// <summary>
        /// 获得Component的所有引用
        /// </summary>
        /// <param name="component"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] GetReferences<T>(this Component component) where T : IPool, new() {
            if (ComponentReferenceMap.ContainsKey(component)) {
                return ComponentReferenceMap[component].Where(refer => refer is T).Cast<T>().ToArray();
            } else {
                return default;
            }
        }
    }
}