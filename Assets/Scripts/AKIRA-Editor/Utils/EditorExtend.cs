using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class EditorExtend {
    /// <summary>
    /// 获得文件路径
    /// </summary>
    /// <param name="name"></param>
    /// <param name="extension">后缀，不需要带 . </param>
    public static string GetFileLocation(this string name, string extension) {
        var guids = AssetDatabase.FindAssets(name);
        foreach (var guid in guids) {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Split('/').Last().Equals($"{name}.{extension}")) {
                return path;
            }
        }
        return default;
    }

    /// <summary>
    /// 获得脚本系统位置
    /// </summary>
    /// <param name="script">脚本名称</param>
    /// <returns></returns>
    public static string GetScriptLocation(this string name) {
        var guids = AssetDatabase.FindAssets(name);
        foreach (var guid in guids) {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Split('/').Last().Equals($"{name}.cs")) {
                return Application.dataPath + "/" + path.Substring(7);
            }
        }
        return default;
    }

    /// <summary>
    /// 编辑器下生成预制体
    /// </summary>
    /// <param name="script">预制体源文件</param>
    /// <returns></returns>
    public static Object CreatePrefab(this Object prefab) {
        return PrefabUtility.InstantiatePrefab(prefab);
    }

    /// <summary>
    /// 获得资源路径
    /// </summary>
    /// <param name="path">
    ///     例：C://Demo/Assets/Res/Textures/Test.png => Assets/Res/Textures/Test.png
    /// </param>
    /// <returns></returns>
    public static string GetRelativeAssetsPath(this string path) {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }

    /// <summary>
    /// 获得详细路径
    /// </summary>
    /// <param name="path">
    ///     例：Assets/Res/Textures/Test.png => C://Demo/Assets/Res/Textures/Test.png
    /// </param>
    /// <returns></returns>
    public static string GetFullAssetsPath(this string path) {
        return Path.Combine(Application.dataPath, path.Replace("Assets/", ""));
    }

    /// <summary>
    /// 清空控制台
    /// </summary>
    public static void ClearConsole() {
        Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
        Type logEntries = assembly.GetType("UnityEditor.LogEntries");
        MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
        clearConsoleMethod.Invoke(new object(), null);
    }
}
