using AKIRA.UIFramework;
using Cysharp.Threading.Tasks;
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
            StartCoroutine(AssetSystem.Instance.LoadBundles());

            await UniTask.WaitUntil(() => AssetSystem.Instance.BundleLoadCompleted);
            EventSystem.Instance.TriggerEvent(GameData.Event.OnLoadCompleted);
        }

        /// <summary>
        /// UI初始化
        /// </summary>
        /// <param name="data"></param>
        private void InitSystems(object data) {
            EventSystem.Instance.RemoveEventListener(GameData.Event.OnLoadCompleted, InitSystems);

            // Systems Init
            UIManager.Instance.Initialize();

            EventSystem.Instance.TriggerEvent(GameData.Event.OnInitSystemCompleted);

            GameObject.Destroy(this.gameObject);
        }

        /// <summary>
        /// 创建带Controller的Manager，用Init替代Start和Awake
        /// </summary>
        /// <param name="root"></param>
        /// <typeparam name="T"></typeparam>
        private void CreateSystem<T>(GameObject root) where T : MonoSingleton<T> {
            var system = new GameObject($"[{typeof(T).Name}]").AddComponent<T>();
            system.SetParent(root.transform);
            system.Initialize();
        }

    }
}