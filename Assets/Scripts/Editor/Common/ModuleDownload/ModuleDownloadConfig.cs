using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public List<string> paths = new List<string>();
}

[CreateAssetMenu(fileName = "ModuleDownloadConfig", menuName = "AKIRA.Framework/Common/ModuleDownloadConfig", order = 0)]
public class ModuleDownloadConfig : ScriptableObject {
    public const string GitHubURL = "https://github.com";

    [SerializeField]
    private ModuleConfig[] configs;

    /// <summary>
    /// 獲得具體模塊配置文件
    /// </summary>
    /// <param name="moduleName"></param>
    /// <returns></returns>
    public ModuleConfig GetConfig(string moduleName) => configs.SingleOrDefault(config => config.moduleName.Equals(moduleName));

    /// <summary>
    /// 獲得完成的Git路徑
    /// </summary>
    /// <param name="moduleName"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public string GetTotalGitPath(string moduleName, string path) {
        return Path.Combine(GetConfig(moduleName).gitPath, path);
    }

    /// <summary>
    /// 模塊是否包含路徑
    /// </summary>
    /// <param name="moduleName"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool IsPathInConfig(string moduleName, string path) {
        return GetConfig(moduleName).paths.Contains(GetTotalGitPath(moduleName, path));
    }

    /// <summary>
    /// 添加路徑
    /// </summary>
    /// <param name="moduleName"></param>
    /// <param name="path"></param>
    public void AddPath(string moduleName, string path) {
        var totalPath = GetTotalGitPath(moduleName, path);
        var config = GetConfig(moduleName);
        if (!config.paths.Contains(totalPath))
            config.paths.Add(totalPath);
        totalPath = GetTotalGitPath(moduleName, $"{path}.meta");
        if (!config.paths.Contains(totalPath))
            config.paths.Add(totalPath);
    }

    /// <summary>
    /// 移除路徑
    /// </summary>
    /// <param name="moduleName"></param>
    /// <param name="path"></param>
    public void RemovePath(string moduleName, string path) {
        var totalPath = GetTotalGitPath(moduleName, path);
        var config = GetConfig(moduleName);
        if (config.paths.Contains(totalPath))
            config.paths.Remove(totalPath);
        totalPath = GetTotalGitPath(moduleName, $"{path}.meta");
        if (config.paths.Contains(totalPath))
            config.paths.Remove(totalPath);
    }
}