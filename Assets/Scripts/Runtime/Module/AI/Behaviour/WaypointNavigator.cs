using UnityEngine;

namespace AKIRA.Behaviour.AI {
    [RequireComponent(typeof(AIAgent))]
    public class WaypointNavigator : MonoBehaviour, IUpdate {
        private AIAgent agent;
        public Waypoint currentWaypoint;

        private int direction;

        private void Awake() {
            // controller = this.GetComponent<CharacterNavigationController>();
            agent = this.GetComponent<AIAgent>();
        }

        private void Start() {
            direction = Mathf.RoundToInt(Random.Range(0f, 1f));
            agent.SetDestination(currentWaypoint.GetPosition());
            this.Regist();
        }

        private void OnDisable() {
            this.Remove();
        }

        public void GameUpdate() {
            if (agent.Reach) {
                bool shouldBranch = false;

                if (currentWaypoint.branches != null && currentWaypoint.branches.Count > 0) {
                    shouldBranch = Random.Range(0f, 1f) <= currentWaypoint.branchRatio;
                }

                if (shouldBranch) {
                    currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count - 1)];
                } else {
                    if (direction == 0) {
                        if (currentWaypoint.nextWaypoint != null) {
                            currentWaypoint = currentWaypoint.nextWaypoint;
                        } else {
                            currentWaypoint = currentWaypoint.previousWaypoint;
                            direction = 1;
                        }
                    } else if (direction == 1) {
                        if (currentWaypoint.previousWaypoint != null) {
                            currentWaypoint = currentWaypoint.previousWaypoint;
                        } else {
                            currentWaypoint = currentWaypoint.nextWaypoint;
                            direction = 0;
                        }
                    }
                }

                agent.SetDestination(currentWaypoint.GetPosition());
            }
        }
    }
}