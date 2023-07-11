using System.Linq;
using UnityEditor;
using UnityEngine;

public static class EditorExtend {
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
}