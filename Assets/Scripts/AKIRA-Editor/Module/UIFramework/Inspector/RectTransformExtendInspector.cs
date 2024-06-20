using System;
using UnityEngine;
using UnityEditor;
using AKIRA.UIFramework;
using System.Reflection;
using System.IO;
using System.Linq;

namespace AKIRA.Editor {
    /// <summary>
    /// RectTransform 扩展更新UI组件
    /// </summary>
    [CustomEditor(typeof(RectTransform))]
    public partial class RectTransformExtendInspector : DecoratorEditor {
        public RectTransformExtendInspector(): base("RectTransformEditor"){}

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            try {
                var rect = target as RectTransform;

                #region Panel部分
                var panelType = $"{target.name}Panel".GetConfigTypeByAssembley();
                if (panelType != null) {
                    EditorGUILayout.Space();
                    DrawUpdatePropsBtn(rect.gameObject, false);
                    DrawEditBtn($"{target.name}Panel");
                    if (Application.isPlaying) {
                        DrawActiveBtn(UIManager.Instance.Get(panelType));
                    } else {
                        DrawAnimationPop();
                        DrawDeleteBtn(panelType);
                    }
                }
                #endregion

                #region Component部分
                // 判断是否重命名
                var propRenameComponent = rect.GetComponent<UIPropRenameComponent>();
                var componentType = (propRenameComponent == null ? $"{target.name}Component" : propRenameComponent.propName).GetConfigTypeByAssembley();
                if (componentType != null && componentType.IsSubclassOf(typeof(UIBase))) {
                    EditorGUILayout.Space();
                    DrawUpdatePropsBtn(rect.gameObject, true);
                    DrawEditBtn(propRenameComponent == null ? $"{target.name}Component" : propRenameComponent.propName);
                    if (Application.isPlaying) {
                        var componentProp = GetComponentPropObject(FindComponentParentPanel(rect), componentType);
                        DrawActiveBtn(componentProp);
                    } else {
                        DrawRenameBtn(componentType, rect);
                        DrawAnimationPop();
                        DrawDeleteBtn(componentType);
                    }
                }
                #endregion

                #region Button 红点部分
                if (rect.GetComponent<UnityEngine.UI.Button>()) {
                    DrawRedDotBtn();
                }
                #endregion
            } catch { }
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
        private void DrawUpdatePropsBtn(GameObject go, bool isComponent) {
            if (Application.isPlaying)
                return;

            if (GUILayout.Button("Update Props")) {
                if (isComponent) {
                    GenerateUIPropGUI.UpdateUIProp(go);
                } else {
                    GenerateUIGUI.UpdateUI(go);
                }
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
        /// 绘制重命名按钮
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        private void DrawRenameBtn(Type type, RectTransform target) {
            var component = target.GetComponent<UIPropRenameComponent>();
            if (component == null) {
                if (GUILayout.Button("Rename")) {
                    component = target.gameObject.AddComponent<UIPropRenameComponent>();
                    component.propName = type.Name;
                }
            } else {
                if (EditorUtility.IsPersistent(target))
                    return;
                if (GUILayout.Button("Reset Name")) {
                    var originName = component.propName.Replace("Component", "");
                    GameObject.DestroyImmediate(component);
                    target.gameObject.name = originName;
                }
            }
        }

        /// <summary>
        /// 绘制删除按钮
        /// </summary>
        private void DrawDeleteBtn(Type type) {
            var drawable = GameConfig.Instance.GetConfig<UIRuleConfig>().drawDeleteBtn;
            if (!drawable)
                return;
            EditorGUILayout.Space();
            GUI.color = System.Drawing.Color.OrangeRed.ToUnityColor();
            if (GUILayout.Button("Delete")) {
                var prefabPath = AssetDatabase.GetAssetPath((target as RectTransform).gameObject);
                if (string.IsNullOrEmpty(prefabPath)) {
                    "删除UI预制体和脚本失败:请选择目录下的预制体".Log(GameData.Log.Error);
                    return;
                }

                // 删除Enum
                if (!type.IsSubclassOf(typeof(UIComponentProp))) {
                    var enumLocation = typeof(WinEnum).Name.GetScriptLocation();
                    var enumLines = File.ReadAllLines(enumLocation).ToList();
                    enumLines.Remove(@$"        {target.name},");
                    File.WriteAllLines(enumLocation, enumLines);
                }

                var scriptPath = type.Name.GetScriptLocation();
                var scriptPropPath = $"{type.Name}Prop".GetScriptLocation();
                File.Delete(scriptPropPath);
                File.Delete(scriptPath);
                File.Delete(prefabPath);
                @$"删除UI预制体和脚本 ⬇
                删除预制体路径:         {prefabPath}
                删除脚本路径:           {scriptPath}
                删除脚本Prop路径:       {scriptPropPath}".Log(GameData.Log.Success);
                AssetDatabase.Refresh();
            }
            GUI.color = Color.white;;
        }
    }

    /// <summary>
    /// UIAnimator扩展
    /// </summary>
    public partial class RectTransformExtendInspector {
        private string[] names;
        private Type[] types;
        private int selected = 0;
        private GUIStyle style;

        private void DrawAnimationPop() {
            var rect = target as RectTransform;
            var animation = rect.GetComponent(typeof(IUIAnimation));
            
            style ??= new GUIStyle {
                    richText = true
                };

            EditorGUILayout.BeginHorizontal();
            if (animation == null) {
                if (types == null) {
                    types = typeof(IUIAnimation).Name.GetConfigTypeByInterface();
                    names = new string[types.Length];
                    for (int i = 0; i < types.Length; i++)
                        names[i] = types[i].Name;
                }
                selected = EditorGUILayout.Popup(selected, names);
                if (GUILayout.Button("Add", GUILayout.Width(70f))) {
                    rect.gameObject.AddComponent(types[selected]);
                }
            } else {
                EditorGUILayout.LabelField($"<b>Animed: {animation}</b>".Colorful(Color.white), style);
                if (GUILayout.Button("Remove", GUILayout.Width(70f))) {
                    GameObject.DestroyImmediate(animation, true);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// Button 红点扩展
    /// </summary>
    public partial class RectTransformExtendInspector {
        public void DrawRedDotBtn() {
            var config = GameConfig.Instance.GetConfig<UIRuleConfig>();
            if (!config.enableRedDot)
                return;

            EditorGUILayout.Space();
            // 检查预制体
            if (config.reddotPrefab == null) {
                EditorGUILayout.HelpBox("Enabled reddot, but prefab is null\nPlease check GameConfig reddot prefab", MessageType.Info);
                if (GUILayout.Button("Select GameConfig"))
                    GameConfigWindow.SelectConfig();
                return;
            }

            // 检查红点脚本
            if (config.reddotPrefab.GetComponent<ReddotComponent>() == null) {
                EditorGUILayout.HelpBox("Enabled reddot, but prefab dont contains IReddot interface\nPlease check reddot prefab", MessageType.Info);
                if (GUILayout.Button("Select Reddot"))
                    Selection.activeObject = config.reddotPrefab;
                return;
            }

            var button = target as RectTransform;
            var reddot = button.Find(config.reddotPrefab.name);
            if (reddot != null) {
                if (Application.isPlaying) {
                    EditorGUILayout.LabelField("RedDot is enbaled");
                } else {
                    // 永远在在按钮最下层
                    if (reddot.GetSiblingIndex() != button.childCount - 1)
                        reddot.SetAsLastSibling();
                    // 直接删掉
                    if (GUILayout.Button("Disable RedDot")) {
                        GameObject.DestroyImmediate(reddot.gameObject);
                    } else {
                        var component = reddot.GetComponent<ReddotComponent>();
                        var style = new GUIStyle() { richText = true };
                        EditorGUILayout.BeginVertical("framebox");
                        EditorGUILayout.LabelField($@"<b>Reddot Linker => {component.Linker}</b>".Colorful(Color.white), style);
                        EditorGUILayout.LabelField($@"<b>Reddot Tag    => {component.ReddotTag}</b>".Colorful(Color.white), style);
                        EditorGUILayout.EndVertical();
                    }
                }
            } else {
                if (GUILayout.Button("Enable RedDot")) {
                    reddot = (config.reddotPrefab.CreatePrefab() as GameObject).transform;
                    reddot.SetParent(button, false);
                    reddot.GetComponent<ReddotComponent>().SetLinker();
                }
            }
        }
    }
}