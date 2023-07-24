using UnityEngine;

namespace AKIRA.Behaviour.Camera {
    /// <summary>
    /// 摄像机移动脚本
    /// </summary>
    public class CameraFollowBehaviour : CameraBehaviour {
        // 跟随目标
        [SerializeField]
        // [ReadOnly]
        private Transform target;
        // 跟随速度
        [SerializeField]
        private float lerpSpeed;
        // 差值
        [SerializeField]
        private Vector3 offset;

        // 是否跟随
        public bool Follow = false;
        // 是否一直盯着
        public bool lookAt = false;

        public override void GameUpdate() {
            if (!Follow || target == null)
                return;
            
            this.transform.position =
                Vector3.Lerp(this.transform.position, target.position + offset, Time.deltaTime * lerpSpeed);
            
            if (lookAt)
                this.transform.LookAt(target);
        }

        /// <summary>
        /// 设置跟随目标
        /// </summary>
        /// <param name="target"></param>
        public void SetCameraFollowTarget(Transform target) {
            if (target == null || this.target.Equals(target))
                return;
            
            this.target = target;
        }

#if UNITY_EDITOR
        [ContextMenu("Save Offset")]
        private void SaveOffset() {
            if (target == null)
                return;

            offset = this.transform.position - target.position;
        }
#endif
    }
}