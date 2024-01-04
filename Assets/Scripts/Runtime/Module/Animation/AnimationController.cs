using AKIRA.Behaviour.AI;
using UnityEngine;

namespace AKIRA.Behaviour.Aniamtion {
    /// <summary>
    /// 动画控制器
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimationController : MonoBehaviour {
        protected Animator animator { get; private set; }

        protected virtual void Awake() {
            animator = this.GetComponent<Animator>();
        }

        /// <summary>
        /// change animator play speed
        /// </summary>
        /// <param name="v"></param>
        public void ChangeAnimationSpeed(float v) => animator.speed = v;

        public void SwitchAnima(AIState state, object data = null) { }
    }
}