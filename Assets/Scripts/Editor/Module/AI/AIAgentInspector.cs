using UnityEngine;
using UnityEditor;
using AKIRA.Behaviour.AI;
using AKIRA;

[CustomEditor(typeof(AIAgent), true)]
public class AIAgentInspector : Editor {
    public override void OnInspectorGUI() {
        // base.OnInspectorGUI();
        var agent = target as AIAgent;
        SerializedObject agentObject = new SerializedObject(agent);
        SerializedProperty modeProperty = agentObject.FindProperty("mode");
        SerializedProperty stateProperty = agentObject.FindProperty("state");

        EditorGUILayout.BeginVertical("framebox");
        if (Application.isPlaying) {
            var mode = (UpdateMode)modeProperty.enumValueIndex;
            var isUpdating = agent.IsUpdating(GameData.Group.Animator, mode);
            if (isUpdating) {
                GUI.color = Color.green;
                EditorGUILayout.LabelField("Updating...");
            } else {
                GUI.color = Color.red;
                EditorGUILayout.LabelField("Stoping...");
            }
            GUI.color = Color.white;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Begin Update"))
                agent.Regist(GameData.Group.Animator, mode);
            if (GUILayout.Button("Stop Update"))
                agent.Remove(GameData.Group.Animator, mode);
            EditorGUILayout.EndHorizontal();
        } else {
            EditorGUILayout.LabelField("Editor下面板更新操作（游戏未开启）");
        }
        EditorGUILayout.EndVertical();


        EditorGUILayout.BeginVertical("framebox");

        GUI.enabled = !Application.isPlaying;
        EditorGUILayout.PropertyField(modeProperty);

        EditorGUI.BeginChangeCheck();
        var curState = (AIState)stateProperty.enumValueIndex;
        GUI.enabled = Application.isPlaying;
        AIState tempState = (AIState)EditorGUILayout.EnumPopup("State", curState);
        if (EditorGUI.EndChangeCheck() && Application.isPlaying) {
            if (!tempState.Equals(curState)) {
                agent.SwitchState(tempState);
                $"{agent} Log: inspector to switch state => {tempState}".Log(GameData.Log.Editor);
            }
        }

        GUI.enabled = !Application.isPlaying;
        agent.tree = (BehaviourTree)EditorGUILayout.ObjectField("Behaviour Tree", agent.tree, typeof(BehaviourTree), false);
        GUI.enabled = true;
        
        agentObject.ApplyModifiedProperties();
        EditorGUILayout.EndVertical();
    }
}