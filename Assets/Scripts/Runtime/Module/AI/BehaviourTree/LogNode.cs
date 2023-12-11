using UnityEngine;

namespace AKIRA.Behaviour.AI {
    public class LogNode : ActionNode {
        // log message
        public string message;

        public override void OnCreate() {
            this.description = "message log";
        }

        protected override void OnStart()
        {
            Debug.Log($"OnStart: {message}");
        }

        protected override void OnStop()
        {
            Debug.Log($"OnStop: {message}");
        }

        protected override State OnUpdate()
        {
            Debug.Log($"OnUpdate: {message}");

            Debug.Log($"Blackboard: {blackboard.moveToPosition}");
            blackboard.moveToPosition.x += 1;

            return State.Success;
        }
    }
}