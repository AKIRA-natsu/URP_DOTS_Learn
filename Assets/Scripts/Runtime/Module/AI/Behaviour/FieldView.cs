using UnityEngine;

namespace AKIRA.Behaviour.AI {
    /// <summary>
    /// 视野
    /// </summary>
    public class FieldView : MonoBehaviour {
        // 视野距离
        public float viewRadius = 5f;
        // 视野范围
        public float viewAngle = 90f;
        // 射线数量
        public int viewAngleStep = 30;

        // 遮挡层级
        public LayerMask mask;
        // 是否已经在editor绘制
        public bool drawGizmosAlways = false;

        /// <summary>
        /// 视野内是否有目标
        /// </summary>
        /// <param name="hit"></param>
        /// <returns></returns>
        public bool ViewRayHit(out RaycastHit hit) {
            hit = default;

            // 最左边的那条射线
            Vector3 forward_left = Quaternion.Euler(0f, -viewAngle / 2, 0f) * transform.forward * viewRadius;
            // 处理射线
            for (int i = 0; i < viewAngleStep; i++) {
                // 每条射线都在forward_left的基础上偏转一点，最后一个正好偏转90度到视线最右侧
                Vector3 v = Quaternion.Euler(0f, (viewAngle / viewAngleStep) * i, 0f) * forward_left;
                
                // 创建射线
                if (Physics.Raycast(transform.position, v, out hit, viewRadius, mask)) {
                    return true;
                }
            }
            return false;
        }
    }
}