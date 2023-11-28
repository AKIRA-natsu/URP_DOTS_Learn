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

    [MenuItem("Tools/AKIRA.Framework/Common/PlayerPrefsWindow")]
    private static void ShowWindow() {
        var window = GetWindow<PlayerPrefsWindow>();
        window.titleContent = new GUIContent("PlayerPrefsWindow");
        window.minSize = new Vector2(720, 360);
    }

    public void CreateGUI() {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        // var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(location.Replace($".cs", $".uss"));
        // root.styleSheets.Add(styleSheet);

        // 菜单容器
        var menuContainer = new Box() { name = "Menu" } ;
        menuContainer.style.flexDirection = FlexDirection.Row;
        menuContainer.style.borderBottomColor = Color.grey;
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
    private Action<Dictionary<string, (object, Type)>> onPreviewRegisters;

    public FileDataView(Action<Dictionary<string, (object, Type)>> onPreviewRegisters) : base() {
        this.onPreviewRegisters = onPreviewRegisters;
        
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
            onPreviewRegisters(GetProjectRegistry());
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
            // 用注册表修改存在读取不了的问题
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

    public PrefDataView() : base() {
        this.style.flexDirection = FlexDirection.ColumnReverse;
        this.style.alignItems = Align.Center;

        header = DrawRegisterElement("Key", "Type", "Value", ("Close", _ => ClearView()));
        this.Add(view = new ScrollView());
    }

    /// <summary>
    /// 显示存档View
    /// </summary>
    /// <param name="onPreviewRegisters"></param>
    public void OnShowDataView(Dictionary<string, (object, Type)> onPreviewRegisters) {
        ClearView();

        this.Add(header);
        foreach (var kvp in onPreviewRegisters)
            view.Add(DrawRegisterElement(kvp.Key, kvp.Value.Item2.ToString(), kvp.Value.Item1.ToString(),
                                        ("Change", OnChangeRegisterValue),
                                        ("Delete", OnDeleteRegisterValue)));
    }

    /// <summary>
    /// 绘制列表元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private VisualElement DrawRegisterElement(string key, string type, string value, params (string name, Action<VisualElement> callback)[] btns) {
        VisualElement element = new() { name = "origin" };
        element.style.flexDirection = FlexDirection.Row;
        element.style.flexShrink = 1f;

        Label keyLabel, typeLabel, valueLabel;
        VisualElement btnRoot;
        element.Add(keyLabel = new Label(key) { name = "key" });
        element.Add(typeLabel = new Label(type) { name = "type" });
        element.Add(valueLabel = new Label(value) { name = "value" });
        element.Add(btnRoot = new VisualElement() { name = "btns" });

        keyLabel.style.minWidth = 650f / 4;
        typeLabel.style.minWidth = 650f / 4;
        valueLabel.style.minWidth = 650f / 4;
        btnRoot.style.minWidth = 650f / 4;
        btnRoot.style.flexDirection = FlexDirection.Row;

        foreach (var btn in btns)
            btnRoot.Add(new Button(() => btn.callback(element)) { text = btn.name });

        return element;
    }

    private void OnChangeRegisterValue(VisualElement element) {
        if (element.name == "origin") {
            element.name = "changing";
            var value = element.Q<Label>("value").text;
            element.RemoveAt(2);
            var text = new TextField() { name = "value", value = value };
            text.style.minWidth = 650f / 4 - 6f;
            element.Insert(2, text);

            var btns = element.Q<VisualElement>("btns");
            // 隐藏原先的按钮
            foreach (var btn in btns.Children()) {
                btn.visible = false;
                btn.style.display = DisplayStyle.None;
            }
            
            // 添加确定和取消
            btns.Add(new Button(() => OnChangeRegisterValue(element)) { text = "ok" });
            btns.Add(new Button(() => OnChangeRegisterValue(element)) { text = "cancel" });
        } else {
            element.name = "origin";
            var value = element.Q<TextField>("value").value;
            element.RemoveAt(2);
            var label = new Label(value) { name = "value" };
            label.style.minWidth = 650f / 4;
            element.Insert(2, label);

            var btns = element.Q<VisualElement>("btns");
            // 去掉最后两个并还原按钮
            btns.RemoveAt(btns.childCount - 1);
            btns.RemoveAt(btns.childCount - 1);
            foreach (var btn in btns.Children()) {
                btn.visible = true;
                btn.style.display = DisplayStyle.Flex;
            }
        }
        Debug.Log("待添加，不知道是修改文件还是本地注册表");
    }

    private void OnDeleteRegisterValue(VisualElement element) {
        Debug.Log("待添加，不知道是删除文件还是本地注册表");
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