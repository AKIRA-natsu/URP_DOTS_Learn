using AKIRA;
using AKIRA.Attribute;
using UnityEngine;

namespace AKIRA.Behaviour.Camera {
    /// <summary>
    /// 摄像机标签
    /// </summary>
    public class CameraTag : MonoBehaviour {
        /// <summary>
        /// 标签
        /// </summary>
        [SerializeField]
        [SelectionPop(typeof(GameData.Camera))]
        private string tag;

        private void Awake() {
            CameraExtend.AddCamera(tag, this.gameObject);
            this.gameObject.SetActive(false);
            this.Destory();
        }
    }
}