using UnityEngine;

namespace AKIRA.Behaviour.AI {
    public abstract class Node : ScriptableObject {
        /// <summary>
        /// Node State
        /// </summary>
        public enum State {
            Running,
            Failure,
            Success,
        }

        /// <summary>
        /// node state
        /// </summary>
        [HideInInspector]
        public State state = State.Running;
        /// <summary>
        /// is started
        /// </summary>
        [HideInInspector]
        public bool started = false;
        /// <summary>
        /// id
        /// </summary>
        [HideInInspector]
        public string guid = default;
        /// <summary>
        /// view position
        /// </summary>
        [HideInInspector]
        public Vector2 position = default;
        /// <summary>
        /// 
        /// </summary>
        [HideInInspector]
        public Blackboard blackboard = new Blackboard();
        /// <summary>
        /// AI
        /// </summary>
        [HideInInspector]
        public AIAgent ai;
        /// <summary>
        /// 描述
        /// </summary>
        [TextArea]
        public string description;


        public State Update() {
            if (!started) {
                started = true;
                OnStart();
            }

            state = OnUpdate();

            if (state == State.Failure || state == State.Success) {
                started = false;
                OnStop();
            }

            return state;
        }

        public virtual Node Clone()  {
            return Instantiate(this);
        }

        // abstract functions
        public abstract void OnCreate();
        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();
    }
}