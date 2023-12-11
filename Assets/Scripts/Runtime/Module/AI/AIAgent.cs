using UnityEngine;
using UnityEngine.AI;

namespace AKIRA.Behaviour.AI {
    /// <summary>
    /// 状态
    /// </summary>
    public enum AIState {
        /// <summary>
        /// 空
        /// </summary>
        None,
        /// <summary>
        /// 待机
        /// </summary>
        Idle,
        /// <summary>
        /// 等待
        /// </summary>
        Wait,
        /// <summary>
        /// 移动
        /// </summary>
        Move,
        /// <summary>
        /// 跳跃
        /// </summary>
        Jump,
        /// <summary>
        /// 攀爬
        /// </summary>
        Climb,
        /// <summary>
        /// 准备攻击
        /// </summary>
        ReadyAttack,
        /// <summary>
        /// 攻击
        /// </summary>
        Attack,
        /// <summary>
        /// 死亡
        /// </summary>
        Die,
    }

    /// <summary>
    /// AI基类
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class AIAgent : AIBase, IPool {
        // state
        public AIState state { get; private set; } = AIState.None;
        // unityengine ai agent
        protected NavMeshAgent agent { get; private set; }
        // behaviour tree
        [SerializeField]
        public BehaviourTree tree;

        /// <summary>
        /// 到达目标点
        /// </summary>
        /// <value></value>
        public bool Reach {
            get {
                if (!agent)
                    return false;
                return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
            }
        }

        protected virtual void Awake() {
            agent = this.GetComponent<NavMeshAgent>();
            tree?.Bind(this);
        }

        // Pool function
        public abstract void Wake(object data = null);
        public abstract void Recycle(object data = null);

        public override void GameUpdate() {
            tree?.Update();
        }

        /// <summary>
        /// Swtich AI state
        /// </summary>
        /// <param name="state"></param>
        public void SwitchState(AIState state) {
            if (this.state == state)
                return;

            this.state = state;
            #if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
            #endif
            Animation?.SwitchAnima(state);
            "更新动画".Log();
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="destination"></param>
        public void SetDestination(Vector3 destination) {
            this.agent.destination = destination;
        }
    }
}