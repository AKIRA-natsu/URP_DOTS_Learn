using System;
using UnityEngine;

namespace AKIRA.Behaviour.AI {
    // 玩家待机状态
    public class PlayerIdleState : PlayerStateBase {
        public override void OnEnter() {
            base.OnEnter();
            SwitchAnimation(Animation.Idle);
        }

        public override void OnUpdate() {
            // 检测跳跃
            if (GetInputAction(InputActions.Jump).WasPerformedThisFrame()) {
                SwitchState<PlayerJumpState>();
                return;
            }

            // 检测攻击

            // 检测移动
            Owner.Controller.Move(new(0, Player.Gravity * Time.deltaTime, 0));
            if (GetInputAction(InputActions.Move).IsPressed()) {
                SwitchState<PlayerMoveState>();
                return;
            }
        }
    }

    // 玩家移动状态
    public class PlayerMoveState : PlayerStateBase {
        private float walk2RunTransition;       // 0 - 1
        private MoveChildState moveState;

        public override void OnEnter() {
            base.OnEnter();
            SwitchMoveState(MoveChildState.Move);
        }

        public override void OnExit() {
            base.OnExit();
            walk2RunTransition = 0;
        }

        public override void OnUpdate() {
            // 检测跳跃
            if (GetInputAction(InputActions.Jump).WasPerformedThisFrame()) {
                SwitchState<PlayerJumpState>();
                return;
            }

            switch (moveState) {
                case MoveChildState.Move:
                    MoveOnUpdate();
                break;
                case MoveChildState.Stop:
                    StopOnUpdate();
                break;
            }
        }

        private void MoveOnUpdate() {
            var action = GetInputAction(InputActions.Move);
            if (action.WasReleasedThisFrame()) {
                SwitchState<PlayerIdleState>();
                return;
            }

            var deltaTime = Time.deltaTime;
            var self = Owner.transform;

            // 处理跑步
            bool isRun = GetInputAction(InputActions.Run).IsPressed();
            walk2RunTransition = Mathf.Clamp(walk2RunTransition + (isRun ? deltaTime : -deltaTime) * Player.TransitionSpeed, 0, 1);
            SetAnimationValue(Animation.Move, walk2RunTransition);

            // 处理旋转
            var input = action.ReadValue<Vector2>();
            // 四元数 x 向量：向量按照四元数旋转得到新的向量
            var moveDir = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * new Vector3(input.x, 0, input.y);
            self.rotation = Quaternion.Slerp(self.rotation, Quaternion.LookRotation(moveDir), deltaTime * Player.RotateSpeed);
        }

        private void StopOnUpdate() {

        }

        /// <summary>
        /// 切换移动状态
        /// </summary>
        /// <param name="state"></param>
        private void SwitchMoveState(MoveChildState state) {
            moveState = state;
            switch (state) {
                case MoveChildState.Move:
                    SwitchAnimation(Animation.Move);
                    SetRootMotion(RootMotionMove);
                break;
                case MoveChildState.Stop:
                    SetRootMotion(null);
                break;
            }
        }

        private void RootMotionMove(Vector3 vector, Quaternion quaternion) {
            vector.y = Time.deltaTime * Player.Gravity;
            Owner.Controller.Move(vector);
        }
    }

    // 玩家跳跃状态
    public class PlayerJumpState : PlayerStateBase {
        public override void OnEnter() {
            base.OnEnter();
            SwitchAnimation(Animation.Jump);
            SetRootMotion(RootMotionMove);
        }

        public override void OnExit() {
            base.OnExit();
            SetRootMotion(null);
        }

        public override void OnUpdate() {
            var info = Owner.AnimatorComponent.GetAnimationInfo();
            if (info.IsName(Animation.Jump)) {

                var action = GetInputAction(InputActions.Move);
                if (action.IsPressed()) {
                    var input = action.ReadValue<Vector2>();
                    var self = Owner.transform;
                    var deltaTime = Time.deltaTime;
                    var camera = Camera.main.transform;
                    var direction = new Vector3(input.x, 0, input.y);
                    var dir = camera.TransformDirection(direction);

                    // 处理移动
                    Owner.Controller.Move(Player.MoveSpeedForJump * deltaTime * dir);

                    // 处理旋转
                    // 四元数 x 向量：向量按照四元数旋转得到新的向量
                    var moveDir = Quaternion.Euler(0, camera.eulerAngles.y, 0) * direction;
                    self.rotation = Quaternion.Slerp(self.rotation, Quaternion.LookRotation(moveDir), deltaTime * Player.RotateSpeed);
                }

                // 攻击检测

                if (info.normalizedTime >= .95f) {
                    SwitchState<PlayerIdleState>();
                }
            }
        }

        private void RootMotionMove(Vector3 vector, Quaternion quaternion) {
            vector.y *= Player.JumpPower;
            Owner.Controller.Move(vector);
        }
    }
}