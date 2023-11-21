using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using System;
using System.Collections;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AKIRA.Editor {
    public class SourceReferWindow : EditorWindow {
        // 去除后缀
        private readonly string[] withoutExtensions = {
            ".meta", ".asmdef", ".json", ".cs", ".dll", ".xml", ".p7s", ".txt", ".nupkj"
        };

        // 右下列表父物体
        private VisualElement referview;
        // 预览窗口
        private PreviewElementView preview;

        [MenuItem("Tools/AKIRA.Framework/Common/SourceReferWindow")]
        public static void ShowWindow() {
            SourceReferWindow wnd = GetWindow<SourceReferWindow>();
            wnd.titleContent = new GUIContent("SourceReferWindow");
            wnd.minSize = new Vector2(960, 540);
        }

        public void CreateGUI() {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            var location = typeof(SourceReferWindow).Name.GetScriptLocation().GetRelativeAssetsPath();

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(location.Replace(".cs", ".uxml"));
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);

            // Bind Mune Click Actions
            var previewMenu = root.Q<ToolbarMenu>("PreviewMenu");
            previewMenu.menu.AppendAction("Default is never shown", a => {}, a => DropdownMenuAction.Status.None);
            previewMenu.menu.AppendAction("Inspector", a => preview.SwitchPreview(PreviewElementView.PreviewType.Inspector), a => DropdownMenuAction.Status.Normal);
            previewMenu.menu.AppendAction("Preview", a => preview.SwitchPreview(PreviewElementView.PreviewType.Preview), a => DropdownMenuAction.Status.Normal);

            var refreshBtn = root.Q<ToolbarButton>("RefreshBtn");
            refreshBtn.clickable.clicked += RefreshResources;

            // SourceView
            root.Q<VisualElement>("LeftPanel").Add(new SourceElementView(GetResources(), SearchReferences));
            // ReferView save parent
            referview = root.Q<VisualElement>("RightBottomPanel");
            // Preview
            preview = new PreviewElementView();
            root.Q<VisualElement>("RightTopPanel").Add(preview);
        }

        /// <summary>
        /// 刷新资源
        /// 重新找找一遍资源塞到左侧列表
        /// </summary>
        private void RefreshResources() {
            VisualElement leftPanel = rootVisualElement.Q<VisualElement>("LeftPanel");
            leftPanel.Clear();
            leftPanel.Add(new SourceElementView(GetResources(), SearchReferences));
            referview.Clear();
            preview.UpdatePreview(null);
        }

        /// <summary>
        /// 获得所有去除后缀的资源
        /// </summary>
        /// <returns></returns>
        private string[] GetResources() {
            return Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories).Where(s => !withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
        }

        private void SearchReferences(ReorderableList list, string res) {
            // 更新预览
            preview.UpdatePreview(res.GetRelativeAssetsPath().LoadAssetAtPath<Object>());

            // 序列化模式，可以手动unity设置，否则无法找到确实使用了资源的文件
            EditorSettings.serializationMode = SerializationMode.ForceText;
            string guid = AssetDatabase.AssetPathToGUID(res.GetRelativeAssetsPath());

            List<string> referFiles = new();
            int startIndex = 0;
            EditorApplication.update = () => {
                var file = list.list[startIndex].ToString();
                bool isCancel = EditorUtility.DisplayCancelableProgressBar("Find Match Resources", file, (float)startIndex / list.count);

                if (Regex.IsMatch(File.ReadAllText(file), guid)) {
                    // file.Log(context : AssetDatabase.LoadAssetAtPath<Object>(GetRelativeAssetsPath(file)));
                    referFiles.Add(file);
                }

                if (isCancel || ++startIndex >= list.count) {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    referview.Clear();
                    referview.Add(new ReferElementView(referFiles, res.Split('\\').Last()));
                }
            };
        }
    }

    #region ReorderableList Element => IMGUIContainer
    /// <summary>
    /// ReorderableList Base Type
    /// </summary>
    public abstract class ReorderableListElementView : IMGUIContainer {
        // 滑条
        private Vector2 view;
        // 列表
        protected ReorderableList ViewList { get; private set; }

        public ReorderableListElementView(IList elements, Type elementType) : base() {
            ViewList = new ReorderableList(elements, elementType);
            ViewList.drawHeaderCallback = DrawHeader;
            ViewList.drawElementCallback = DrawElement;
            OnInitReorderList(ViewList);

            onGUIHandler = OnGUIHandler;
        }

        protected virtual void OnGUIHandler() {
            view = EditorGUILayout.BeginScrollView(view);
            EditorGUILayout.BeginHorizontal();
            ViewList.DoLayoutList();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        protected virtual void OnInitReorderList(ReorderableList list) {}
        protected abstract void DrawElement(Rect rect, int index, bool isActive, bool isFocused);
        protected abstract void DrawHeader(Rect rect);
    }

    /// <summary>
    /// 资源ReorderList
    /// </summary>
    public class SourceElementView : ReorderableListElementView {
        private Action<ReorderableList, string> onElementFocused;
        private int focusIndex = -1;

        public SourceElementView(IList sources, Action<ReorderableList, string> onElementFocused) : base(sources, typeof(string)) {
            this.onElementFocused = onElementFocused;
        }

        protected override void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            var res = ViewList.list[index].ToString();
            GUILayout.BeginVertical();
            GUI.enabled = false;
            var asset = res.GetRelativeAssetsPath().LoadAssetAtPath<Object>();
            if (asset == null) {
                GUI.color = Color.red;
                EditorGUI.LabelField(rect, res);
                GUI.color = Color.white;
            } else {
                EditorGUI.ObjectField(rect, asset, typeof(Object), false);
            }

            GUI.enabled = true;

            GUILayout.EndVertical();

            if (isFocused && focusIndex != index) {
                focusIndex = index;
                onElementFocused?.Invoke(ViewList, res);
            }
        }

        protected override void DrawHeader(Rect rect) {
            GUI.Label(rect, $"Resources({ViewList.count})");
        }
    }

    /// <summary>
    /// 引用ReorderList
    /// </summary>
    public class ReferElementView : ReorderableListElementView {
        // title
        private string title;

        public ReferElementView(IList refers, string title) : base(refers, typeof(string)) {
            this.title = title;
        }

        protected override void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            var res = ViewList.list[index].ToString();
            GUILayout.BeginVertical();
            GUI.enabled = false;
            var asset = res.GetRelativeAssetsPath().LoadAssetAtPath<Object>();
            if (asset == null) {
                GUI.color = Color.red;
                EditorGUI.LabelField(rect, res);
                GUI.color = Color.white;
            } else {
                EditorGUI.ObjectField(rect, asset, typeof(Object), false);
            }

            GUI.enabled = true;

            GUILayout.EndVertical();
        }

        protected override void DrawHeader(Rect rect) {
            GUI.Label(rect, title);
        }
    }
    #endregion

    #region Preview Element => IMGUICointainer
    /// <summary>
    /// 预览窗口
    /// </summary>
    public class PreviewElementView : IMGUIContainer {
        public enum PreviewType {
            Inspector,
            Preview,
        }

        // 资产
        private Object asset;
        // 滑条
        private Vector2 view;

        public PreviewType previewType;

        public PreviewElementView() : base() { }

        /// <summary>
        /// 更新预览
        /// </summary>
        /// <param name="asset"></param>
        public void UpdatePreview(Object asset) {
            this.asset = asset;
            ShowPreview();
        }

        /// <summary>
        /// 切换面板
        /// </summary>
        /// <param name="type"></param>
        public void SwitchPreview(PreviewType type) {
            if (previewType == type)
                return;
            previewType = type;
            ShowPreview();
        }

        /// <summary>
        /// 实现面板
        /// </summary>
        private void ShowPreview() {
            if (asset == null) {
                onGUIHandler = null;
                return;
            }

            switch (previewType) {
                case PreviewType.Inspector:
                    var assetEditor = UnityEditor.Editor.CreateEditor(asset);
                    onGUIHandler = () => {
                        EditorGUILayout.LabelField($"Preview Type: {previewType}");
                        view = EditorGUILayout.BeginScrollView(view);
                        assetEditor.OnInspectorGUI();
                        EditorGUILayout.EndScrollView();
                    };
                break;
                case PreviewType.Preview:
                    var texture = AssetPreview.GetAssetPreview(asset);
                    var rect = new Rect(worldBound.x + worldBound.width / 2, worldBound.y + worldBound.height, worldBound.width, worldBound.height);
                    onGUIHandler = () => {
                        EditorGUILayout.LabelField($"Preview Type: {previewType}");
                        view = EditorGUILayout.BeginScrollView(view);
                        // FIXME: 图片不显示。
                        rect.Log();
                        if (texture != null)
                            EditorGUI.DrawPreviewTexture(rect, texture);
                        EditorGUILayout.EndScrollView();
                    };
                    
                break;
            }
        }
    }
    #endregion
}