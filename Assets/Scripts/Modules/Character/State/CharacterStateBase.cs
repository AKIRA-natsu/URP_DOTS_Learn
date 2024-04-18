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