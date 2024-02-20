using UnityEngine;

namespace AKIRA.Behaviour.AI {
    /// <summary>
    /// <para>手部IK</para>
    /// </summary>
    public class HandIKComponent : IComponentData {
        // ik behaivour
        private IKBehaviour behaviour;

        public HandIKComponent(IKBehaviour behaviour) {
            this.behaviour = behaviour;
            behaviour.RegistIKAction(OnAdjustHand);
        }

        private void OnAdjustHand(Animator animator, int arg2)
        {
            // 调整双手位置，否则双手穿模到裙子
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        }

        public void Dispose() {
            this.behaviour.RemoveIKAction(OnAdjustHand);
        }
    

    }
}