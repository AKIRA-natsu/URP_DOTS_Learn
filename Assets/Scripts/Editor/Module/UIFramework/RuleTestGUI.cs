using UnityEngine;
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

    private void OnGUI() {
        size = EditorGUILayout.BeginScrollView(size);
        // title
        EditorGUILayout.LabelField("自动生成 UI (GUI)");
        EditorGUILayout.Space();

        var rule = GameConfig.Instance.GetConfig<UIRuleConfig>();

        if (rule == null) {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField($"配置文件没有规则文件");
            EditorGUI.EndDisabledGroup();
            return;
        } else {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("已读取规则文件");
            EditorGUI.EndDisabledGroup();
        }

        if (Selection.gameObjects.Length == 0) return;
        GameObject obj = Selection.gameObjects[0];
        var content = GenerateUIGUI.LinkControlContent(obj.transform);
        EditorGUILayout.TextArea(content.ToString());
        EditorGUILayout.EndScrollView();
    }
}