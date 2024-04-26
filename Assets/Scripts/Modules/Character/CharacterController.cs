using System;
using UnityEngine;

namespace AKIRA.Behaviour.AI {
    /// <summary>
    /// 玩家控制器
    /// </summary>
    public class CharacterController : AIBase {
        // motion config
        public readonly float Gravity = -9.8f;
        public readonly float MoveSpeed = 2f;
        public readonly float RunSpeed = 4f;
        public readonly float RotateSpeed = 5f;
        public readonly float TransitionSpeed = 3f;
        public readonly float JumpPower = 1.2f;
        public readonly float MoveSpeedForJump = 2f;
        public readonly float JumpNeedHeight = 2.1f;          // 空中检测到地面距离，如果有这个距离播放翻滚
        public readonly float JumpEndHeight = 2f;             // jump end 动画播放的高度距离

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