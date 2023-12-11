using UnityEngine;
using UnityEditor;
using AKIRA.Behaviour.AI;

public class WaypointManagerWindow : EditorWindow {
    public Transform waypointRoot;

    [MenuItem("Tools/AKIRA.Framework/Module/AI/WaypointManagerWindow")]
    private static void Open() {
        GetWindow<WaypointManagerWindow>();
    }

    private void OnGUI() {
        SerializedObject obj = new SerializedObject(this);

        EditorGUILayout.PropertyField(obj.FindProperty("waypointRoot"));

        if (waypointRoot == null) {
            EditorGUILayout.HelpBox("Root Transform must be selected. Please assign a root transform.", MessageType.Warning);
        } else {
            EditorGUILayout.BeginVertical("Box");
            DrawButtons();
            EditorGUILayout.EndVertical();
        }

        obj.ApplyModifiedProperties();
    }

    private void DrawButtons() {
        if (GUILayout.Button("Create Waypoint")) {
            CreateWaypoint();
        }

        if (Selection.activeGameObject != null && Selection.activeGameObject.TryGetComponent<Waypoint>(out Waypoint point)) {
            if (GUILayout.Button("Add Branch Waypoint")) {
                CreateBranch();
            }
            
            if (GUILayout.Button("Create Waypoint Before")) {
                CreateWaypointBefore(point);
            }
            if (GUILayout.Button("Create Waypoint After")) {
                CreateWaypointAfter(point);
            }
            if (GUILayout.Button("Remove Waypoint")) {
                RemoveWaypoint(point);
            }
        }

        if (waypointRoot.childCount >= 3) {
            if (GUILayout.Button("Link Waypoint")) {
                LinkWaypoint();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void CreateWaypoint() {
        GameObject waypointGo = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointGo.transform.SetParent(waypointRoot, false);

        Waypoint waypoint = waypointGo.GetComponent<Waypoint>();
        if (waypointRoot.childCount > 1) {
            waypoint.previousWaypoint = waypointRoot.GetChild(waypointRoot.childCount - 2).GetComponent<Waypoint>();
            waypoint.previousWaypoint.nextWaypoint = waypoint;
            //
            waypoint.transform.position = waypoint.previousWaypoint.transform.position;
            waypoint.transform.forward = waypoint.previousWaypoint.transform.forward;
        }

        Selection.activeGameObject = waypointGo;
    }

    /// <summary>
    /// 
    /// </summary>
    private void CreateWaypointBefore(Waypoint waypointSelected) {
        GameObject waypointGo = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointGo.transform.SetParent(waypointRoot, false);

        Waypoint waypoint = waypointGo.GetComponent<Waypoint>();

        waypoint.transform.position = waypointSelected.transform.position;
        waypoint.transform.forward = waypointSelected.transform.forward;

        var previousWaypoint = waypointSelected.previousWaypoint;
        if (previousWaypoint != null) {
            waypoint.previousWaypoint = previousWaypoint;
            previousWaypoint.nextWaypoint = waypoint;
        }
        waypoint.nextWaypoint = waypointSelected;
        waypointSelected.previousWaypoint = waypoint;
        waypoint.transform.SetSiblingIndex(waypointSelected.transform.GetSiblingIndex());

        Selection.activeGameObject = waypointGo;
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void CreateWaypointAfter(Waypoint waypointSelected) {
        GameObject waypointGo = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointGo.transform.SetParent(waypointRoot, false);

        Waypoint waypoint = waypointGo.GetComponent<Waypoint>();

        waypoint.transform.position = waypointSelected.transform.position;
        waypoint.transform.forward = waypointSelected.transform.forward;

        var nextWaypoint = waypointSelected.nextWaypoint;
        if (nextWaypoint != null) {
            waypoint.nextWaypoint = nextWaypoint;
            nextWaypoint.previousWaypoint = waypoint;
        }
        waypoint.previousWaypoint = waypointSelected;
        waypointSelected.nextWaypoint = waypoint;
        waypoint.transform.SetSiblingIndex(waypointSelected.transform.GetSiblingIndex() + 1);

        Selection.activeGameObject = waypointGo;
    }

    /// <summary>
    /// 
    /// </summary>
    private void RemoveWaypoint(Waypoint waypointSelected) {
        var previousWaypoint = waypointSelected.previousWaypoint;
        var nextWaypoint = waypointSelected.nextWaypoint;

        if (previousWaypoint != null) {
            previousWaypoint.nextWaypoint = nextWaypoint;
            Selection.activeGameObject = previousWaypoint.gameObject;
        }
        
        if (nextWaypoint != null) {
            nextWaypoint.previousWaypoint = previousWaypoint;
        }

        GameObject.DestroyImmediate(waypointSelected.gameObject);
    }

    /// <summary>
    /// 
    /// </summary>
    private void LinkWaypoint() {
        var firstWaypoint = waypointRoot.GetChild(0).GetComponent<Waypoint>();
        var lastWaypoint = waypointRoot.GetChild(waypointRoot.childCount - 1).GetComponent<Waypoint>();

        firstWaypoint.previousWaypoint = lastWaypoint;
        lastWaypoint.nextWaypoint = firstWaypoint;
    }

    /// <summary>
    /// 
    /// </summary>
    private void CreateBranch() {
        GameObject waypointGo = new GameObject("Waypoint" + waypointRoot.childCount, typeof(Waypoint));
        waypointGo.transform.SetParent(waypointRoot, false);

        Waypoint waypoint = waypointGo.GetComponent<Waypoint>();
        Waypoint branchedFrom = Selection.activeGameObject.GetComponent<Waypoint>();
        branchedFrom.branches.Add(waypoint);

        waypoint.transform.position = branchedFrom.transform.position;
        waypoint.transform.forward = branchedFrom.transform.forward;

        Selection.activeGameObject = waypointGo;
    }

}