using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
#endif

namespace AKIRA.Behaviour.Camera {
    /// <summary>
    /// 摄像机拖拽表现
    /// </summary>
    public class CameraDragBehaviour : CameraBehaviour {
        private IDrag dragObject;
        /// <summary>
        /// 当前拖拽物体
        /// </summary>
        public IDrag CurrentDrag => dragObject;

#if UNITY_ANDROID || UNITY_IOS
        protected override void Awake() {
            base.Awake();
            EnhancedTouchSupport.Enable();
        }
#endif

        public override void GameUpdate() {
#if UNITY_EDITOR
            if (Mouse.current.leftButton.wasReleasedThisFrame && dragObject != null)
#else
            if (Touch.activeTouches.Count == 0 && dragObject != null)
#endif
            {
                dragObject.OnDragUp();
                dragObject = null;
            }

#if UNITY_EDITOR
            if (Mouse.current.leftButton.wasPressedThisFrame && dragObject == null)
#else
            if (Touch.activeTouches.Count > 0 && dragObject == null)
#endif
            {
#if UNITY_EDITOR
                Ray ray = CameraExtend.MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
#else
                Ray ray = CameraExtend.MainCamera.ScreenPointToRay(Touch.activeTouches[0].screenPosition);
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
}