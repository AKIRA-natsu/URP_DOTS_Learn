using System;
using UnityEngine;
using UnityEditor;
using AKIRA.UIFramework;
using System.Reflection;
using AKIRA;

/// <summary>
/// RectTransform 扩展更新UI组件
/// </summary>
[CustomEditor(typeof(RectTransform))]
public class RectTransformExtendInspector : DecoratorEditor {
    public RectTransformExtendInspector(): base("RectTransformEditor"){}

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        #region Panel部分
        var panelType = $"{target.name}Panel".GetConfigTypeByAssembley();
        if (panelType != null) {
            EditorGUILayout.Space();
            DrawUpdatePropsBtn((target as RectTransform).gameObject);
            DrawEditBtn($"{target.name}Panel");
            if (Application.isPlaying) {
                DrawActiveBtn(UIManager.Instance.Get(panelType));
                DrawMethodBtns(panelType, UIManager.Instance.Get(panelType) ?? null);
            }
        }
        #endregion

        #region Component部分
        var componentType = $"{target.name}Component".GetConfigTypeByAssembley();
        if (componentType != null) {
            EditorGUILayout.Space();
            DrawUpdatePropsBtn((target as RectTransform).gameObject);
            DrawEditBtn($"{target.name}Component");
            if (Application.isPlaying) {
                var componentProp = GetComponentPropObject(FindComponentParentPanel(target as RectTransform), componentType);
                DrawActiveBtn(componentProp);
                DrawMethodBtns(componentType, componentProp);
            }
        }
        #endregion
    }

    /// <summary>
    /// 获得Component所属的Panel类
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    private Type FindComponentParentPanel(RectTransform transform) {
        var name = transform.parent.name;
        if (name.Equals(UI.View.name) || name.Equals(UI.Background.name) || name.Equals(UI.Top.name))
            return $"{transform.name}Panel".GetConfigTypeByAssembley();
        return FindComponentParentPanel(transform.parent as RectTransform);
    }

    /// <summary>
    /// 反射获得游戏内UI的Prop对象
    /// </summary>
    /// <param name="panelType"></param>
    /// <param name="propType"></param>
    /// <returns></returns>
    private UIComponentProp GetComponentPropObject(Type panelType, Type propType) {
        var ui = UIManager.Instance.Get(panelType);
        if (ui == null)
            return default;
        var fields = panelType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields) {
            if (field.FieldType.Equals(propType))
                return field.GetValue(ui) as UIComponentProp;
        }
        return default;
    }

    /// <summary>
    /// 绘制更新Props按钮
    /// </summary>
    /// <param name="go"></param>
    private void DrawUpdatePropsBtn(GameObject go) {
        if (Application.isPlaying)
            return;

        if (GUILayout.Button("Update Props")) {
            GenerateUIGUI.UpdateUI(go);
        }
    }

    /// <summary>
    /// 绘制显示/隐藏按钮
    /// </summary>
    private void DrawActiveBtn(UIComponent component) {
        if (GUILayout.Button("Active")) {
            component.Active = !component.Active;
        }
    }

    /// <summary>
    /// 绘制编辑按钮
    /// </summary>
    /// <param name="name"></param>
    private void DrawEditBtn(string name) {
        if (GUILayout.Button("Edit")) {
            System.Diagnostics.Process.Start(name.GetScriptLocation());
        }
    }

    /// <summary>
    /// 绘制方法按钮
    /// </summary>
    /// <param name="type"></param>
    /// <param name="type"></param>
    private void DrawMethodBtns(Type type, object target) {
        if (target == null)
            return;

        var methodInfos = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var info in methodInfos) {
            var attributes = info.GetCustomAttributes(typeof(UIBtnMethodAttribute), true) as UIBtnMethodAttribute[];
            if (attributes.Length == 0)
                continue;
            if (info.GetParameters().Length != 0) {
                $"UIBtnMethodAttribute 暂时仅支持无参方法".Log(GameData.Log.Warn);
                continue;
            }
            var attribute = attributes[0];
            var name = attribute.name;
            if (String.IsNullOrEmpty(name))
                name = info.Name;
            if (GUILayout.Button(name)) {
                info.Invoke(target, null);
            }
        }
    }
}