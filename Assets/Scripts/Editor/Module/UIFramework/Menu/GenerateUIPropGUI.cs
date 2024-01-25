using UnityEngine;
using UnityEditor;
using System.IO;

namespace AKIRA.Editor {
    /// <summary>
    /// 自动生成 UI 组件脚本 (EditorGUI)
    /// </summary>
    public class GenerateUIPropGUI : EditorWindow {
#region MenuItem Tools
        // 控制按钮什么时候显示
        [MenuItem("Tools/AKIRA.Framework/Module/UI/[Select Gameobect] CreateUIComponent", true, priority = 3)]
        internal static bool CreateUIComponentActive() {
            var obj = Selection.activeObject;
            if (obj == null || obj is not GameObject)
                return false;
            return $"{obj.name}Panel".GetConfigTypeByAssembley() == null;
        }

        [MenuItem("Tools/AKIRA.Framework/Module/UI/[Select Gameobect] CreateUIComponent", priority = 3)]
        internal static void CreateUIComponent() {
            var objs = Selection.gameObjects;
            for (int i = 0; i < objs.Length; i++)
                CreateUIProp(objs[i]);
        }

        // 控制按钮什么时候显示
        [MenuItem("Tools/AKIRA.Framework/Module/UI/[Select Gameobect] UpdateUIComponentProp", true, priority = 4)]
        internal static bool UpdateUIComponentActive() {
            var obj = Selection.activeObject;
            if (obj == null || obj is not GameObject)
                return false;
            return $"{obj.name}Component".GetConfigTypeByAssembley() != null;
        }

        [MenuItem("Tools/AKIRA.Framework/Module/UI/[Select Gameobect] UpdateUIComponentProp", priority = 4)]
        internal static void UpdateUIComponent() {
            UpdateUIProp(Selection.activeObject as GameObject);
        }
#endregion

        /// <summary>
        /// 生成UI组件脚本并保存Prefab
        /// </summary>
        /// <param name="obj"></param>
        internal static void CreateUIProp(GameObject obj) {
            var prefabPath = $"{GenerateUIGUI.Rule.prefabPath}/Component";
            var scriptPath = $"{GenerateUIGUI.Rule.scriptPath}/Component";
            if (!Directory.Exists(prefabPath))
                Directory.CreateDirectory(prefabPath);
            if (!Directory.Exists(scriptPath))
                Directory.CreateDirectory(scriptPath);
            
            var name = obj.name;
            var panelComponentPath = $"{scriptPath}/{name}Component.cs";
            var componentPropPath = $"{scriptPath}/{name}ComponentProp.cs";
            var objPath = $"{prefabPath}/{name}.prefab";

            // =============================================================================================================================

            if (File.Exists(componentPropPath)) {
                $"已经存在prop文件\n进行删除，路劲为{componentPropPath}".Log(GameData.Log.Warn);
                File.Delete(componentPropPath);
            }
            string propContent =
            #region code

$@"using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AKIRA.UIFramework {{
    public class {name}ComponentProp : UIComponentProp {{";
        propContent += $"\n{GenerateUIGUI.LinkControlContent(obj.transform)}";
        propContent +=
$@"    }}
}}";
            #endregion

            File.WriteAllText(componentPropPath, propContent);
            $"生成prop.cs完毕\n路劲为{componentPropPath}".Log(GameData.Log.Success);

            // =============================================================================================================================

            if (File.Exists(panelComponentPath)) {
                $"已经存在panel文件\n进行删除，路劲为{panelComponentPath}".Log(GameData.Log.Warn);
                File.Delete(panelComponentPath);
            }
            string panelContent =
            #region code

$@"using UnityEngine;

namespace AKIRA.UIFramework {{
    public class {name}Component : {name}ComponentProp {{
        public override void Awake(object obj) {{
            base.Awake(obj);";
        panelContent += $"\n{GenerateUIGUI.LinkBtnListen()}";
        panelContent +=
$@"        }}
    }}
}}";
            #endregion

            File.WriteAllText(panelComponentPath, panelContent);
            $"生成panel.cs完毕\n路劲为{panelComponentPath}".Log(GameData.Log.Success);

            // =============================================================================================================================
            
            // 检查是否存在预制体
            if (File.Exists(objPath)) {
                $"已经存在预制体{obj}\n进行删除, 路径为{objPath}".Log(GameData.Log.Warn);
                File.Delete(objPath);
            }

            var prefab = PrefabUtility.SaveAsPrefabAsset(obj, objPath, out bool result);
            var parent = obj.transform.parent.gameObject;
            GameObject.DestroyImmediate(obj);
            if (!result) {
                $"保存预制体{obj}失败".Colorful(Color.red).Error();
            } else {
                var newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                newObj.SetParent(parent);
                newObj.transform.localScale = Vector3.one;
                newObj.transform.localPosition = Vector3.zero;
                $"保存预制体{newObj}成功\n路径为{objPath}".Log(GameData.Log.Success);
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 更新UI Component Prop脚本
        /// </summary>
        /// <param name="obj"></param>
        internal static void UpdateUIProp(GameObject obj) {
            var name = obj.name;
            var componentPropPath = $"{name}ComponentProp".GetScriptLocation();
            File.Delete(componentPropPath);
            
            string propContent =
            #region code

$@"using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AKIRA.UIFramework {{
    public class {name}ComponentProp : UIComponentProp {{";
        propContent += $"\n{GenerateUIGUI.LinkControlContent(obj.transform)}";
        propContent +=
$@"    }}
}}";
            #endregion

            File.WriteAllText(componentPropPath, propContent);
            $"更新prop.cs完毕\n路劲为{componentPropPath}".Log(GameData.Log.Success);
            AssetDatabase.Refresh();
        }
    }
}