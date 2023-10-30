using System.Linq;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using AKIRA;

/// <summary>
/// 层级配置
/// 改手动更新
/// </summary>
// [InitializeOnLoad]
public class LayerConfig
{

    static LayerConfig() { }

    /// <summary>
    /// 更新Layer
    /// </summary>
    /// <param name="path"></param>
    [MenuItem("Tools/AKIRA.Framework/Common/Update Layer")]
    private static void UpdateLayer()
    {
        var path = "Layer".GetScriptLocation();
        // 检查文件是否还存在
        if (string.IsNullOrEmpty(path)) {
            "Layer.cs文件不存在，请在项目内创建一个Layer.cs的脚本！".Error();
            return;
        }
        $"Layer Update: Path => {path}".Log(GameData.Log.Editor);

        var content = @"/// <summary>
/// <para>层级</para>
/// <para>Create&Update By LayerConfig</para>
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
    public static readonly int {name} = {i};
                ";
        }
        content += @"
}

            ";

        File.WriteAllText(path, content);
        AssetDatabase.Refresh();
    }
}