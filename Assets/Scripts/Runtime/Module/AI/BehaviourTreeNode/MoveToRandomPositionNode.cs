using UnityEngine;

namespace AKIRA.Behaviour.AI {
    public class MoveToRandomPositionNode : ActionNode {
        // random radius
        public float radius;
        // if reset start position
        public bool resetStartPosition;
        // origin position
        private Vector3 originPosition;
        // target position
        private Vector3 targetPosition;

        public override void OnCreate() {
            this.description = "Move to random position in radius.\n if reset start position\n start position always renew when reach target position";
        }

        protected override void OnStart() {
            var curPosition = ai.transform.position;
            var randomPosition = Random.insideUnitSphere * radius;
            randomPosition.y = curPosition.y;
            if (resetStartPosition) {
                targetPosition = curPosition + randomPosition;
            } else {
                if (originPosition.Equals(default)) {
                    originPosition = curPosition;
                }
                targetPosition = originPosition + randomPosition;
            }

            ai.SwitchState(AIState.Move);
            ai.SetDestination(targetPosition);
        }

        protected override void OnStop() {
            ai.SwitchState(AIState.Idle);
        }

        protected override State OnUpdate() {
            return ai.Reach ? Node.State.Success : Node.State.Running;
        }
    }
}