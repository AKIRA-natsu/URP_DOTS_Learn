using UnityEngine;
using UnityEditor;
using System.IO;
using AKIRA;

/// <summary>
/// excel 2 json/byte & class
/// </summary>
public class ExcelDataUtil : EditorWindow {
    private ExcelDataConfig config;
    private Editor configEditor;

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
        config = GameConfig.Instance.GetConfig<ExcelDataConfig>();

        if (config == null) {
            "ExcelDataConfig is null".Error();
            return;
        }

        configEditor = Editor.CreateEditor(config);
        GetFiles();
    }

    private void OnGUI() {
        if (config == null)
            return;

        EditorGUI.BeginDisabledGroup(true);
        configEditor.OnInspectorGUI();
        EditorGUI.EndDisabledGroup();
        
        if (!config.IsValid(out string message)) {
            EditorGUILayout.HelpBox(message, MessageType.Error);
            return;
        }

        EditorGUILayout.BeginVertical("frameBox");
        EditorGUILayout.BeginHorizontal("box");
        if (GUILayout.Button("Refresh"))
            GetFiles();
        
        if (files == null || files.Length == 0) {
            GUIUtility.ExitGUI();
            return;
        }
        if (GUILayout.Button("Create All Scripts"))
            CreateAllScripts();
        if (GUILayout.Button("Create All Outputs"))
            CreateAllOutputs();

        
        EditorGUILayout.EndHorizontal();

        view = EditorGUILayout.BeginScrollView(view);
        foreach (var file in files) {
            EditorGUILayout.BeginHorizontal("box");
            DrawFileElement(file);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }

    private void GetFiles() {
        if (string.IsNullOrEmpty(config.excelPath))
            return;
        files = Directory.GetFiles(config.excelPath, "*.xls");
        config.UpdatePaths();
        $"ExcelDataUtil: 获得{files.Length}文件, 已转化文件数量 {config.paths.Length}".Log(GameData.Log.Editor);
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
                var file = Path.Combine(config.scriptPath, $"{scriptName}.cs");
                ExcelHelp.CreateExcelDataScript(path, file);
                AssetDatabase.Refresh();
            }
        } else {
            if (GUILayout.Button("Update Script", GUILayout.Width(150))) {
                // 覆盖
                var file = scriptName.GetScriptLocation();
                ExcelHelp.CreateExcelDataScript(path, file);
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Update Output", GUILayout.Width(150))) {
                if (config.encrypt) {
                    var @byte = Path.Combine(config.output, $"{scriptName}.bytes");
                    ExcelHelp.CreateExcelByteScript(path, @byte, config.encryptKey, scriptName);
                } else {
                    var json = Path.Combine(config.output, $"{scriptName}.json");
                    ExcelHelp.CreateExcelJsonScript(path, json);
                }
                AssetDatabase.Refresh();
            }
        }
    }

    /// <summary>
    /// 更新创建所有类
    /// </summary>
    private void CreateAllScripts() {
        for (int i = 0; i < files.Length; i++) {
            var file = files[i];
            var name = Path.GetFileNameWithoutExtension(file);
            var scriptName = $"{name.Split("（")[0]}Data";

            var cancel = EditorUtility.DisplayCancelableProgressBar("Generator scripts", $"generator script {scriptName}.cs", (float)i / files.Length);
            if (cancel)
                break;   
            
            var type = scriptName.GetConfigTypeByAssembley();
            string scriptPath = type == null ? Path.Combine(config.scriptPath, $"{scriptName}.cs") : scriptName.GetScriptLocation();
            ExcelHelp.CreateExcelDataScript(file, scriptPath);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 更新输出文件
    /// </summary>
    private void CreateAllOutputs() {
        for (int i = 0; i < files.Length; i++) {
            var file = files[i];
            var name = Path.GetFileNameWithoutExtension(file);
            var outputName = $"{name.Split("（")[0]}Data";

            var cancel = EditorUtility.DisplayCancelableProgressBar("Generator output files", $"generator file {outputName}", (float)i / files.Length);
            if (cancel)
                break;

            if (config.encrypt) {
                var outputPath = Path.Combine(config.output, $"{outputName}.bytes");
                ExcelHelp.CreateExcelByteScript(file, outputPath, config.encryptKey, outputName);
            } else {
                var outputPath = Path.Combine(config.output, $"{outputName}.json");
                ExcelHelp.CreateExcelJsonScript(file, outputPath);
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }
}
