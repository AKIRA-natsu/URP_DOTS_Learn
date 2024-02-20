using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditorInternal;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using Object = UnityEngine.Object;
using Microsoft.Win32;

/// <summary>
/// 用来保存/读取当前存档的数值
/// </summary>
public class PlayerPrefsWindow : EditorWindow {
    private FileDataView fileView;
    private PrefDataView prefView;

    [MenuItem("Tools/AKIRA.Framework/Common/PlayerPrefsWindow", priority = 150)]
    private static void ShowWindow() {
        var minSize = new Vector2(780, 400);
        var window = GetWindowWithRect<PlayerPrefsWindow>(new Rect(Vector2.zero, minSize), false, "PlayerPrefsWindow");
        window.minSize = minSize;
    }

    public void CreateGUI() {
        VisualElement root = rootVisualElement;

        // 菜单容器
        var menuContainer = new Box() { name = "Menu", style = {
            height = 20f,
            flexDirection = FlexDirection.Row,
            borderBottomColor = Color.grey,
        }};
        menuContainer.Add(new ToolbarButton(ClearPrefs) { text = "Delete All Prefs" } );
        menuContainer.Add(new ToolbarButton(ClearFiles) { text = "Clear All Files" } );
        
        // 视图容器
        var viewContainer = new VisualElement() { name = "View" };
        // 实例化顺序问题，反着排序了。。
        viewContainer.style.flexDirection = FlexDirection.ColumnReverse;
        viewContainer.Add(prefView = new PrefDataView());
        viewContainer.Add(fileView = new FileDataView(prefView.OnShowDataView));

        root.Add(menuContainer);
        root.Add(viewContainer);
    }

    /// <summary>
    /// 清空存档
    /// </summary>
    private void ClearPrefs() {
        PlayerPrefs.DeleteAll();
        Debug.Log("删除全部存档");
    }

    /// <summary>
    /// 清空文件
    /// </summary>
    private void ClearFiles() {
        fileView.ClearFiles();
        prefView.ClearView();
    }
}

internal class FileDataView : IMGUIContainer {
    // 保存类
    private class SaveFileData {
        // 文件
        public Object file;
        // 名称
        public string name;
        // 描述
        public string description;
    }

    // 本地存档 文件
    private List<SaveFileData> texts = new List<SaveFileData>();
    private ReorderableList textList;
    // 滑条
    private Vector2 view;

    // 描述结束标识
    private const string Description = "Description: ";
    // 存档路径
    private string path;

    // 预览事件
    private Action<string> onPreviewRegisters;

    public FileDataView(Action<string> onPreviewRegisters) : base() {
        this.onPreviewRegisters = onPreviewRegisters;

        // style
        // limit max height
        style.maxHeight = EditorWindow.GetWindow<PlayerPrefsWindow>().position.size.y / 2;

        // check path
        path = Path.Combine(Application.dataPath, "StreamingAssets", "~PlayerPrefs Save Files");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        
        ReadFiles();
        // list init
        textList = new ReorderableList(texts, typeof(SaveFileData));
        textList.drawElementCallback += DrawElement;
        textList.onAddCallback += AddElement;
        textList.onRemoveCallback += RemoveElement;

        onGUIHandler = OnGUIHandler;
    }

    private void OnGUIHandler() {
        view = EditorGUILayout.BeginScrollView(view);
        textList.DoLayoutList();
        EditorGUILayout.EndScrollView();
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
            data.name = file.Split('\\').Last().Replace(".txt", "");
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
        if (GUILayout.Button("Write File"))
            WriteFile(data);

        if (GUILayout.Button("Read File"))
            ReadFile(data);

        if (GUILayout.Button("View Prefs"))
            onPreviewRegisters(Path.Combine(path, $"{data.name}.txt"));
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
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
    /// 生成存档文件
    /// </summary>
    private void WriteFile(SaveFileData data) {
        if (string.IsNullOrWhiteSpace(data.name)) {
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

        // 正常都要清掉
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
        }
        Debug.Log("存档修改成功");
    }
    
    /// <summary>
    /// 清空文件
    /// </summary>
    public void ClearFiles() {
        if (Directory.Exists(path)) {
            Directory.Delete(path, true);
            AssetDatabase.Refresh();
            texts.Clear();
        }
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

internal class PrefDataView : VisualElement {
    // 列表头
    private VisualElement header;
    // 滑窗视图
    private ScrollView view;
    // 元素宽度集合
    private float[] widths;
    // 当前读取的路径
    private string path;

    public PrefDataView() : base() {
        this.style.flexDirection = FlexDirection.ColumnReverse;
        this.style.alignItems = Align.Center;
        this.style.flexGrow = 1f;
        this.style.flexShrink = 1f;

        var width = EditorWindow.GetWindow<PlayerPrefsWindow>().position.size.x;
        widths = new float[4] { width / 16f * 3f, width / 8f, width / 2, width / 16f * 3f };

        header = DrawRegisterElement("Key", "Type", "Value",
                                    ("Edit", _ => System.Diagnostics.Process.Start(path)),
                                    ("Close", _ => ClearView()));
        this.Add(view = new ScrollView());
    }

    /// <summary>
    /// 显示存档View
    /// </summary>
    public void OnShowDataView(string path) {
        this.path = path;
        ClearView();

        this.Add(header);

        var lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++) {
            var line = lines[i].Split("||");
            view.Add(DrawRegisterElement(line[0], line[2], line[1],
                                        ("Change", OnChangeRegisterValue),
                                        ("Delete", OnDeleteRegisterValue)));
        }
    }

    /// <summary>
    /// 绘制列表元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private VisualElement DrawRegisterElement(string key, string type, string value, params (string name, Action<VisualElement> callback)[] btns) {
        VisualElement element = new() {
            name = "origin",
            style = {
                flexDirection = FlexDirection.Row,
                flexShrink = 1f,
                flexGrow = 1f,
            },
        };

        Label keyLabel, typeLabel, valueLabel;
        VisualElement btnRoot;
        element.Add(keyLabel = new Label(key) { name = "key", style = { width = widths[0], overflow = Overflow.Hidden } });
        element.Add(typeLabel = new Label(type) { name = "type", style = { width = widths[1], overflow = Overflow.Hidden } });
        element.Add(valueLabel = new Label(value) { name = "value", style = { width = widths[2], overflow = Overflow.Hidden } });
        element.Add(btnRoot = new VisualElement() { name = "btns", style = {
            width = widths[3],
            alignItems = Align.Center, 
            justifyContent = Justify.Center,
            flexDirection = FlexDirection.Row,
        }});

        foreach (var btn in btns)
            btnRoot.Add(new Button(() => btn.callback(element)) { text = btn.name });

        return element;
    }

    // 显示/隐藏元素
    private void ActiveElement(VisualElement target, bool active) {
        target.visible = active;
        target.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void OnChangeRegisterValue(VisualElement element) {
        var btns = element.Q<VisualElement>("btns");
        if (element.name == "origin") {
            element.name = "changing";
            var label = element.Q<Label>("value");
            ActiveElement(label, false);
            element.Insert(3, new TextField() { name = "value", value = label.text, style = {
                width = widths[2],
                overflow = Overflow.Hidden,
            }});

            // 隐藏原先的按钮
            foreach (var btn in btns.Children())
                ActiveElement(btn, false);
            // 添加确定和取消
            btns.Add(new Button(() => ApplyChanged(element, true)) { text = "ok" });
            btns.Add(new Button(() => ApplyChanged(element, false)) { text = "cancel" });
        } else {
            element.name = "origin";
            // var label = element.ElementAt(2) as Label;
            // label.text = element.Q<TextField>("value").value;
            // element.RemoveAt(3);
            // ActiveElement(label, true);

            // 去掉最后两个并还原按钮
            btns.RemoveAt(btns.childCount - 1);
            btns.RemoveAt(btns.childCount - 1);
            foreach (var btn in btns.Children())
                ActiveElement(btn, true);
        }
    }

    /// <summary>
    /// 是否接受更改
    /// </summary>
    /// <param name="element"></param>
    /// <param name="apply"></param>
    private void ApplyChanged(VisualElement element, bool apply) {
        var label = element.ElementAt(2) as Label;
        if (apply) {
            var originValue = label.text;
            var changeValue = element.Q<TextField>("value").value;

            // change file value
            var key = $"{element.Q<Label>("key").text}||";

            var lines = File.ReadAllLines(path).ToList();
            var targetLine = lines.Single(line => line.StartsWith(key));
            var newLine = targetLine.Replace(originValue, changeValue);
            lines[lines.IndexOf(targetLine)] = newLine;
            File.WriteAllLines(path, lines);

            label.text = changeValue;
        }
        element.RemoveAt(3);
        ActiveElement(label, true);

        OnChangeRegisterValue(element);
    }

    private void OnDeleteRegisterValue(VisualElement element) {
        var key = $"{element.Q<Label>("key").text}||";

        var lines = File.ReadAllLines(path).ToList();
        var targetLine = lines.Single(line => line.StartsWith(key));
        lines.Remove(targetLine);
        File.WriteAllLines(path, lines);
        
        view.Remove(element);
    }

    /// <summary>
    /// 清空面板
    /// </summary>
    public void ClearView() {
        view.Clear();

        if (this.Contains(header))
            this.Remove(header);
    }
}