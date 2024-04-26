using AKIRA.Manager;
using Cinemachine;
using UnityEngine;

namespace AKIRA.Behaviour.Camera {
    /// <summary>
    /// <para>第三人称摄像头控制器</para>
    /// <para>来源：UnityAsset  Third Person Controller StarterAssets</para>
    /// </summary>
    public class TPCameraController : MonoBehaviour, IUpdate {
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        private bool IsCurrentDeviceMouse
        {
            get
            {
// #if ENABLE_INPUT_SYSTEM
                return InputManager.Instance?.Input?.currentControlScheme == "Keyboard&Mouse";
// #else
// 				return false;
// #endif
            }
        }

        private const float _threshold = 0.01f;

        private void Start() {
            CameraExtend.GetCamera(GameData.Camera.Main).GetComponent<CinemachineVirtualCamera>().Follow = CinemachineCameraTarget.transform;
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            this.Regist(GameData.Group.Default, UpdateMode.LateUpdate);
        }

        public void OnUpdate() {
            CameraRotation();
        }

        private void CameraRotation() {
// #if ENABLE_INPUT_SYSTEM
            var lookPosition = InputManager.Instance?.GetInputAction(InputActions.Look)?.ReadValue<Vector2>() ?? Vector2.zero;
// #else
//             // UI 虚拟摇杆
// #endif
            // if there is an input and camera position is not fixed
            if (lookPosition.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                // 仔细看InputAction中，遥感的部分输入缩放了几百，键鼠部分缩放了几百分之一
                // 个人认为应该是缩小ReadValue读到的值
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += lookPosition.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += lookPosition.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, 
                _cinemachineTargetYaw, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax) {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}