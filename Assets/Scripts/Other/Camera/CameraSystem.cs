using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using AKIRA.Manager;
using System.Linq;
using System.Threading.Tasks;


#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
#endif
#endif

namespace AKIRA.Behaviour.Camera {
#if UNITY_2022_1_OR_NEWER
    #region UNITY_2022 Entity System
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
    #endregion
#else
    #region UNITY_2021(or lower) AKIRA.System
    public class CameraSystem : Singleton<CameraSystem>, IUpdate {
        // 跟随目标数组
        private List<CameraFollowAuthoring> targets = new();
        /// <summary>
        /// 当前跟随目标
        /// </summary>
        public CameraFollowAuthoring CurFollowTarget => targets?.LastOrDefault();

        // 平滑时间
        private const float SmoothTime = 1f;
        // 
        private Vector3 velocity;

        // 是否被占用（繁忙）
        private bool isBusying = false;

        // 拖拽目标
        private IDrag dragObject;

        protected CameraSystem() {}

        public override async Task Initialize() {
            await Task.Yield();
            this.Regist();
        }

        /// <summary>
        /// 添加跟随目标
        /// </summary>
        /// <param name="target"></param>
        public void AddLookTarget(CameraFollowAuthoring target) {
            if (targets.Contains(target))
                return;
            targets.Add(target);
        }

        /// <summary>
        /// 移除跟随目标
        /// </summary>
        /// <param name="target"></param>
        public void RemoveLookTarget(CameraFollowAuthoring target) {
            if (!targets.Contains(target))
                return;
            targets.Remove(target);
        }

        public void GameUpdate() {
            // 忙碌中
            if (isBusying)
                return;
            
            var target = CurFollowTarget;
            // 当前跟随目标为空
            if (target == default) {
                return;
            } else {
                if (target.isIntervalLook) {
                    SetIntervalWorldLocation(target);
                } else {
                    SetWorldLocation(target);
                }
            }

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

        /// <summary>
        /// 设置摄像机位置和转向
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        private void SetWorldLocation(CameraFollowAuthoring target) {
            // 跟随摄像机，一般摄像机
            var camera = CameraExtend.GetCamera(GameData.Camera.Main).transform;
            camera.position = Vector3.SmoothDamp(camera.position, target.GetPosition(), ref velocity, SmoothTime);
            camera.rotation = Quaternion.Slerp(camera.rotation, target.GetRotation(), SmoothTime * Time.deltaTime);
        }

        /// <summary>
        /// 设置摄像机短暂停留位置和转向
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private async void SetIntervalWorldLocation(CameraFollowAuthoring target) {
            float time = target.lookTime + SmoothTime;
            isBusying = true;
            while (time >= 0) {
                time -= Time.deltaTime;
                SetWorldLocation(target);
                await Task.Yield();
            }
            target.enabled = false;
            isBusying = false;
        }
    }
    #endregion
#endif
}