using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using AKIRA.UIFramework;
using AKIRA;

/// <summary>
/// 自动生成 UI 脚本 (EditorGUI)
/// </summary>
public class GenerateUIGUI : EditorWindow {
#region MenuItem Tools
    [MenuItem("Tools/AKIRA.Framework/Module/UI/CreateUI(Select Gameobject)")]
    internal static void CreateUI() {
        var objs = Selection.gameObjects;
        if (objs == null || objs.Length == 0)
            $"未选择物体".Log(GameData.Log.Warn);
        for (int i = 0; i < objs.Length; i++) {
            var obj = objs[i];
            CreateUI(obj.name, obj);
        }
    }

    [MenuItem("Tools/AKIRA.Framework/Module/UI/UpdateUIProp(Select GameObject)")]
    internal static void UpdateUI() {
        var obj = Selection.activeGameObject;
        if (obj == null)
            $"未选择物体".Log(GameData.Log.Warn);
        UpdateUI(obj);
    }
#endregion

#region params
    internal static UIRuleConfig rule = null;
    private static List<UINode> nodes = new List<UINode>();
    private static List<string> btns = new List<string>();
#endregion

    /// <summary>
    /// 生成UI脚本并保存ui prefab
    /// </summary>
    internal static void CreateUI(string @enum, GameObject obj) {
        if (!Directory.Exists("Assets/Scripts/UI"))
            Directory.CreateDirectory("Assets/Scripts/UI");
        if (!Directory.Exists(Application.dataPath + $"/Res/MainBundle/Prefabs/UI"))
            Directory.CreateDirectory(Application.dataPath + $"/Res/MainBundle/Prefabs/UI");

        var name = obj.name;
        var panelPath = $"Assets/Scripts/UI/{name}Panel.cs";
        var propPath = $"Assets/Scripts/UI/{name}PanelProp.cs";
        var objPath = Application.dataPath + $"/Res/MainBundle/Prefabs/UI/{name}.prefab";
        var parent = obj.transform.parent.gameObject;

        // =============================================================================================================================

        if (File.Exists(propPath)) {
            $"已经存在prop文件\n进行删除, 路劲为{propPath}".Log(GameData.Log.Warn);
            File.Delete(propPath);
        }
        string propContent =
        #region code

$@"using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AKIRA.UIFramework {{
    public class {name}PanelProp : UIComponent {{";
        propContent += $"\n{LinkControlContent(obj.transform)}";
        propContent +=
$@"    }}
}}";
        #endregion

        File.WriteAllText(propPath, propContent);
        $"生成prop.cs完毕\n路劲为{propPath}".Log(GameData.Log.Success);

        // =============================================================================================================================

        if (File.Exists(panelPath)) {
            $"已经存在panel文件\n进行删除，路劲为{panelPath}".Log(GameData.Log.Warn);
            File.Delete(panelPath);
        }

        string @type = default;
        if (parent.name.Equals("View"))
            @type = "WinType.Normal";
        if (parent.name.Equals("Top"))
            @type = "WinType.Interlude";
        if (parent.name.Equals("Background"))
            @type = "WinType.Notify";

        string panelContent =
        #region code

$@"using UnityEngine;

namespace AKIRA.UIFramework {{
    [Win(WinEnum.{@enum}, ""{objPath.GetRelativeAssetsPath()}"", {@type})]
    public class {name}Panel : {name}PanelProp {{
        public override void Awake(object obj) {{
            base.Awake(obj);";
        panelContent += $"\n{LinkBtnListen()}";
        panelContent +=
$@"        }}
    }}
}}";
        #endregion

        File.WriteAllText(panelPath, panelContent);
        $"生成panel.cs完毕\n路劲为{panelPath}".Log(GameData.Log.Success);

        // =============================================================================================================================
        
        // 检查是否存在预制体
        if (File.Exists(objPath)) {
            $"已经存在预制体{obj}\n进行删除，路径为{objPath}".Log(GameData.Log.Warn);
            File.Delete(objPath);
        }

        var prefab = PrefabUtility.SaveAsPrefabAsset(obj, objPath, out bool result);
        GameObject.DestroyImmediate(obj);
        if (!result) {
            $"保存预制体{obj}失败".Colorful(Color.red).Error();
        } else {
            var newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            newObj.SetParent(parent);
            newObj.transform.localScale = Vector3.one;
            $"保存预制体{newObj}成功\n路径为{objPath}".Log(GameData.Log.Success);
        }
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 更新UI Prop脚本
    /// </summary>
    /// <param name="obj"></param>
    internal static void UpdateUI(GameObject obj) {
        var name = obj.name;
        var propPath = $"Assets/Scripts/UI/{name}PanelProp.cs";
        if (!File.Exists(propPath)) {
            $"{propPath}下不存在{name}PanelProp.cs".Log(GameData.Log.Error);
            return;
        }

        File.Delete(propPath);
        string propContent =
        #region code

$@"using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AKIRA.UIFramework {{
    public class {name}PanelProp : UIComponent {{";
        propContent += $"\n{LinkControlContent(obj.transform)}";
        propContent +=
$@"    }}
}}";
        #endregion

        File.WriteAllText(propPath, propContent);
        $"更新prop.cs完毕\n路劲为{propPath}".Log(GameData.Log.Success);
        AssetDatabase.Refresh();
    }

#region 辅助事件
    /// <summary>
    /// 获得UI个节点组件
    /// </summary>
    /// <param name="_transform"></param>
    /// <returns></returns>
    internal static StringBuilder LinkControlContent(Transform _transform) {
        if (nodes.Count != 0) nodes.Clear();
        if (btns.Count != 0) btns.Clear();
        if (rule == null) rule = GameConfig.Instance.GetConfig<UIRuleConfig>();
        TraverseUI(_transform.transform, "");
        StringBuilder content = new StringBuilder();
        // 最后一个是transform根节点
        for (int i = 0; i < nodes.Count - 1; i++) {
            var node = nodes[i];
            // 删去路径根节点  /_transform.name/
            node.path = node.path.Remove(0, _transform.name.Length + 2);
            var componentPropType = $"{node.name}Component".GetConfigTypeByAssembley();
            var paramName = node.name.Replace(" ", "");
            if (componentPropType != null) {
                if (rule.CheckMatchableControl(node.name)) {
                    content.Append($"        [UIControl(\"{node.path}\", true)]\n        protected {componentPropType} {paramName};\n");
                } else {
                    content.Append($"        [UIControl(\"{node.path}\")]\n        protected {componentPropType} {paramName};\n");
                }
                continue;
            }
            if (rule.TryGetControlName(node.name, out string controlName)) {
                if (rule.CheckMatchableControl(node.name)) {
                    content.Append($"        [UIControl(\"{node.path}\", true)]\n        protected {controlName} {paramName};\n");
                } else {
                    content.Append($"        [UIControl(\"{node.path}\")]\n        protected {controlName} {paramName};\n");
                }
                if (controlName.Equals("Button"))
                    // btns.Add(node.path.Replace('/', '_').Replace("@", ""));
                    btns.Add(paramName);
            }
        }
        return content;
    }

    /// <summary>
    /// 添加按钮监听事件
    /// </summary>
    /// <returns></returns>
    internal static StringBuilder LinkBtnListen() {
        StringBuilder content = new StringBuilder();
        foreach (var btn in btns)
            content.Append($"            this.{btn}.onClick.AddListener(() => {{}});\n");
        return content;
    }

    /// <summary>
    /// 遍历UI节点
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="path"></param>
    private static void TraverseUI(Transform parent, string path) {
        // 去掉TexturePro Sprite的影响
        if (parent.name.Contains("[TextMeshPro/Sprite]"))
            return;
            
        path += $"/{parent.name}";
        if (parent.childCount != 0) {
            // 省略
            if (parent.GetComponent<IUIIgnore>() != null)
                return;
            
            // 如果是Component，添加自身但省略子节点
            // 但如果遍历的是Component也会省略，判断节点是否为0，如果是0说明遍历Component
            if ($"{parent.name}Component".GetConfigTypeByAssembley() == null || nodes.Count == 0) {
                for (int i = 0; i < parent.childCount; i++)
                    TraverseUI(parent.GetChild(i), path);
            }
        }

        nodes.Add(new UINode(parent.name, path));
    }
#endregion
}