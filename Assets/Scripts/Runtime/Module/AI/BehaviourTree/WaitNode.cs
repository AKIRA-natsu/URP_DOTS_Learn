using UnityEngine;

namespace AKIRA.Behaviour.AI {
    public class WaitNode : ActionNode {
        public float duration = 1f;
        private float startTime;
        
        public override void OnCreate() {
            this.description = $"Delay duration node(default 1)";
        }


        protected override void OnStart() {
            startTime = Time.time;
        }

        protected override void OnStop() { }

        protected override State OnUpdate() {
            if (Time.time - startTime > duration)
                return State.Success;
            else
                return State.Running;
        }
    }
}