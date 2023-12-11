using System.Collections.Generic;
using UnityEngine;

namespace AKIRA.Behaviour.AI {
    public class Waypoint : MonoBehaviour {
        /// <summary>
        /// 當人物從當前 Waypoint 的下個 Waypoint 方向走來，上個 Waypoint 即為人物接續的目標點。
        /// </summary>
        public Waypoint previousWaypoint;
        /// <summary>
        /// 當人物從當前 Waypoint 的上個 Waypoint 方向走來，下個 Waypoint 即為人物接續的目標點。
        /// </summary>
        public Waypoint nextWaypoint;

        /// <summary>
        /// 宽度
        /// </summary>
        [Range(0f, 5f)]
        public float width = 1f;

        /// <summary>
        /// 分支
        /// </summary>
        public List<Waypoint> branches;

        [Range(0f, 1f)]
        public float branchRatio = 0.5f;

        public Vector3 GetPosition() {
            Vector3 minBound = transform.position + transform.right * width / 2;
            Vector3 maxBound = transform.position - transform.right * width / 2;

            return Vector3.Lerp(minBound, maxBound, Random.Range(0f, 1f));
        }
    }
}