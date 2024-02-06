#if UNITY_EDITOR
using System;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AKIRA.Manager;
using UnityEditorInternal;
using AKIRA;
using Unity.VisualScripting;
using AKIRA.UIFramework;

/// <summary>
/// Guide Editor
/// </summary>
public class GuideWindow : EditorWindow {
    // XML文件
    private XML xml;

    // 指引列表转换
    private List<GuideInfo> infos = new List<GuideInfo>();
    // ScrollView滑动
    private Vector2 scrollView;
    // 
    private ReorderableList guideList;

    [MenuItem("Tools/AKIRA.Framework/Module/GuideWindow")]
    private static void ShowWindow() {
        var window = GetWindow<GuideWindow>();
        window.titleContent = new GUIContent("GuideWindow");
        window.minSize = new Vector2(500, 300);
        window.Show();
    }

    private void OnEnable() {
        guideList = new ReorderableList(infos, typeof(GuideInfo));
        guideList.drawElementCallback = DrawElement;
        guideList.drawHeaderCallback = DrawElementHeader;
        guideList.onChangedCallback = UpdateInfoID;
    }

    private void OnDisable() {
        guideList = null;
    }

    /// <summary>
    /// 更新键值
    /// </summary>
    /// <param name="list"></param>
    private void UpdateInfoID(ReorderableList list) {
        for (int i = 0; i < infos.Count; i++) {
            infos[i].ID = i;
        }
    }

    /// <summary>
    /// 元素绘制
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="index"></param>
    /// <param name="isActive"></param>
    /// <param name="isFocused"></param>
    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
        GuideInfo element = guideList.list[index] as GuideInfo;
        EditorGUI.LabelField(new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight), $"Guide Index {element.ID}");
        EditorGUI.LabelField(new Rect(rect.x + 100, rect.y, rect.width - 100, EditorGUIUtility.singleLineHeight), $"{element.completeType}");
        EditorGUI.LabelField(new Rect(rect.x + 180, rect.y, rect.width - 180, EditorGUIUtility.singleLineHeight), $"{element.arrowTarget?.name}");

        if (index == guideList.index) {
            DrawInfoProperty(element, index);
        }
    }

    /// <summary>
    /// 列表名称
    /// </summary>
    /// <param name="rect"></param>
    private void DrawElementHeader(Rect rect) {
        EditorGUI.LabelField(rect, "Guide Infos");
    }

    private void OnGUI() {
        // 第一次打开窗口读取指引数据
        if (xml == null) {
            xml = new XML(GameData.SaveKey.Guide);
            if (xml.Exist()) {
                xml.Read((x) => {
                    var nodes= x.SelectSingleNode("Data").ChildNodes;
                    // 清空上一次
                    infos.Clear();
                    foreach (XmlElement node in nodes) {
                        // 提前处理一下路径问题
                        var completeType = (GuideCompleteType)node.GetAttribute(GuideInfoName.GuideCompleteType).ToInt32();
                        var path = node.GetAttribute(GuideInfoName.ArrowTargetPath);
                        GameObject target = default;
                        if (completeType == GuideCompleteType.UIWorld) {
                            // 查找预制体
                            var prefabName = path.Split("/")[0];
                            var prefab = $"{prefabName}Panel".GetConfigTypeByAssembley().GetAttribute<WinAttribute>().Data.path.LoadAssetAtPath<GameObject>();
                            target = prefab.transform.Find(path.Replace($"{prefabName}/", "")).gameObject;
                        } else {
                            // 3D物体下简单找到对象
                            target = GameData.Asset.Guide3DRoot.LoadAssetAtPath<Transform>().Find(path).gameObject;
                        }

                        infos.Add(new GuideInfo() {
                            ID = node.GetAttribute(GuideInfoName.ID).ToInt32(),
                            completeType = completeType,
                            isShowBg = node.GetAttribute(GuideInfoName.IsShowBg).ToInt32() == 1,
                            dialog = node.GetAttribute(GuideInfoName.Dialog),
                            dialogDirection = (GuideDialogDirection)node.GetAttribute(GuideInfoName.DialogDirection).ToInt32(),
                            useArrow = node.GetAttribute(GuideInfoName.UseArrow).ToInt32() == 1,
                            arrowTarget = target,
                            reachDistance = node.GetAttribute(GuideInfoName.ReachDistance).ToSingle(),
                            controlByIGuide = node.GetAttribute(GuideInfoName.ControlByIGuide).ToInt32() == 1,
                        });
                    }
                });
            } else {
                xml.Create((x) => {
                    var root = x.CreateElement("Data");
                    x.AppendChild(root);
                });
                AssetDatabase.Refresh();
            }
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label($"Path: {GameData.SaveKey.Guide}");
        GUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal("box");
        if (GUILayout.Button("Save File"))
            SaveFile();
        
        if (GUILayout.Button("Delete File"))
            DeleteFile();
        EditorGUILayout.EndHorizontal();

        scrollView = EditorGUILayout.BeginScrollView(scrollView);
        // 更新ReorderableList
        guideList.DoLayoutList();
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// 保存文件
    /// </summary>
    private void SaveFile() {
        var tempMap = new Dictionary<int, GuideInfo>();
        for (int i = 0; i < infos.Count; i++) {
            var info = infos[i];
            if (!info.IsVailiable) {
                infos.RemoveAt(i--);
                continue;
            }
            // 重新更新键值
            info.ID = i;
            tempMap.Add(info.ID, info);
        }

        xml.Update((x) => {
            XmlNode data = x.SelectSingleNode("Data");
            var nodes = data.ChildNodes;
            for (int i = 0; i < nodes.Count; i++) {
                XmlElement node = nodes[i] as XmlElement;
                var id = node.GetAttribute(GuideInfoName.ID).ToInt32();
                if (tempMap.ContainsKey(id)) {
                    // 更新节点
                    UpdateNode(node, tempMap[id]);
                    // 暂存移除
                    tempMap.Remove(id);
                } else {
                    // 删除也放在更新
                    // map不存在节点移除
                    data.RemoveChild(node);
                    i--;
                }
            }

            // 添加新增节点
            foreach (var info in tempMap.Values) {
                XmlElement node = x.CreateElement($"Guide{info.ID}");
                UpdateNode(node, info);
                data.AppendChild(node);
            }
        });
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    private void DeleteFile() {
        if (!xml.Exist())
            return;
        xml.Delete();
        infos.Clear();
        xml = null;
    }

    /// <summary>
    /// 绘制元素
    /// </summary>
    /// <param name="info"></param>
    /// <param name="i"></param>
    private void DrawInfoProperty(GuideInfo info, int i) {
        // 设置排颜色（info类决定）
        var message = info.GetInfoGUIMessage();
        GUI.color = message.Color;
        GUILayout.BeginHorizontal("box");
        EditorGUILayout.BeginVertical("frameBox");
        if (!String.IsNullOrEmpty(message.WarnMessage))
            EditorGUILayout.HelpBox(message.WarnMessage, MessageType.Info);
        EditorGUI.BeginDisabledGroup(true);
        info.ID = EditorGUILayout.IntField(GuideInfoName.ID, i);
        EditorGUI.EndDisabledGroup();
        info.completeType = (GuideCompleteType)EditorGUILayout.EnumPopup(GuideInfoName.GuideCompleteType, info.completeType);

        if (info.completeType != GuideCompleteType.None)
            info.arrowTarget = (GameObject)EditorGUILayout.ObjectField(GuideInfoName.ArrowTargetPath, info.arrowTarget, typeof(GameObject), allowSceneObjects : true);

        // 检测是否有IGuide接口
        info.controlByIGuide = info.arrowTarget != null && info.arrowTarget.TryGetComponent<IGuide>(out _);

        if (info.completeType == GuideCompleteType.TDWorld) {
            info.useArrow = EditorGUILayout.Toggle(GuideInfoName.UseArrow, info.useArrow);
            if (!info.controlByIGuide)
                info.reachDistance = EditorGUILayout.FloatField(GuideInfoName.ReachDistance, info.reachDistance);
        }

        if (info.completeType == GuideCompleteType.UIWorld) {
            info.isShowBg = EditorGUILayout.Toggle(GuideInfoName.IsShowBg, info.isShowBg);
            info.dialogDirection = (GuideDialogDirection)EditorGUILayout.EnumPopup(GuideInfoName.DialogDirection, info.dialogDirection);
            if (info.dialogDirection != GuideDialogDirection.None)
                info.dialog = EditorGUILayout.TextField(GuideInfoName.Dialog, info.dialog);
        }
        GUI.color = Color.white;

        EditorGUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 更新节点
    /// </summary>
    /// <param name="node"></param>
    /// <param name="info"></param>
    private void UpdateNode(XmlElement node, GuideInfo info) {
        node.SetAttribute(GuideInfoName.ID, info.ID.ToString());
        node.SetAttribute(GuideInfoName.GuideCompleteType, ((int)info.completeType).ToString());
        node.SetAttribute(GuideInfoName.IsShowBg, (info.isShowBg ? 1 : 0).ToString());
        node.SetAttribute(GuideInfoName.Dialog, info.dialog);
        node.SetAttribute(GuideInfoName.DialogDirection, ((int)info.dialogDirection).ToString());
        node.SetAttribute(GuideInfoName.UseArrow, (info.useArrow ? 1 : 0).ToString());
        node.SetAttribute(GuideInfoName.ArrowTargetPath, info.arrowTarget.GetPath().Replace(GuideInfo.TDRemovePathName, "").Replace(GuideInfo.UIRemovePathName, ""));
        node.SetAttribute(GuideInfoName.ReachDistance, info.reachDistance.ToString());
        node.SetAttribute(GuideInfoName.ControlByIGuide, (info.controlByIGuide ? 1 : 0).ToString());
    }
}
#endif