using System;
using System.Collections.Generic;
using UnityEngine;

namespace AKIRA.Behaviour.AI {
    /// <summary>
    /// 路径
    /// </summary>
    [ExecuteInEditMode]
    public class WayPath : MonoBehaviour {
        #region static methods/parameters
        private static Dictionary<string, WayPath> paths = new();
        /// <summary>
        /// 只读路径列表
        /// </summary>
        public static IReadOnlyDictionary<string, WayPath> Paths => paths;

        /// <summary>
        /// 获得路径字典
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, WayPath> GetPaths() {
            var paths = FindObjectsOfType<WayPath>();
            Dictionary<string, WayPath> result = new();
            foreach (var path in paths) {
                result.Add(path.wayTag, path);
            }

            return result;
        }
        
        /// <summary>
        /// 添加路径
        /// </summary>
        /// <param name="key"></param>
        /// <param name="path"></param>
        public static void AddWayPath(string key, WayPath path) {
            if (!paths.ContainsKey(key)) {
                paths.Add(key, path);
            } else {
                if (!paths[key].Equals(path)) {
                    $"WayPath Log: {key} has contained {paths[key]}, but set another {path}".Colorful(Color.yellow).Log();
                }
            }
        }

        /// <summary>
        /// 移除路径
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveWayPath(string key) {
            if (paths.ContainsKey(key)) {
                paths.Remove(key);
            }
        }

        /// <summary>
        /// 拿到路径
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static WayPath GetPath(string key) {
            if (paths.ContainsKey(key)) {
                return paths[key];
            }
            return default;
        }
        #endregion

        /// <summary>
        /// 路径标签
        /// </summary>
        public string wayTag = default;

        /// <summary>
        /// 一堆路径点
        /// </summary>
        private Waypoint[] points;

        private void Awake() {
            points = this.GetComponentsInChildren<Waypoint>();
        }

        private void OnEnable() {
            if (!String.IsNullOrWhiteSpace(wayTag)) {
                AddWayPath(wayTag, this);
            }
        }

        private void OnDisable() {
            if (!String.IsNullOrWhiteSpace(wayTag)) {
                RemoveWayPath(wayTag);
            }
        }

        /// <summary>
        /// 获得第一个点
        /// </summary>
        /// <returns></returns>
        public Waypoint GetFirstPoint() {
            return points[0];
        }

        /// <summary>
        /// 获得随机点
        /// </summary>
        /// <returns></returns>
        public Waypoint GetRandomPoint() {
            return points[UnityEngine.Random.Range(0, points.Length)];
        }
    }
}