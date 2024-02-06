using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
#endif

[CreateAssetMenu(fileName = "AssetBundleConfig", menuName = "AKIRA.Framework/Module/AssetBundleConfig", order = 0)]
public class AssetBundleConfig : ScriptableObject {
    // editor下模拟加载ab包方式加载资源
    public bool simulation = false;
    // 用WebRequest测试AB包
    public bool useWebRequestTest = false;
    // 默认streamingassets下加载ab
    public string[] paths;
}

#if UNITY_EDITOR
[CustomEditor(typeof(AssetBundleConfig))]
public class AssetBundleConfigInspector : Editor {
    private const string Extension = ".manifest";

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var config = target as AssetBundleConfig;
        var searchPath = Application.streamingAssetsPath;
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Search Paths(StreamingAssets)")) {
            var paths = Directory.GetFiles(searchPath, "*.*", SearchOption.AllDirectories);

            List<string> result = new();
            foreach (var path in paths) {
                // AB不包含后缀
                if (!string.IsNullOrEmpty(Path.GetExtension(path)))
                    continue;
                
                // 检查是否包含后缀，如果不是说明也不是AB文件
                if (!paths.Contains($"{path}{Extension}"))
                    continue;

                result.Add(path.Replace("\\", "/").Remove(0, searchPath.Length + 1));
            }
            config.paths = result.ToArray();
        }
    }
}
#endif