using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using AKIRA.Manager;

[CustomEditor(typeof(UpdateSystem))]
public class UpdateSystemInspector : Editor {
    // 折叠数据
    private class FoldData : IPool {
        // 折叠
        public Dictionary<UpdateMode, bool> folds = new Dictionary<UpdateMode, bool>();

        public FoldData() {
            foreach (UpdateMode mode in System.Enum.GetValues(typeof(UpdateMode)))
                folds.Add(mode, false);
        }

        public void Wake(object data = null) {}

        public void Recycle(object data = null) {
            foreach (var key in folds.Keys)
                folds[key] = false;
        }
    }

    /// <summary>
    /// 折叠数据字典
    /// </summary>
    /// <typeparam name="string">对应更新组键值</typeparam>
    /// <typeparam name="FoldData">折叠数据</typeparam>
    /// <returns></returns>
    private Dictionary<string, FoldData> foldDatas = new Dictionary<string, FoldData>();

    // 更新提示文本
    private const string UpdateEnable = "更新中";
    private const string UpdateDisable = "暂停";
    
    public override void OnInspectorGUI() {
        // base.OnInspectorGUI();
        
        if (!Application.isPlaying) {
            EditorGUILayout.HelpBox("Game Not Start", MessageType.Info);
        } else {
            ShowDebugWindow(target as UpdateSystem);
        }
    }

    /// <summary>
    /// 初始化文件夹
    /// </summary>
    private void InitFoldOut(in List<string> keys) {
        var foldKeys = new List<string>(foldDatas.Keys);
        foreach (var key in foldKeys) {
            if (keys.Contains(key)) {
                continue;
            } else {
                // 组被删除
                foldDatas.Remove(key);
            }
        }

        foreach (var key in keys) {
            if (foldDatas.ContainsKey(key)) {
                continue;
            } else {
                // 新增组
                foldDatas.Add(key, ReferencePool.Instance.Instantiate<FoldData>());
            }
        }
    }

    private void ShowDebugWindow(UpdateSystem manager) {
        var datas = manager.GroupMap;
        var keys = new List<string>(datas.Keys);
        var groups = new List<UpdateGroup>(datas.Values);
        InitFoldOut(keys);
        for (int i = 0; i < datas.Count; i++) {
            var group = groups[i];
            var foldData = foldDatas[keys[i]];

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            group.Updating = EditorGUILayout.Toggle(group.Updating, GUILayout.Width(15f));
            GUI.color = group.Updating ? Color.green : Color.red;
            EditorGUILayout.LabelField($"({(group.Updating ? UpdateEnable : UpdateDisable)}) Group Name: {keys[i]}");
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();

            foreach (UpdateMode mode in System.Enum.GetValues(typeof(UpdateMode))) {
                var updateList = group.UpdateMap[mode];
                var spaceUpdateList = group.SpaceUpdateMap[mode];
                var updateCount = updateList.Count;
                var infoCount = spaceUpdateList.Count;
                if (updateCount + infoCount != 0) {
                    EditorGUILayout.BeginVertical("frameBox");
                    foldData.folds[mode] = EditorGUILayout.Foldout(foldData.folds[mode], $"{mode}列表：总数: {updateCount + infoCount}");
                    if (foldData.folds[mode]) {

                        if (updateCount != 0) {
                            EditorGUILayout.BeginVertical("frameBox");
                            for (int j = 0; j < updateCount; j++) {
                                var update = updateList[j];
                                EditorGUILayout.LabelField(update.ToString());
                            }
                            EditorGUILayout.EndVertical();
                        }

                        if (infoCount != 0) {
                            EditorGUILayout.BeginVertical("frameBox");
                            for (int j = 0; j < infoCount; j++) {
                                var info = spaceUpdateList[j];
                                EditorGUILayout.LabelField($"{info.iupdate}  间隔：{info.interval}s 上一次更新：{System.Math.Round(info.lastUpdateTime, 2)}s");
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                    EditorGUILayout.EndVertical();
                } else {
                    // continue;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        // 强制每帧刷新一次
        // if (EditorApplication.isPlaying)
            Repaint();
    }
}