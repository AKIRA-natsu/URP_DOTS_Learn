using UnityEngine;

namespace AKIRA.Behaviour.AI {
    public class FindTargetNode : ActionNode {
        public LayerMask mask;
        public float radius;

        public override void OnCreate()
        {
            this.description = "find target with mask";
        }

        protected override void OnStart() {}

        protected override void OnStop() {}

        protected override State OnUpdate() {
            var colliders = Physics.OverlapSphere(ai.transform.position, radius, mask);
            return colliders.Length != 0 ? Node.State.Success : Node.State.Failure;
        }
    }
}