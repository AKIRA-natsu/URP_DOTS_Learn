using System.Text;
using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using AKIRA.UIFramework;
using AKIRA;

/// <summary>
/// UI测试
/// </summary>
public class RuleTestGUI : EditorWindow {
    /// <summary>
    /// 面板大小
    /// </summary>
    private Vector2 size;

    [MenuItem("Tools/AKIRA.Framework/Module/UI/UIRuleTest(GUI)")]
    public static void Open() {
        var gui = GetWindow<RuleTestGUI>();
        gui.titleContent = new GUIContent("UI规则测试");
    }

    private static UIRuleConfig rule = null;
    private List<UINode> nodes = new List<UINode>();

    private void OnGUI() {
        // EditorGUI.BeginChangeCheck();
        size = EditorGUILayout.BeginScrollView(size);
        // title
        EditorGUILayout.LabelField("自动生成 UI (GUI)");
        EditorGUILayout.Space();

        rule = GameData.Path.UIConfig.Load<UIRuleConfig>();

        if (rule == null) {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField($"路径下没有规则文件 => {GameData.Path.UIConfig}");
            EditorGUI.EndDisabledGroup();

            // if (GUILayout.Button("规则文件位置")) {
            //     if (string.IsNullOrEmpty(rulePath))
            //         rulePath = Application.dataPath;
            //     rulePath = EditorUtility.OpenFilePanelWithFilters("选择文件位置", rulePath, new string[] {"Asset", "asset"});

            //     // 加上 Assets
            //     rulePath = rulePath.Replace(Application.dataPath + "/Resources/", "").Split('.')[0];
            //     rule = rulePath.Load<UIRule>();
            // }
            return;
        } else {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("已读取规则文件");
            EditorGUI.EndDisabledGroup();
        }

        if (Selection.gameObjects.Length == 0) return;
        GameObject obj = Selection.gameObjects[0];
        // EditorGUI.BeginDisabledGroup(true);
        // EditorGUILayout.TextArea(n + "   " + controlName);
        // EditorGUI.EndDisabledGroup();
        LinkControlContent(obj.transform);
        EditorGUILayout.EndScrollView();
        // EditorGUI.EndChangeCheck();
    }

    /// <summary>
    /// 添加按钮监听事件
    /// </summary>
    /// <returns></returns>
    private StringBuilder LinkControlContent(Transform _transform) {
        if (nodes.Count != 0) nodes.Clear();
        EditorGUI.BeginDisabledGroup(true);
        TraverseUI(_transform.transform, "");
        StringBuilder content = new StringBuilder("下面是添入脚本代码\n");
        // 最后一个是transform根节点
        for (int i = 0; i < nodes.Count - 1; i++) {
            var node = nodes[i];
            node.path = node.path.Remove(0, _transform.name.Length + 2);
            if (rule.TryGetControlName(node.name, out string controlName)) {
                if (rule.CheckMatchableControl(node.name)) {
                    node.name = node.name.Replace("@", "");
                    content.Append($"        [UIControl(\"{node.path}\", true)]\n        private {controlName} {node.name};\n");
                } else {
                    content.Append($"        [UIControl(\"{node.path}\")]\n        private {controlName} {node.name};\n");
                }
            }
        }
        EditorGUILayout.TextArea(content.ToString());
        EditorGUI.EndDisabledGroup();
        return content;
    }

    /// <summary>
    /// 遍历UI节点
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="path"></param>
    private void TraverseUI(Transform parent, string path) {
        path += $"/{parent.name}";
        if (parent.childCount != 0)
            for (int i = 0; i < parent.childCount; i++)
                TraverseUI(parent.GetChild(i), path);

        EditorGUILayout.TextArea(parent.name + "   " + path);

        nodes.Add(new UINode(parent.name, path));
    }
}