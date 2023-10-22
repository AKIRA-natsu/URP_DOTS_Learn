using UnityEngine;

[CreateAssetMenu(fileName = "AssetBundleConfig", menuName = "AKIRA.Framework/Module/AssetBundleConfig", order = 0)]
public class AssetBundleConfig : ScriptableObject {
    // editor下模拟加载ab包方式加载资源
    public bool simulation = false;
    // 用WebRequest测试AB包
    public bool useWebRequestTest = false;
    // 默认streamingassets下加载ab
    public string[] paths;
}