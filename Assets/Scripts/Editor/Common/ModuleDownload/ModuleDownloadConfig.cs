using UnityEngine;

[System.Serializable]
public class ModuleConfig {
    // 模块名称
    public string moduleName;
    // git url
    public string gitPath;
    // 是否已经加载过
    public bool isLoaded;
    // 模块的所有路径字段
    public string[] paths;
}

[CreateAssetMenu(fileName = "ModuleDownloadConfig", menuName = "AKIRA.Framework/Common/ModuleDownloadConfig", order = 0)]
public class ModuleDownloadConfig : ScriptableObject {
    public ModuleConfig[] configs;
}