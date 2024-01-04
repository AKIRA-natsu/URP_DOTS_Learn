using System;
using System.Collections.Generic;
using UnityEngine;

namespace AKIRA {
    /// <summary>
    /// 游戏配置文件
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "AKIRA.Framework/Common/GameConfig", order = 0)]
    public class GameConfig : ScriptableObject {
#if UNITY_EDITOR
        // 仅编辑器下用的配置
        [SerializeField]
        [HideInInspector]
        private List<ScriptableObject> editorConfigs;
#endif

        // 路径
        private const string Path = "GameConfig";

        private static GameConfig instance;
        public static GameConfig Instance {
            get {
#if UNITY_EDITOR
                return Path.Load<GameConfig>();
#else
                if (instance == null)
                    instance = Path.Load<GameConfig>();
                return instance;
#endif
            }
        }

        // runtime及编辑器下用的配置
        [SerializeField]
        [HideInInspector]
        private List<ScriptableObject> runtimeConfigs;

        /// <summary>
        /// 获得配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetConfig<T>() where T : ScriptableObject {
            return GetConfig(typeof(T)) as T;
        }

        /// <summary>
        /// 获得配置文件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ScriptableObject GetConfig(Type type) {
            if (!type.IsSubclassOf(typeof(ScriptableObject)))
                return default;
            
            #if UNITY_EDITOR
            var editorRes = SearchConfig(editorConfigs, type);
            if (editorRes != null)
                return editorRes;
            #endif

            return SearchConfig(runtimeConfigs, type);
        }

        /// <summary>
        /// 查找配置
        /// </summary>
        /// <param name="searchs"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private ScriptableObject SearchConfig(List<ScriptableObject> searchs, Type targetType) {
            foreach (var search in searchs)
                if (search.GetType() == targetType)
                    return search;
            return default;
        }
    }
}