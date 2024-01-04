using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ExcelDataConfig))]
public class ExcelDataConfigInspector : Editor {
    private bool folder;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var config = target as ExcelDataConfig;
        
        EditorGUILayout.BeginVertical("framebox");
        DrawFolderField("Excel", serializedObject.FindProperty("excelPath"));
        DrawFolderField("Script", serializedObject.FindProperty("scriptPath"));
        DrawFolderField("Output", serializedObject.FindProperty("output"));

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("With encrypt will output bytes, else output json", MessageType.Info);

        EditorGUI.BeginChangeCheck();        
        var encryptProperty = serializedObject.FindProperty("encrypt");
        EditorGUILayout.PropertyField(encryptProperty);
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();

        if (encryptProperty.boolValue) {
            EditorGUI.BeginChangeCheck();
            var keyProperty = serializedObject.FindProperty("encryptKey");
            EditorGUILayout.PropertyField(keyProperty);
            if (EditorGUI.EndChangeCheck()) {
                if (string.IsNullOrEmpty(keyProperty.stringValue))
                    keyProperty.stringValue = Application.productName;
                serializedObject.ApplyModifiedProperties();
            }
        }

        EditorGUI.BeginDisabledGroup(true);
        folder = EditorGUILayout.Foldout(folder, "Outputs files");
        if (folder) {
            for (int i = 0; i < config.paths.Length; i++)
                EditorGUILayout.LabelField(config.paths[i]);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndVertical();
    }

    private void DrawFolderField(string name, SerializedProperty property) {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(name, GUILayout.Width(50f));
        var value = property.stringValue;
        EditorGUILayout.TextField(value);
        if (GUILayout.Button("Choose", GUILayout.Width(70f))) {
            // 来源: https://forum.unity.com/threads/editorutility-openfilepanel-causes-error-log-for-endlayoutgroup.1389873/
            EditorApplication.delayCall += () => {
                value = EditorUtility.OpenFolderPanel($"Choose {name} Path", string.IsNullOrEmpty(value) ? Application.dataPath : value, "");
                if (value.Contains(Application.dataPath))
                    value = value.GetRelativeAssetsPath();
                property.stringValue = value;
                serializedObject.ApplyModifiedProperties();
            };
        }
        EditorGUILayout.EndHorizontal();
    }
}
