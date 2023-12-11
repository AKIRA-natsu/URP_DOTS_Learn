using UnityEngine;
using UnityEditor;
using AKIRA.Behaviour.AI;

/// <summary>
/// Waypoint 绘制
/// </summary>
[InitializeOnLoad()]
public class WaypointEditor {
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(Waypoint waypoint, GizmoType gizmoType) {
        float alpha = (gizmoType & GizmoType.NonSelected) != 0 ? 0.25f : 1f;
    
        Color color = Color.yellow;  
        color.a = alpha;  
    
        Gizmos.color = color;  
        Gizmos.DrawSphere(waypoint.transform.position, 0.5f);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(waypoint.transform.position + (waypoint.transform.right * waypoint.width / 2),
                        waypoint.transform.position - (waypoint.transform.right * waypoint.width / 2));
        
        if (waypoint.previousWaypoint != null) {
            Gizmos.color = Color.red;
            Vector3 offset = waypoint.transform.right * waypoint.width / 2f;
            Vector3 offsetTo = waypoint.previousWaypoint.transform.right * waypoint.previousWaypoint.width / 2f;

            Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.previousWaypoint.transform.position + offsetTo);
        }

        if (waypoint.nextWaypoint != null) {
            Gizmos.color = Color.green;
            Vector3 offset = waypoint.transform.right * -waypoint.width / 2f;
            Vector3 offsetTo = waypoint.nextWaypoint.transform.right * -waypoint.nextWaypoint.width / 2f;

            Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.nextWaypoint.transform.position + offsetTo);
        }

        if (waypoint.branches != null) {
            foreach (var branch in waypoint.branches) {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(waypoint.transform.position, branch.transform.position);
            }
        }
    }
}