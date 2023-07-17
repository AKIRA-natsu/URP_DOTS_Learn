using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AKIRA.Manager;
using UnityEditorInternal;

/// <summary>
/// 引用池的辅助窗口
/// </summary>
public class ReferencePoolWindow : EditorWindow {
    private Vector2 scrollView;

    [MenuItem("Tools/Framework/ReferencePoolWindow")]
    private static void ShowWindow() {
        var window = GetWindow<ReferencePoolWindow>();
        window.titleContent = new GUIContent("ReferencePoolWindow");
        window.Show();
    }

    private void OnGUI() {
        if (!Application.isPlaying) {
            EditorGUILayout.HelpBox("Game Not Start", MessageType.Info);
            return;
        }

        scrollView = EditorGUILayout.BeginScrollView(scrollView);
        var map = ReferenceHelp.ReadOnlyComponentReferenceMap;
        foreach (var kvp in map) {
            DrawElement(kvp.Key, kvp.Value);
        }
        EditorGUILayout.EndScrollView();

        Repaint();
    }

    /// <summary>
    /// 绘制元素
    /// </summary>
    private void DrawElement(Component component, List<IPool> refers) {
        ReorderableList list = new ReorderableList(refers, typeof(IPool));
        EditorGUILayout.BeginVertical("framebox");
        GUI.enabled = false;
        EditorGUILayout.ObjectField(component, typeof(Component), allowSceneObjects : true);
        GUI.enabled = true;
        EditorGUILayout.BeginHorizontal("box");
        list.DoLayoutList();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
}