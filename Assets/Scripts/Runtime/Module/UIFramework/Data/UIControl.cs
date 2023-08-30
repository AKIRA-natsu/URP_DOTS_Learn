using System;

/// <summary>
/// UI节点
/// </summary>
[Serializable]
public struct UINode {
    /// <summary>
    /// 节点名称
    /// </summary>
    public string name;
    /// <summary>
    /// 节点路径
    /// </summary>
    public string path;

    public UINode(string name, string path) {
        this.name = name;
        this.path = path;
    }
}

/// <summary>
/// 规则
/// </summary>
[Serializable]
public struct UIControlRule {
    /// <summary>
    /// 组件名称
    /// </summary>
    public string controlName;
    /// <summary>
    /// 命名名称
    /// </summary>
    public string[] ruleNames;
}