using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using AKIRA.UIFramework;

namespace AKIRA.Editor {
    /// <summary>
    /// 自动生成 UI 脚本 (EditorGUI)
    /// </summary>
    public class GenerateUIGUI : EditorWindow {
#region MenuItem Tools
        // 控制按钮什么时候显示
        [MenuItem("Tools/AKIRA.Framework/Module/UI/[Select Gameobject] CreateUI", true, priority = 1)]
        internal static bool CreateUIActive() {
            var obj = Selection.activeObject;
            if (obj == null || obj is not GameObject)
                return false;
            return $"{obj.name}Component".GetConfigTypeByAssembley() == null;
        }

        [MenuItem("Tools/AKIRA.Framework/Module/UI/[Select Gameobject] CreateUI", priority = 1)]
        internal static void CreateUI() {
            CreateUI(Selection.activeObject as GameObject);
        }

        // 控制按钮什么时候显示
        [MenuItem("Tools/AKIRA.Framework/Module/UI/[Select Gameobject] UpdateUIProp", true, priority = 2)]
        internal static bool UpdateUIActive() {
            var obj = Selection.activeObject;
            if (obj == null || obj is not GameObject)
                return false;
            return $"{obj.name}Panel".GetConfigTypeByAssembley() != null;
        }

        [MenuItem("Tools/AKIRA.Framework/Module/UI/[Select Gameobject] UpdateUIProp", priority = 2)]
        internal static void UpdateUI() {
            UpdateUI(Selection.activeGameObject);
        }
#endregion

#region params
        internal static UIRuleConfig Rule => GameConfig.Instance.GetConfig<UIRuleConfig>();
        private static List<UIPath> nodes = new List<UIPath>();
        private static List<string> btns = new List<string>();
#endregion

        /// <summary>
        /// 生成UI脚本并保存ui prefab
        /// </summary>
        internal static void CreateUI(GameObject obj) {
            var prefabPath = Rule.prefabPath;
            var scriptPath = Rule.scriptPath;
            if (!Directory.Exists(prefabPath))
                Directory.CreateDirectory(prefabPath);
            if (!Directory.Exists(scriptPath))
                Directory.CreateDirectory(scriptPath);

            var name = obj.name;
            var panelPath = $"{scriptPath}/{name}Panel.cs";
            var propPath = $"{scriptPath}/{name}PanelProp.cs";
            var objPath = $"{prefabPath}/{name}.prefab";
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
    [Win(WinEnum.{obj.name}, ""{objPath.GetRelativeAssetsPath()}"", {@type})]
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
                newObj.SetParent(parent, true);
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
            var propPath = $"{name}PanelProp".GetScriptLocation();
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
            TraverseUI(_transform.transform, "");
            StringBuilder content = new StringBuilder();
            // 最后一个是transform根节点
            for (int i = 0; i < nodes.Count - 1; i++) {
                var node = nodes[i];
                // 删去路径根节点  /_transform.name/
                node.path = node.path.Remove(0, _transform.name.Length + 2);
                var componentPropType = $"{node.name}Component".GetConfigTypeByAssembley();
                var paramName = node.name.Replace(" ", "_").Replace("@", "");
                if (componentPropType != null) {
                    AppendContent(ref content, node.name, node.path, componentPropType.ToString(), paramName);
                } else {
                    if (Rule.TryGetControlName(node.name, out string controlName)) {
                        AppendContent(ref content, node.name, node.path, controlName, paramName);
                        if (controlName.Equals("Button"))
                            btns.Add(paramName);
                    }
                }
            }
            return content;
        }

        /// <summary>
        /// 添加字段拼接
        /// </summary>
        /// <param name="content"></param>
        /// <param name="nodeName"></param>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="paramName"></param>
        private static void AppendContent(ref StringBuilder content, string nodeName, string path, string type, string paramName) {
            if (Rule.CheckMatchableControl(nodeName)) {
                content.Append($"        [UIControl(\"{path}\", true)]\n        protected {type} {paramName};\n");
            } else {
                content.Append($"        [UIControl(\"{path}\")]\n        protected {type} {paramName};\n");
            }
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
            var nodeName = parent.name;
            // 判断忽略名单
            if (Rule.IsIgnoreName(nodeName))
                return;

            path += $"/{nodeName}";
            // 判断忽略标签和子节点
            if (parent.childCount != 0 && parent.GetComponent<IUIIgnore>() == null) {
                // 如果是Component，添加自身但省略子节点
                // 但如果遍历的是Component也会省略，判断节点是否为0，如果是0说明遍历Component
                if ($"{nodeName}Component".GetConfigTypeByAssembley() == null || nodes.Count == 0) {
                    for (int i = 0; i < parent.childCount; i++)
                        TraverseUI(parent.GetChild(i), path);
                }
            }

            nodes.Add(new UIPath(nodeName, path));
        }
#endregion
    }
}