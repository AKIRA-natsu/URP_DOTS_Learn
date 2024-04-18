using System;
using UnityEngine;

namespace AKIRA.Behaviour.AI {
    /// <summary>
    /// 基类
    /// </summary>
    [SelectionBase]
    public abstract partial class AIBase : EntityBase, IMachineOwner {
        // 动画托管
        public CharacterAnimatorDelgateComponent AnimatorComponent { get; private set; }
        // 控制器驱动移动
        public UnityEngine.CharacterController Controller { get; private set; }

        // 状态机
        [field: SerializeReference]
        public IStateMachine StateMachine { get; protected set; }

        protected virtual void Awake() {
            Controller = this.GetComponent<UnityEngine.CharacterController>();
            AnimatorComponent = 
                this.GetComponentInChildren<Animator>().GetOrAddComponent<CharacterAnimatorDelgateComponent>();
        }

        protected virtual void Start() {
            StateMachine = new HierarchyFiniteStateMachine(this);
            RegistStates();
        }

        /// <summary>
        /// 注册状态机
        /// </summary>
        protected abstract void RegistStates();
    }

    public partial class CharacterAnimatorDelgateComponent : MonoBehaviour {
        // 动画
        private Animator animator;
        // 根运动事件
        public Action<Vector3, Quaternion> OnRootMotion;

        private void Awake() {
            animator = this.GetComponent<Animator>();
        }

        private void OnAnimatorMove() {
            OnRootMotion?.Invoke(animator.deltaPosition, animator.deltaRotation);
        }

        /// <summary>
        /// 切换动画
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        public void SwitchAnimation(string name, float time = .25f) {
            animator.CrossFadeInFixedTime(name, time);
        }

        /// <summary>
        /// 获得当前状态信息
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public AnimatorStateInfo GetAnimationInfo(int layer = 0) {
            return animator.GetCurrentAnimatorStateInfo(layer);
        }

        /// <summary>
        /// 设置动画参数值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetValue(string name, float value) {
            animator.SetFloat(name, value);
        }
    }
}