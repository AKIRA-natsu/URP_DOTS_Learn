using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ExcelDataConfig))]
public class ExcelDataConfigInspector : Editor {
    private bool folder;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        ExcelDataConfig config = target as ExcelDataConfig;

        EditorGUILayout.BeginVertical("framebox");
        DrawFolderField("Excel", ref config.excelPath);
        DrawFolderField("Script", ref config.scriptPath);
        DrawFolderField("Output", ref config.output);

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("With encrypt will output bytes, else output json", MessageType.Info);
        config.encrypt = EditorGUILayout.Toggle("Encrypt", config.encrypt);
        if (config.encrypt) {
            config.encryptKey = EditorGUILayout.TextField("Encrypt Key", config.encryptKey);
            if (string.IsNullOrEmpty(config.encryptKey))
                config.encryptKey = Application.productName;
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

    private void DrawFolderField(string name, ref string value) {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(name, GUILayout.Width(50f));
        EditorGUILayout.TextField(value);
        if (GUILayout.Button("Choose", GUILayout.Width(70f))) {
            value = EditorUtility.OpenFolderPanel("Choose Excel Path", string.IsNullOrEmpty(value) ? Application.dataPath : value, "");
            if (value.Contains(Application.dataPath))
                value = value.GetRelativeAssetsPath();
            GUIUtility.ExitGUI();
            AssetDatabase.Refresh();
        }
        EditorGUILayout.EndHorizontal();
    }
}