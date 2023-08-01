using AKIRA.Attribute;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
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

        // private void Awake() {
        //     CameraExtend.AddCamera(cameraTag, this.gameObject);
        //     GameObject.Destroy(this);
        // }

        class Baker : Unity.Entities.Baker<CameraTagAuthoring>
        {
            public override void Bake(CameraTagAuthoring authoring)
            {
                var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
                manager.Log();
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                entity.Log();
                AddComponent(entity, new CameraTagComponent {
                    tag = authoring.cameraTag
                });
            }
        }
    }

    public struct CameraTagComponent : IComponentData {
        public FixedString32Bytes tag;
    }
}