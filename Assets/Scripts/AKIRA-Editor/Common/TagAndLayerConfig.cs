using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using AKIRA;
using System.Reflection;
using UnityEditorInternal;

/// <summary>
/// 层级和标签自动更新
/// </summary>
// [InitializeOnLoad]
public class TagAndLayerConfig
{

    static TagAndLayerConfig() { }

    /// <summary>
    /// 更新Layer
    /// </summary>
    /// <param name="path"></param>
    [MenuItem("Tools/AKIRA.Framework/Common/Update Tags && Layers", priority = 100)]
    private static void UpdateTagAndLayer()
    {
        var path = "TagAndLayer".GetScriptLocation();
        // 检查文件是否还存在
        if (string.IsNullOrEmpty(path)) {
            "TagAndLayer.cs文件不存在，请在项目内创建一个TagAndLayers.cs的脚本！".Error();
            return;
        }
        $"Layer Update: Path => {path}".Log(GameData.Log.Editor);

        #region layers
        var content = @"/// <summary>
/// <para>Create&Update By TagAndLayerConfig</para>
/// </summary>
public static class Layer {
            ";
        for (int i = 0; i < 32; i++)
        {
            var name = LayerMask.LayerToName(i).Replace(" ", "");
            if (String.IsNullOrEmpty(name))
                continue;
            content += @$"
    /// <summary>
    /// {name}
    /// </summary>
    public const int {name} = {i};
                ";
        }
        content += @"
}

            ";
        #endregion

        #region Sorting layers
        var names = GetSortingLayerNames();
        content += @"
/// <summary>
/// <para>Create&Update By TagAndLayerConfig</para>
/// </summary>
public static class Sorting {
            ";
        foreach (var name in names) 
        {
            content += @$"
    /// <summary>
    /// {name}
    /// </summary>
    public const string {name} = ""{name}"";
                ";
        }
        content += @"
}

            ";
        #endregion

        #region tags
        content += @"
/// <summary>
/// <para>Create&Update By TagAndLayerConfig</para>
/// </summary>
public static class Tags {
            ";
        var tags = InternalEditorUtility.tags;
        foreach (var tag in tags)
        {
            content += @$"
    /// <summary>
    /// {tag}
    /// </summary>
    public const string {tag} = ""{tag}"";
                ";
        }
        content += @"
}

            ";
        #endregion

        File.WriteAllText(path, content);
        AssetDatabase.Refresh();
    }

    // Get the sorting layer names
    internal static string[] GetSortingLayerNames() {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        return (string[])sortingLayersProperty.GetValue(null, new object[0]);
    }

    // Get the unique sorting layer IDs -- tossed this in for good measure
    internal static int[] GetSortingLayerUniqueIDs() {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
        return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
    }
}