using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.IO;
using Microsoft.Win32;

/// <summary>
/// 用来保存/读取当前存档的数值
/// </summary>
public class PlayerPrefsHelpWindow : EditorWindow {
    // 保存类
    private class SaveFileData {
        // 文件
        public UnityEngine.Object file;
        // 名称
        public string name;
        // 描述
        public string description;
    }

    // 本地存档 文件
    private ReorderableList textList;
    private List<SaveFileData> texts = new List<SaveFileData>();

    // 描述结束标识
    private const string Description = "Description: ";

    // 路径
    private string path;

    // 是否删除全部存档后再写入
    private bool deleteAllSave = false;
    
    [MenuItem("Tools/AKIRA.Framework/Common/PlayerPrefsHelpWindow")]
    private static void OpenWindow() {
        var window = GetWindow<PlayerPrefsHelpWindow>();
        window.titleContent = new GUIContent("PlayerPrefsHelpWindow");
        window.minSize = new Vector2(600, 300);
        window.Show();
    }

    private void OnGUI() {
        EditorGUILayout.BeginHorizontal("framebox");
        deleteAllSave = EditorGUILayout.Toggle(deleteAllSave, GUILayout.Width(25f));
        EditorGUILayout.LabelField("是否删除全部存档后在读取文件");
        EditorGUILayout.EndHorizontal();

        textList.DoLayoutList();
    }

    private void OnDisable() {
        textList = null;
    }

    private void OnEnable() {
        path = Path.Combine(Application.dataPath, "streamingAssets", "~PlayerPrefs Save Files");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        
        ReadFiles();

        textList = new ReorderableList(texts, typeof(SaveFileData));
        textList.drawElementCallback += DrawElement;
        textList.onAddCallback += AddElement;
        textList.onRemoveCallback += RemoveElement;
    }

    /// <summary>
    /// 获得文件夹下的文件
    /// </summary>
    private void ReadFiles() {
        var files = Directory.GetFiles(path);
        texts.Clear();
        foreach (var file in files) {
            if (file.Contains(".meta"))
                continue;
            var data = new SaveFileData();
            data.file = AssetDatabase.LoadAssetAtPath(file.Replace(Application.dataPath, "Assets"), typeof(UnityEngine.Object));
            var splits = file.Split('\\');
            data.name = splits[splits.Length - 1].Replace(".txt", "");
            data.description = File.ReadAllLines(file)[0].Replace(Description, "");
            texts.Add(data);
        }
    }

    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
        SaveFileData data = textList.list[index] as SaveFileData;
        string name = data.name ?? default;
        string description = data.description ?? default;
        EditorGUI.LabelField(new Rect(rect.x, rect.y, 250, EditorGUIUtility.singleLineHeight), $"File Name: {name}");
        EditorGUI.LabelField(new Rect(rect.x + 250, rect.y, rect.width - 250, EditorGUIUtility.singleLineHeight), $"Description: {description}");

        // 绘制内容
        if (index != textList.index)
            return;

        EditorGUILayout.BeginVertical("framebox");
        EditorGUILayout.BeginVertical("box");
        data.file = EditorGUILayout.ObjectField("Data File: ", data.file, typeof(UnityEngine.Object), false);
        // 存在文件不可更改名称
        GUI.enabled = data.file == null;
        if (data.file != null)
            data.name = data.file.name;
        data.name = EditorGUILayout.TextField("File Name: ", data.name);
        GUI.enabled = true;
        data.description = EditorGUILayout.TextField("File Description: ", data.description);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal("box");
        if (GUILayout.Button("Write File")) {
            WriteFile(data);
        }

        if (GUILayout.Button("Read File")) {
            ReadFile(data);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal("box");
        if (GUILayout.Button("Delete All")) {
            if (Directory.Exists(path)) {
                Directory.Delete(path, true);
                AssetDatabase.Refresh();
                texts.Clear();
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 生成存档文件
    /// </summary>
    private void WriteFile(SaveFileData data) {
        if (String.IsNullOrWhiteSpace(data.name)) {
            data.name = $"{Application.productName}_{DateTime.Now.ToShortDateString().Replace("/", "_")}";
        }
        
        // 判断名字为空且已经存在同名文件夹
        if (texts.Where(d => d.name.Equals(data.name)).Count() > 1) {
            data.description = $"已经存在 {data.name} 了，请重新命名";
            return;
        }

        var map = GetProjectRegistry();
        if (map == null)
            return;
            
        var filePath = Path.Combine(path, $"{data.name}.txt");
        var content = @$"{Description}{data.description}";
        foreach (var kvp in map) {
            content += $"\n{kvp.Key}||{kvp.Value.Item1}||{kvp.Value.Item2}";
        }

        if (!File.Exists(filePath)) {
            var stream = new FileStream(filePath, FileMode.OpenOrCreate);
            stream.Dispose();
            // 刷新文件夹
            AssetDatabase.Refresh();
        }

        File.WriteAllText(filePath, content);
        data.file = AssetDatabase.LoadAssetAtPath(filePath.Replace(Application.dataPath, "Assets"), typeof(UnityEngine.Object));
        Debug.Log("保存存档");
    }

    /// <summary>
    /// 通过文件修改本地存档
    /// </summary>
    /// <param name="data"></param>
    private void ReadFile(SaveFileData data) {
        if (data.file == null)
            return;

        if (deleteAllSave)
            PlayerPrefs.DeleteAll();

        var contents = File.ReadAllLines(Path.Combine(path, $"{data.name}.txt"));
        // 第一行是描述
        for (int i = 1; i < contents.Length; i++) {
            var content = contents[i];
            var splits = content.Split("||");
            var key = splits[0].Trim();
            var value = splits[1].Trim();
            var type = Type.GetType(splits[2].Trim());
            // 判断类型
            if (type.Equals(typeof(System.Int32))) {
                PlayerPrefs.SetInt(key, Convert.ToInt32(value));
            } else if (type.Equals(typeof(System.Int64))) {
                PlayerPrefs.SetFloat(key, Convert.ToInt64(value));
            } else {
                PlayerPrefs.SetString(key, value);
            }
            // 用注册表修改存在读取不了的问题
        }
        Debug.Log("存档修改成功");
    }

    private void AddElement(ReorderableList list) {
       texts.Add(new SaveFileData());
    }

    private void RemoveElement(ReorderableList list) {
        var data = list.list[list.index] as SaveFileData;
        if (data.file != null) {
            File.Delete(Path.Combine(path, $"{data.file.name}.txt"));
        }

        texts.RemoveAt(list.index);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 获得注册表
    /// </summary>
    /// <returns></returns>
    private Dictionary<string, (object, Type)> GetProjectRegistry() {
        string registryPath = string.Format("Software\\Unity\\UnityEditor\\{0}\\{1}", Application.companyName, Application.productName);
        RegistryKey registry = Registry.CurrentUser.OpenSubKey(registryPath, true);

        // if (registryKey == null) {
            // registryKey = Registry.CurrentUser.CreateSubKey(registryPath);
        // }

        if (registry == null)
            return default;

        Dictionary<string, (object, Type)> result = new Dictionary<string, (object, Type)>();
        
        var names = registry.GetValueNames();
        for (int i = 0; i < names.Length; i++) {
            var index = names[i].LastIndexOf('_');
            var name = names[i].Remove(index);
            var value = registry.GetValue(names[i]);
            if (name.ToLower().Contains("unity"))
                continue;

            if (value.GetType().Equals(typeof(Byte[]))) {
                result.Add(name, (PlayerPrefs.GetString(name), typeof(string)));
            } else {
                result.Add(name, (value, value.GetType()));
            }

        }

        return result;
    }
}
