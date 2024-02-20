using System;
using UnityEngine;

namespace AKIRA.Behaviour.AI {
    /// <summary>
    /// 委托表现
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class IKBehaviour : MonoBehaviour {
        // Animator
        private Animator animator;
        public Animator Animator => animator;
        // IK Event
        private Action<Animator, int> onAnimatorIK;

        private void Awake() {
            animator = this.GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex) {
            onAnimatorIK?.Invoke(animator, layerIndex);
        }

        // regist animator ik callback
        public void RegistIKAction(Action<Animator, int> onAnimatorIK) => this.onAnimatorIK += onAnimatorIK;
        // remove animator ik callback
        public void RemoveIKAction(Action<Animator, int> onAnimatorIK) => this.onAnimatorIK -= onAnimatorIK;
    }
}