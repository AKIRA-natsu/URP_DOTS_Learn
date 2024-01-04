using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace AKIRA.Manager {
    public class AssetSystem : Singleton<AssetSystem> {
        // 配置
        private AssetBundleConfig config;
        // 前缀
        private string preformPath;
        // 前缀
        public string PreformPath => preformPath;

        /// <summary>
        /// 资源是否加载完成
        /// </summary>
        public bool BundleLoadCompleted { get; private set; } = false;
        /// <summary>
        /// 加载进度
        /// </summary>
        public float LoadProgress { get; private set; }

        /// <summary>
        /// 资源
        /// </summary>
        private AssetBundle[] bundles;

        /// <summary>
        /// 对加载过的物体做一个记录
        /// </summary>
        private Dictionary<string, Object> records = new();

        protected AssetSystem() {
            config = GameConfig.Instance.GetConfig<AssetBundleConfig>();
            preformPath = 
#if UNITY_ANDROID && !UNITY_EDITOR
                "jar:file://" + Application.dataPath + "!/assets";
#elif UNITY_IOS && !UNITY_EDITOR
                Application.dataPath + "/Raw";
#else
                Application.streamingAssetsPath;
#endif
        }

        public IEnumerator LoadBundles() {
#if UNITY_EDITOR
            if (!config.simulation) {
                BundleLoadCompleted = true;
                LoadProgress = 1f;
                yield break;
            }
#endif

            var paths = config.paths;
            bundles = new AssetBundle[paths.Length];

            if (paths.Length == 0) {
                BundleLoadCompleted = true;
                LoadProgress = 1f;
                "不存在AB包".Log();
                yield break;
            }

#if UNITY_EDITOR
            if (!config.useWebRequestTest) {
                for (int i = 0; i < paths.Length; i++) {
                    var path = paths[i];
                    $"AB: AssetBundle.LoadFromFile 尝试读取 => {Path.Combine(preformPath, path)}".Log(GameData.Log.Editor);
                    bundles[i] = AssetBundle.LoadFromFile(Path.Combine(preformPath, path));
                    LoadProgress = (float)i / path.Length;
                }
            } else
#endif
            {
                for (int i = 0; i < paths.Length; i++) {
                    var path = paths[i];
                    $"AB: UnityWebRequestAssetBundle 尝试读取 => {Path.Combine(preformPath, path)}".Log(GameData.Log.Editor);
                    var request = UnityWebRequestAssetBundle.GetAssetBundle(Path.Combine(preformPath, path));
                    yield return request.SendWebRequest();

                    if (!request.isDone) {
                        $"AB包请求失败 path => {Path.Combine(preformPath, path)}".Error();
                    }
                    
                    bundles[i] = DownloadHandlerAssetBundle.GetContent(request);
                    LoadProgress = (float)i / path.Length;
                }
            }

            BundleLoadCompleted = true;
            "加载AB包成功".Log(GameData.Log.Success);

#if UNITY_WEBGL
            // webgl用的web request获得ab包
#endif

        }

        public T LoadObject<T>(string path) where T : Object {
            if (records.ContainsKey(path))
                return records[path] as T;

            path.Log();
#if UNITY_EDITOR
            if (!config.simulation) {
                var res = path.LoadAssetAtPath<T>();
                records[path] = res;
                return res;
            }
#endif

            foreach (var bundle in bundles) {
                if (!bundle.Contains(path))
                    continue;
                
                T res = default;
                if (typeof(T).IsSubclassOf(typeof(Component)))
                    res = bundle.LoadAsset<GameObject>(path).GetComponent(typeof(T)) as T;
                else
                    res = bundle.LoadAsset<T>(path);
                records[path] = res;
                return res;
            }
            return default;
        }

        /// <summary>
        /// 卸载所有AB包
        /// </summary>
        /// <param name="unloadAllObjects"></param>
        public void UnloadAssetBundles(bool unloadAllObjects = false) {
            // 如果是true，会把加载的资源一同卸载了
            AssetBundle.UnloadAllAssetBundles(unloadAllObjects);
        }
    }
}