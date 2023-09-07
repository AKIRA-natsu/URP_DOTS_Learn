using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AKIRA.Behaviour.Camera {
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
}