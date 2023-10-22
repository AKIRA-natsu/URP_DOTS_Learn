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

            

            GameObject.Destroy(this.gameObject);
        }
    }
}