using UnityEngine;
using UnityEditor;
using System.IO;
using AKIRA;

public class GenerateUIPropGUI : EditorWindow {
    [MenuItem("Tools/AKIRA.Framework/Module/UI/CreateUIComponent(Select Gameobects)")]
    internal static void CreateUIComponent() {
        var objs = Selection.gameObjects;
        if (objs == null || objs.Length == 0)
            $"未选择物体".Log(GameData.Log.Warn);
        for (int i = 0; i < objs.Length; i++) {
            var obj = objs[i];
            CreateUIProp(obj);
        }
    }

    [MenuItem("Tools/AKIRA.Framework/Module/UI/UpdateUIComponentProp(Select Gameobects)")]
    internal static void UpdateUIComponent() {
        var objs = Selection.gameObjects;
        if (objs == null || objs.Length == 0)
            $"未选择物体".Log(GameData.Log.Warn);
        for (int i = 0; i < objs.Length; i++) {
            var obj = objs[i];
            UpdateUIProp(obj);
        }
    }


    /// <summary>
    /// 生成UI组件脚本并保存Prefab
    /// </summary>
    /// <param name="obj"></param>
    internal static void CreateUIProp(GameObject obj) {
        if (!Directory.Exists("Assets/Scripts/UI/Component"))
            Directory.CreateDirectory("Assets/Scripts/UI/Component");
        if (!Directory.Exists(Application.dataPath + $"/Res/MainBundle/Prefabs/UI/Component"))
            Directory.CreateDirectory(Application.dataPath + $"/Res/MainBundle/Prefabs/UI/Component");
        
        var name = obj.name;
        var panelComponentPath = $"Assets/Scripts/UI/Component/{name}Component.cs";
        var componentPropPath = $"Assets/Scripts/UI/Component/{name}ComponentProp.cs";
        var objPath = Application.dataPath + $"/Res/MainBundle/Prefabs/UI/Component/{name}.prefab";

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
        var componentPropPath = $"Assets/Scripts/UI/Component/{name}ComponentProp.cs";
        if (!File.Exists(componentPropPath)) {
            $"{componentPropPath}下不存在{name}ComponentProp.cs".Log(GameData.Log.Error);
            return;
        }

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