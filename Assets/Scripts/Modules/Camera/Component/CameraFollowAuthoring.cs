using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AKIRA.Behaviour.Camera {
#if UNITY_2022_1_OR_NEWER
    #region UNITY_2022 Entity System
    /// <summary>
    /// 摄像机跟随
    /// </summary>
    public class CameraFollowAuthoring : MonoBehaviour {
        // 跟随速度
        [SerializeField]
        private float lerpSpeed;
        // 差值
        [SerializeField]
        private Vector3 offset;

        // 是否跟随
        public bool follow = false;
        // 是否一直盯着
        public bool lookAt = false;

#if UNITY_EDITOR
        [ContextMenu("Save Offset")]
        private void SaveOffset() {
            if (UnityEngine.Camera.main == null)
                return;

            offset = UnityEngine.Camera.main.transform.position - this.transform.position;
        }
#endif

        class Baker : Baker<CameraFollowAuthoring> {
            public override void Bake(CameraFollowAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CameraFollowComponent {
                    lerpSpeed = authoring.lerpSpeed,
                    offset = authoring.offset,
                    follow = authoring.follow,
                    lookAt = authoring.lookAt
                });
            }
        }

    }

    public struct CameraFollowComponent : IComponentData {
        // 跟随速度
        public float lerpSpeed;
        // 差值
        public float3 offset;

        // 是否跟随
        public bool follow;
        // 是否一直盯着
        public bool lookAt;
    }
    #endregion
#else
    #region UNITY_2021(or lower)
    public class CameraFollowAuthoring : MonoBehaviour {
        // 目标
        [SerializeField]
        private Transform lookTarget;
        // 插值
        [SerializeField]
        private Vector3 offsetPosition;
        [SerializeField]
        private Vector3 offsetRotation;
        
        [Space]
        // 是否短时间看
        public bool isIntervalLook = false;
        // 时间
        public float lookTime;

        private void OnEnable() {
            CameraSystem.Instance.AddLookTarget(this);
        }

        private void OnDisable() {
            CameraSystem.Instance.RemoveLookTarget(this);
        }

        /// <summary>
        /// 获得坐标
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPosition() => lookTarget.position + offsetPosition;
        /// <summary>
        /// 获得朝向
        /// </summary>
        /// <returns></returns>
        public Quaternion GetRotation() => Quaternion.Euler(lookTarget.eulerAngles + offsetRotation);

        #if UNITY_EDITOR
        /// <summary>
        /// 设置插值
        /// </summary>
        [ContextMenu("Set Offset")]
        private void SetOffset() {
            if (lookTarget == null)
                return;

            offsetPosition = this.transform.position - lookTarget.position;
            offsetRotation = this.transform.eulerAngles - lookTarget.eulerAngles;
        }
        #endif
    }
    #endregion
#endif
}