using System.Collections.Generic;
using System;
using AKIRA.Attribute;
using AKIRA.UIFramework;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace AKIRA.Manager {
    /// <summary>
    /// 游戏启动器
    /// </summary>
    public class GameManager : MonoBehaviour {
        private void Awake() {
            Application.targetFrameRate = 60;
        }

        private async void Start() {
            EventSystem.Instance.AddEventListener(GameData.Event.OnLoadCompleted, InitSystems);
            // UI初始化，加载需要
            await CreateSystem<UIManager>(null);
            StartCoroutine(AssetSystem.Instance.LoadBundles());

            await UniTask.WaitUntil(() => AssetSystem.Instance.BundleLoadCompleted);
            EventSystem.Instance.TriggerEvent(GameData.Event.OnLoadCompleted);
        }

        /// <summary>
        /// 系统初始化
        /// </summary>
        /// <param name="data"></param>
        private async void InitSystems(object data) {
            EventSystem.Instance.RemoveEventListener(GameData.Event.OnLoadCompleted, InitSystems);

            var root = new GameObject("[Systems]").DontDestory();
            // base systems
            await CreateSystem<ObjectPool>(null);
            await CreateSystem<UpdateSystem>(root);

            // normal systems
            SortedDictionary<int, List<Type>> map = new();
            var launchers = ReflectionHelp.Handle<SystemLauncherAttribute>();
            // sort by attribute significance
            foreach (var launcher in launchers) {
                // 只有继承ISystem的标签才能在Launcher里实例化
                if (launcher.GetInterface("ISystem") == null)
                    continue;
                var attr = launcher.GetAttribute<SystemLauncherAttribute>();
                var significance = attr.significance;
                if (map.ContainsKey(significance)) {
                    map[significance].Add(launcher);
                } else {
                    map.Add(significance, new List<Type> { launcher });
                }
            }
            // create
            foreach (var value in map.Values) {
                foreach (var type in value) {
                    await CreateSystem(root, type);
                }
            }

            EventSystem.Instance.TriggerEvent(GameData.Event.OnInitSystemCompleted);

            GameObject.Destroy(this.gameObject);
        }

        /// <summary>
        /// 创建带Controller的Manager，用Init替代Start和Awake
        /// </summary>
        /// <param name="root"></param>
        /// <typeparam name="T"></typeparam>
        private async UniTask CreateSystem<T>(GameObject root) where T : ISystem {
           await CreateSystem(root, typeof(T));
        }

        /// <summary>
        /// 创建带Controller的Manager，用Init替代Start和Awake
        /// </summary>
        /// <param name="root"></param>
        /// <typeparam name="T"></typeparam>
        private async UniTask CreateSystem(GameObject root, Type type) {
            if (type.IsSubclassOf(typeof(Component))) {
                var system = new GameObject($"[{type.Name}]").AddComponent(type);
                system.SetParent(root.transform);
                await (system as ISystem).Initialize();
            } else {
                // 获得单例字段去实例化
                var property = type.BaseType.GetProperty("Instance");
                await (property.GetValue(type) as ISystem).Initialize();
            }
        }

    }
}