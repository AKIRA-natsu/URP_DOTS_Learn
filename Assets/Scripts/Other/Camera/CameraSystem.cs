using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
#endif
#endif

namespace AKIRA.Behaviour.Camera {
    // public partial struct CameraSystem : ISystem {
    //     [BurstCompile]
    //     public void OnCreate(ref SystemState state) { }

    //     [BurstCompile]
    //     public void OnUpdate(ref SystemState state) { }
    // }
    
    /// <summary>
    /// <para>摄像机跟随</para>
    /// <para>跟随目标需要挂载CameraFollowAuthoring脚本</para>
    /// </summary>
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct CameraFollowSystem : ISystem {
        // 跟随实体
        private Entity target;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<CameraFollowComponent>();
        }

        public void OnUpdate(ref SystemState state) {
            // 获得CameraFollowComponent组件实体
            if (target == Entity.Null) {
                var query = SystemAPI.QueryBuilder().WithAll<CameraFollowComponent>().Build();
                var targets = query.ToEntityArray(Allocator.Temp);
                if (targets.Length == 0)
                    return;
                target = targets[0];
            }

            var camera = CameraExtend.Transform;
            var targetTrans = SystemAPI.GetComponent<LocalToWorld>(target);
            var followComponent = SystemAPI.GetComponent<CameraFollowComponent>(target);

            // 判断是否跟随
            if (followComponent.follow)
                camera.position = 
                    Vector3.Lerp(camera.position, targetTrans.Position + followComponent.offset, SystemAPI.Time.DeltaTime * followComponent.lerpSpeed);

        
            // 判断是否看着
            if (followComponent.lookAt)
                camera.LookAt(targetTrans.Position);
        }
    }

    /// <summary>
    /// <para>摄像机拖拽脚本</para>
    /// <para>拖拽目标继承IDrag接口</para>
    /// <para>SubScene无法使用。。</para>
    /// </summary>
    public partial class CameraDragSystem : SystemBase {
        // 拖拽目标
        private IDrag dragObject;

        protected override void OnCreate() {
            base.OnCreate();
#if ENABLE_INPUT_SYSTEM
    #if UNITY_ANDROID || UNITY_IOS
            EnhancedTouchSupport.Enable();
    #endif
#endif
        }

        protected override void OnUpdate() {
#if ENABLE_INPUT_SYSTEM
#if UNITY_ANDROID || UNITY_IOS
            if (Touch.activeTouches.Count == 0 && dragObject != null)
#else
            if (Mouse.current.leftButton.wasReleasedThisFrame && dragObject != null)
#endif
#else
            if (Input.GetMouseUp(0))
#endif
            {
                dragObject.OnDragUp();
                dragObject = null;
            }

#if ENABLE_INPUT_SYSTEM
#if UNITY_ANDROID || UNITY_IOS
            if (Touch.activeTouches.Count > 0 && dragObject == null)
#else
            if (Mouse.current.leftButton.wasPressedThisFrame && dragObject == null)
#endif
#else
            if (Input.GetMouseDown(0))
#endif
            {
#if ENABLE_INPUT_SYSTEM
#if UNITY_ANDROID || UNITY_IOS
                Ray ray = CameraExtend.MainCamera.ScreenPointToRay(Touch.activeTouches[0].screenPosition);
#else
                Ray ray = CameraExtend.MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
#endif
#else
                Ray ray = CameraExtend.MainCamera.ScreenPointToRay(Input.mousePosition);
#endif
                var hits = Physics.RaycastAll(ray, System.Single.MaxValue);
                foreach (var hit in hits) {
                    // 拿到第一个IDrag
                    if (hit.transform.TryGetComponent<IDrag>(out dragObject)) {
                        dragObject.OnDragDown();
                        break;
                    }
                }
            }


            if (dragObject != null)
                dragObject.OnDrag();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class CameraClickSystem : SystemBase {
        protected override void OnCreate() {
            base.OnCreate();
        }

        protected override void OnUpdate() {
#if ENABLE_INPUT_SYSTEM
    #if UNITY_ANDROID || UNITY_IOS
            if (Touch.activeTouches.Count > 0) {
                Ray ray = CameraExtend.MainCamera.ScreenPointToRay(Touch.fingers[0].screenPosition);
    #else
            if (Mouse.current.leftButton.wasPressedThisFrame) {
                Ray ray = CameraExtend.MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
    #endif
#else
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = CameraExtend.MainCamera.ScreenPointToRay(Input.mousePosition);
#endif
                if (Physics.Raycast(ray, out RaycastHit hit, System.Single.MaxValue)) {
                    if (hit.transform.TryGetComponent<IClick>(out IClick click)) {
                        click.OnClick();
                    }
                }
            }
        }
    }
}