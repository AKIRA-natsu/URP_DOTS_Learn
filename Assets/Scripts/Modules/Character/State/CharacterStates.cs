using System;
using UnityEngine;

namespace AKIRA.Behaviour.AI {
    using Camera = UnityEngine.Camera;

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

            // 检测下落
            if (!Owner.Controller.isGrounded) {
                SwitchState<PlayerAirDownState>();
                return;
            }

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

            // 检测下落
            if (!Owner.Controller.isGrounded) {
                SwitchState<PlayerAirDownState>();
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
                if (walk2RunTransition > .4f)
                    SwitchMoveState(MoveChildState.Stop);
                else
                    SwitchState<PlayerIdleState>();
                return;
            }

            var self = Owner.transform;

            // 处理跑步
            bool isRun = GetInputAction(InputActions.Run).IsPressed();
            walk2RunTransition = Mathf.Clamp(walk2RunTransition + (isRun ? 1 : -1) * Time.deltaTime * Player.TransitionSpeed, 0, 1);
            SetAnimationValue(Animation.Move, walk2RunTransition);

            // 处理旋转
            var input = action.ReadValue<Vector2>();
            // 四元数 x 向量：向量按照四元数旋转得到新的向量
            var moveDir = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * new Vector3(input.x, 0, input.y);
            self.rotation = Quaternion.Slerp(self.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * Player.RotateSpeed);

            // move the player
            var speed = Mathf.Lerp(Player.MoveSpeed, Player.RunSpeed, walk2RunTransition);
            Owner.Controller.Move(self.forward * (speed * Time.deltaTime) +
                             new Vector3(0.0f, Player.Gravity, 0.0f) * Time.deltaTime);
        }

        private void StopOnUpdate() {
            // 检测当前播放进度
            if (IsAnimationName(Animation.MoveToStop, out float time)) {
                if (time >= 1)
                    SwitchState<PlayerMoveState>();
                
                if (GetInputAction(InputActions.Move).WasPerformedThisFrame())
                    SwitchMoveState(MoveChildState.Move);
            }
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
                break;
                case MoveChildState.Stop:
                    SwitchAnimation(Animation.MoveToStop);
                break;
            }
        }
    }

    // 玩家跳跃状态
    public class PlayerJumpState : PlayerStateBase {
        public override void OnEnter() {
            // base.OnEnter();
            // SwitchAnimation(Animation.JumpStart);
            // SetRootMotion(RootMotionMove);
        }

        public override void OnExit() {
            // base.OnExit();
            // SetRootMotion(null);
        }

        public override void OnUpdate() {
            if (IsAnimationName(Animation.JumpStart, out float time)) {
                if (time >= .95f) {
                    SwitchState<PlayerAirDownState>();
                }
            }
        }

        private void RootMotionMove(Vector3 vector, Quaternion quaternion) {
            vector.y *= Player.JumpPower;
            Owner.Controller.Move(vector);
        }
    }

    // 玩家空中掉落状态
    public class PlayerAirDownState : PlayerStateBase {
        private LayerMask envMask = 1 << Layer.Environment;     // 检测对象层级
        private JumpChildState state;                           // 跳跃子状态
        private bool needEndAnimation;                          // 是否需要播放跳跃结束动画

        public override void OnEnter() {
            base.OnEnter();
            SwitchJumpState(JumpChildState.Loop);
            // 判断当前角色高度是否有可能切换到End
            var trans = Player.transform;
            needEndAnimation = !Physics.Raycast(trans.position + Vector3.up * .5f, -trans.up, Player.JumpNeedHeight, envMask);
        }

        public override void OnUpdate() {
            switch (state) {
                case JumpChildState.Loop:
                    var trans = Player.transform;
                    if (needEndAnimation) {
                        if (Physics.Raycast(trans.position + Vector3.up * .5f, -trans.up, Player.JumpEndHeight, envMask)) {
                            SwitchJumpState(JumpChildState.End);
                        }
                    } else {
                        if (Owner.Controller.isGrounded) {
                            SwitchState<PlayerIdleState>();
                            return;
                        }
                    }
                    OnAirControl();
                break;
                case JumpChildState.End:
                    if (IsAnimationName(Animation.JumpEnd, out float time)) {
                        if (time >= .8f) {
                            if (Owner.Controller.isGrounded == false) {
                                // 踩空，继续往下掉落
                                SwitchJumpState(JumpChildState.Loop);
                            } else {
                                SwitchState<PlayerIdleState>();
                            }
                        } else if (time < .6f) {
                            OnAirControl();
                        }
                    }
                break;
            }
        }

        private void SwitchJumpState(JumpChildState state) {
            this.state = state;
            switch (state) {
                case JumpChildState.Loop:
                SwitchAnimation(Animation.JumpLoop);
                break;
                case JumpChildState.End:
                SwitchAnimation(Animation.JumpEnd);
                break;
            }
        }

        /// <summary>
        /// 空中控制
        /// </summary>
        private void OnAirControl() {
            var action = GetInputAction(InputActions.Move);

            var deltaTime = Time.deltaTime;
            Vector3 motion = new (0, Player.Gravity * deltaTime, 0);
            if (action.IsPressed()) {
                var input = action.ReadValue<Vector2>();
                var self = Owner.transform;
                var camera = Camera.main.transform;
                var direction = new Vector3(input.x, 0, input.y);
                var dir = camera.TransformDirection(direction);
                motion.x = Player.MoveSpeedForJump * deltaTime * dir.x;
                motion.z = Player.MoveSpeedForJump * deltaTime * dir.z;

                // 处理旋转
                // 四元数 x 向量：向量按照四元数旋转得到新的向量
                var moveDir = Quaternion.Euler(0, camera.eulerAngles.y, 0) * direction;
                self.rotation = Quaternion.Slerp(self.rotation, Quaternion.LookRotation(moveDir), deltaTime * Player.RotateSpeed);
            }

            // 处理移动
            Owner.Controller.Move(motion);
        }
    }

}