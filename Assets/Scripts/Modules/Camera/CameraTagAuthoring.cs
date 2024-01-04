using AKIRA.Attribute;
using UnityEngine;

namespace AKIRA.Behaviour.Camera {
    /// <summary>
    /// 摄像机标签
    /// </summary>
    public class CameraTagAuthoring : MonoBehaviour {
        /// <summary>
        /// 标签
        /// </summary>
        [SerializeField]
        [SelectionPop(typeof(GameData.Camera))]
        private string cameraTag;

        private void Awake() {
            CameraExtend.AddCamera(cameraTag, this.gameObject);
            GameObject.Destroy(this);
        }
    }
}