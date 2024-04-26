using System;
using UnityEngine;

namespace AKIRA.Behaviour.AI {
    /// <summary>
    /// 人物基类
    /// </summary>
    [SelectionBase]
    [RequireComponent(typeof(UnityEngine.CharacterController))]
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
        // ik运动事件
        public Action<int, Animator> OnIKMotion;

        private void Awake() {
            animator = this.GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex) {
            OnIKMotion?.Invoke(layerIndex, animator);
        }

        /// <summary>
        /// 切换动画
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        public void SwitchAnimation(string name, float time = .25f, int layer = 0) {
            animator.CrossFadeInFixedTime(name, time, layer);
        }

        /// <summary>
        /// 更新Animator Layer Weight
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="weight"></param>
        public void SetLayerWeight(int layer, float weight) {
            animator.SetLayerWeight(layer, weight);
        }

        /// <summary>
        /// 获得当前状态信息
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public AnimatorStateInfo GetAnimationInfo(int layer = 0) {
            return animator.GetCurrentAnimatorStateInfo(layer);
        }

        public void SetValue(string name, float value) => animator.SetFloat(name, value);
        public void SetValue(string name, bool value) => animator.SetBool(name, value);
    }
}