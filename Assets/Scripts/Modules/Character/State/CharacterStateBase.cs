using System;
using AKIRA.Manager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AKIRA.Behaviour.AI {
    // character state base
    public abstract partial class CharacterStateBase : IState, IUpdate {
        // state owner
        protected AIBase Owner => (AIBase)Machine.Owner;

        public IStateMachine Machine { get; set; }

        public virtual void Dispose() { }

        public virtual void OnEnter() => this.Regist(GameData.Group.Animator);
        public virtual void OnExit() => this.Remove(GameData.Group.Animator);

        public abstract void OnUpdate();

        // switch animator state
        protected void SwitchAnimation(string name, float time = .25f) {
            Owner.AnimatorComponent.SwitchAnimation(name, time);
        }

        // set animator param float value
        protected void SetAnimationValue(string name, float value) {
            Owner.AnimatorComponent.SetValue(name, value);
        }

        // set animator root motion
        protected void SetRootMotion(Action<Vector3, Quaternion> onRootMotion) {
            Owner.AnimatorComponent.OnRootMotion = onRootMotion;
        }

        /// <summary>
        /// 判断当前动画状态，并获得当前动画进度
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time">0 - 1</param>
        /// <param name="layer"></param>
        /// <returns></returns>
        protected bool IsAnimationName(string name, out float time, int layer = 0) {
            var info = Owner.AnimatorComponent.GetAnimationInfo(layer);
            time = info.normalizedTime;
            return info.IsName(name);
        }

        // switch state machine
        protected void SwitchState<T>() where T : IState {
            Machine.Switch<T>();
        }
    }

    public partial class CharacterStateBase {
        // 移动子类型，给移动停止到待机一个过渡
        public enum MoveChildState {
            Stop,
            Move,
        }

        public enum JumpChildState {
            Loop,
            End,
        }
    }
    
    // player state base
    public abstract class PlayerStateBase : CharacterStateBase {
        // 玩家
        protected CharacterController Player => Owner as CharacterController;

        // get input action
        protected InputAction GetInputAction(string name) {
            return InputManager.Instance.GetInputAction(name);
        }
    }
}