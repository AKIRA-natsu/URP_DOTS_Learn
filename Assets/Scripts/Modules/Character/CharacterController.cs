using System;
using Cinemachine;
using UnityEngine;

namespace AKIRA.Behaviour.AI {
    /// <summary>
    /// 玩家控制器
    /// </summary>
    public class CharacterController : AIBase {
        // motion config
        public readonly float Gravity = -9.8f;
        public readonly float RotateSpeed = 5f;
        public readonly float TransitionSpeed = 3f;
        public readonly float JumpPower = 1.2f;
        public readonly float MoveSpeedForJump = 3f;
        public readonly float JumpNeedHeight = 2.1f;          // 空中检测到地面距离，如果有这个距离播放翻滚
        public readonly float JumpEndHeight = 2f;             // jump end 动画播放的高度距离
        
        // free look target
        [SerializeField]
        private Transform lookPoint;

        protected override void Start() {
            base.Start();
            InitFreeCamera();
        }

        /// <summary>
        /// 初始化跟随摄像机
        /// </summary>
        private void InitFreeCamera() {
            var camera = CameraExtend.GetCamera(GameData.Camera.Main).GetComponent<CinemachineFreeLook>();
            camera.m_YAxis.m_InputAxisName = "Mouse ScrollWheel";
            camera.LookAt = lookPoint;
            camera.Follow = lookPoint;
        }

        protected override void RegistStates() {
            var hfStateMachine = this.StateMachine as IHFStateMachine;
            var subMoveMachine = hfStateMachine.RegistSubMachine<StateMachine>(StateKey.Move);
            subMoveMachine.RegistState<PlayerIdleState>();
            subMoveMachine.RegistState<PlayerMoveState>();
            subMoveMachine.RegistState<PlayerJumpState>();
            subMoveMachine.RegistState<PlayerAirDownState>();

            ResetMachine();
        }

        public void ResetMachine() {
            StateMachine.Switch<PlayerIdleState>();
        }
    }
}