using UnityEngine;
using UnityEditor;
using AKIRA.Behaviour.AI;
using AKIRA;

[CustomEditor(typeof(AIAgent), true)]
public class AIAgentInspector : Editor {
    private SerializedProperty modeProperty;

    public override void OnInspectorGUI() {
        // base.OnInspectorGUI();
        var agent = target as AIAgent;
        SerializedObject agentObject = new SerializedObject(agent);

        EditorGUILayout.BeginVertical("framebox");

        modeProperty = agentObject.FindProperty("mode");
        EditorGUILayout.PropertyField(modeProperty);

        AIState tempState = (AIState)EditorGUILayout.EnumPopup("state", agent.state);
        if (!tempState.Equals(agent.state)) {
            agent.SwitchState(tempState);
            $"{agent} Log: inspector to switch state => {tempState}".Log(GameData.Log.Editor);
        }

        agent.tree = (BehaviourTree)EditorGUILayout.ObjectField("Behaviour Tree", agent.tree, typeof(BehaviourTree), false);
        
        EditorGUILayout.EndVertical();
    }
}