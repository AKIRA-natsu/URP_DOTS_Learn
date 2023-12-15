using UnityEngine;
using UnityEditor;
using System.IO;
using AKIRA;

public class ExcelDataUtil : EditorWindow {
    // 路径
    private string loadPath;
    // 更改路劲保存
    private const string LoadKey = "ExcelLoadPath";
    private string scriptPath;
    private string jsonPath;

    private Vector2 view;

    // excels 路径
    private string[] files;

    [MenuItem("Tools/AKIRA.Framework/Common/ExcelDataUtil")]
    private static void ShowWindow() {
        var window = GetWindow<ExcelDataUtil>();
        window.titleContent = new GUIContent("ExcelDataUtil");
        window.minSize = new Vector2(700f, 400f);
        window.Show();
    }

    private void OnEnable() {
        loadPath = LoadKey.EditorGetString();
        if (string.IsNullOrEmpty(loadPath))
            loadPath = Application.dataPath;
        
        scriptPath = Path.Combine(Application.dataPath, "Scripts/Data");
        jsonPath = Path.Combine(Application.streamingAssetsPath, "Json");
        GetFiles();
    }

    private void OnGUI() {
        EditorGUILayout.BeginHorizontal("frameBox");
        loadPath = EditorGUILayout.TextField("LoadPath", loadPath);
        if (GUILayout.Button("Save Path", GUILayout.Width(100f))) {
            LoadKey.EditorSave(loadPath);
            GetFiles();
        }
        if (GUILayout.Button("Fresh Excels", GUILayout.Width(100f))) {
            GetFiles();
        }
        EditorGUILayout.EndHorizontal();

        view = EditorGUILayout.BeginScrollView(view);
        EditorGUILayout.BeginVertical("frameBox");

        foreach (var file in files) {
            EditorGUILayout.BeginHorizontal("box");
            DrawFileElement(file);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    private void GetFiles() {
        files = Directory.GetFiles(loadPath, "*.xls");
        $"ExcelDataUtil: 获得{files.Length}文件".Log(GameData.Log.Editor);
    }

    private void DrawFileElement(string path) {
        var name = Path.GetFileNameWithoutExtension(path);
        EditorGUILayout.LabelField(name);
        
        var scriptName = name.Split("（")[0];
        if (!scriptName.Contains("Data"))
            scriptName += "Data";
        var type = scriptName.GetConfigTypeByAssembley();
        if (type == null) {
            if (GUILayout.Button("Create Script", GUILayout.Width(300))) {
                // 生成
                var file = Path.Combine(scriptPath, $"{scriptName}.cs");
                ExcelHelp.CreateExcelDataScript(path, file);
            }
        } else {
            if (GUILayout.Button("Update Script", GUILayout.Width(150))) {
                // 覆盖
                var file = scriptName.GetScriptLocation();
                ExcelHelp.CreateExcelDataScript(path, file);
            }

            if (GUILayout.Button("Update Json", GUILayout.Width(150))) {
                var json = Path.Combine(jsonPath, $"{scriptName}.json");
                ExcelHelp.CreateExcelJsonScript(path, json);
            }
        }
    }
}