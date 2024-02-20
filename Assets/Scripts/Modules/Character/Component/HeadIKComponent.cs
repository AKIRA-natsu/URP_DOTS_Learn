using UnityEngine;

namespace AKIRA.Behaviour.AI {
    /// <summary>
    /// <para>头部IK，朝向看</para>
    /// <para>TODO: https://nekojara.city/unity-look-at</para>
    /// </summary>
    public class HeadIKComponent : IComponentData {
        // ik behaivour
        private IKBehaviour behaviour;

        // look position
        private Vector3 position;
        // look weight
        private float weight;

        // lerp speed
        private const float LookSpeed = 7f;

        public HeadIKComponent(IKBehaviour behaviour) {
            this.behaviour = behaviour;
            behaviour.RegistIKAction(OnHeadIK);
        }

        private void OnHeadIK(Animator animator, int layer) {
            animator.SetLookAtPosition(position);
            animator.SetLookAtWeight(weight);
        }

        public void SetLookTarget(Transform target) {
            if (target == null) {
                this.weight = Mathf.Lerp(this.weight, 0f, Time.deltaTime * LookSpeed);
            } else {
                // adjust y axis
                var localTrans = this.behaviour.transform;
                var selfPosition = localTrans.position;
                var targetPosition = target.position;
                targetPosition.y = selfPosition.y;
                var angle = Vector3.Angle(localTrans.forward, (targetPosition - selfPosition).normalized);
                // limit angle
                if (angle > 60f && angle < 300f) {
                    this.weight = Mathf.Lerp(this.weight, 0f, Time.deltaTime * LookSpeed);
                } else {
                    this.position = Vector3.Lerp(position, target.position, Time.deltaTime * LookSpeed);
                    this.weight = Mathf.Lerp(this.weight, 1f, Time.deltaTime * LookSpeed);
                }
            }
        }

        public void Dispose() {
            this.behaviour.RemoveIKAction(OnHeadIK);
        }
    }
}