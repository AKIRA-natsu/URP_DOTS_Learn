#if UNITY_EDITOR
using System.IO;
#endif
using UnityEngine;

/// <summary>
/// excel数据配置
/// </summary>
[CreateAssetMenu(fileName = "ExcelDataConfig", menuName = "AKIRA.Framework/Module/ExcelDataConfig", order = 0)]
public class ExcelDataConfig : ScriptableObject {
    [HideInInspector]
    public string excelPath;    // excel路径
    [HideInInspector]
    public string scriptPath;   // 脚本路径
    [HideInInspector]
    public string output;       // 输出路径

    [HideInInspector]
    public bool encrypt;        // 是否加密
    [HideInInspector]
    public string encryptKey;   // 加密钥匙

    [HideInInspector]
    public string[] paths;      // 转换路径列表

#if UNITY_EDITOR
    /// <summary>
    /// Editor 用
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public bool IsValid(out string message) {
        message = default;
        if (string.IsNullOrEmpty(excelPath)) {
            message = "Excel path is null";
            return false;
        }

        if (string.IsNullOrEmpty(scriptPath)) {
            message = "Script path is null";
            return false;
        }

        if (!scriptPath.StartsWith("Assets")) {
            message = "Script path is not start with product folder";
            return false;
        }

        if (string.IsNullOrEmpty(output)) {
            message = "Output path is null";
            return false;
        }

        if (!output.StartsWith("Assets")) {
            message = "Output path is not start with product folder";
            return false;
        }

        if (output.Contains("Resources") || output.Contains("StreamingAssets")) {
        
        } else {
            message = "Output path is not in Resources or StreamingAssets folder";
            return false;
        }

        return true;
    }

    /// <summary>
    /// Editor 用
    /// </summary>
    public void UpdatePaths() {
        if (string.IsNullOrEmpty(output))
            return;
        string extense = encrypt ? "*.bytes" : "*.json";
        var files = Directory.GetFiles(output, extense);
        paths = new string[files.Length];
        
        // 查看存放的目录
        string checkStr = output.Contains("Resources") ? "Resources" : "StreamingAssets";

        for (int i = 0; i < files.Length; i++) {
            var file = files[i];
            // 检查StreamingAssets的
            var index = file.IndexOf(checkStr);
            paths[i] = file.Remove(0, index + checkStr.Length + 1);
        }
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
    }
#endif
}