using AKIRA.Manager;
using UnityEngine;

namespace AKIRA.UIFramework {
    /// <summary>
    /// 红点组件，给一个样例，也能直接用。
    /// </summary>
    public class ReddotComponent : MonoBehaviour, IReddot {
        [SerializeField]
        private string reddotTag;
        public string Tag => reddotTag;

        private void Awake() {
            EventSystem.Instance.AddEventListener(GameData.Event.OnInitSystemCompleted, RegistReddot);
        }

        private void RegistReddot(object obj) {
            World.GetWorldController<ReddotController>()?.RegistReddot(this);
        }

        public void Active(bool active) {
            this.gameObject.SetActive(active);
        }

        private void OnDestroy() {
            World.GetWorldController<ReddotController>()?.RemoveReddot(this);
        }
    }
}