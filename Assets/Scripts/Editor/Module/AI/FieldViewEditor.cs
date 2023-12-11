using UnityEngine;
using UnityEditor;
using AKIRA.Behaviour.AI;

/// <summary>
/// Waypoint 绘制
/// </summary>
[InitializeOnLoad()]
public class FieldViewEditor {
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(FieldView view, GizmoType gizmoType) {
        if (view.drawGizmosAlways)
            Gizmos.color = Color.red;
        else
            Gizmos.color = (gizmoType & GizmoType.NonSelected) != 0 ? Color.clear : Color.red;
        
        DrawView(view);
    }

    /// <summary>
    /// 绘制检测扇形区域
    /// </summary>
    /// <param name="view"></param>
    private static void DrawView(FieldView view) {
        // 最左边的那条射线
        Vector3 forward_left = Quaternion.Euler(0f, -view.viewAngle / 2, 0f) * view.transform.forward * view.viewRadius;
        // 处理射线
        for (int i = 0; i < view.viewAngleStep; i++) {
            // 每条射线都在forward_left的基础上偏转一点，最后一个正好偏转90度到视线最右侧
            Vector3 v = Quaternion.Euler(0f, (view.viewAngle / view.viewAngleStep) * i, 0f) * forward_left;
            // player + v == 射线pos
            Vector3 pos = view.transform.position + v;

            // 创建射线
            RaycastHit hit;
            if (Physics.Raycast(view.transform.position, v, out hit, view.viewRadius, view.mask)) {
                // 照射到碰撞物，修改绘制的终点
                pos = hit.point;
            }

            Gizmos.DrawLine(view.transform.position, pos);
        }
    }
}