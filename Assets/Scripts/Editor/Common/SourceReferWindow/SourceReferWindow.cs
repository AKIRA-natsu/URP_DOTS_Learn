using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using System.Linq;
using System;   
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AKIRA.Editor {
    public class SourceReferWindow : EditorWindow {
        // 去除后缀
        private readonly string[] withoutExtensions = {
            ".meta", ".asmdef", ".json", ".cs", ".dll", ".xml", ".p7s", ".txt", ".nupkg", ".unitypackage", ".psd", ".pdf", ".url"
        };

        // 视图
        private SourceViewElement sourceView;
        private SourceViewElement referView;
        private PreviewElementView preview;

        // replace element
        private ObjectField replaceField;

        // 是否正在忙碌
        private bool isbusying = false;

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
            previewMenu.menu.AppendAction("Inspector", a => preview.SwitchPreview(PreviewElementView.PreviewType.Inspector), a => preview.GetStatus(PreviewElementView.PreviewType.Inspector));
            previewMenu.menu.AppendAction("Preview", a => preview.SwitchPreview(PreviewElementView.PreviewType.Preview), a => preview.GetStatus(PreviewElementView.PreviewType.Preview));

            var refreshBtn = root.Q<ToolbarButton>("RefreshBtn");
            refreshBtn.clickable.clicked += RefreshResources;

            // Search Field
            var searchField = root.Q<ToolbarPopupSearchField>("SearchField");
            searchField.RegisterValueChangedCallback(OnSearchRes);

            // SourceView
            sourceView = new SourceViewElement(root.Q<VisualElement>("LeftPanel"), GetResources(), OnUpdateReferView);
            // ReferView save parent
            referView = new SourceViewElement(root.Q<VisualElement>("RightBottomPanel"));
            // Preview
            preview = new PreviewElementView(root.Q<VisualElement>("RightTopPanel"));
            // Replace Element
            replaceField = root.Q<ObjectField>("ReplaceField");
            root.Q<Button>("ReplaceBtn").clickable.clicked += OnReplaceRefer;
        }

        /// <summary>
        /// 替换引用
        /// </summary>
        private void OnReplaceRefer() {
            if (isbusying)
                return;

            var selectAsset = preview.asset;
            if (selectAsset == null) {
                $"未选择替换目标".Log(GameData.Log.Error);
                return;
            }

            var replaceAsset = replaceField.value;
            if (replaceAsset == null) {
                $"未选择替换对象".Log(GameData.Log.Error);
                return;
            }

            if (replaceAsset.GetType() != selectAsset.GetType()) {
                $"替换对象和替换目标的类型不一致".Log(GameData.Log.Error);
                return;
            }

            var oldGuid = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(selectAsset)).ToString();
            var newGuid = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(replaceAsset)).ToString();
            var refers = referView.Sources;

            if (refers.Count == 0) {
                $"替换完成，没有需要替换的资源".Log(GameData.Log.Success);
                return;
            }

            isbusying = true;
            var index = 0;
            EditorApplication.update = () => {
                var file = refers.ElementAt(index);
                bool isCancel = EditorUtility.DisplayCancelableProgressBar("Replace refer", file, (float)index / refers.Count);

                var content = File.ReadAllText(file);
                content = content.Replace(oldGuid, newGuid);
                File.WriteAllText(file, content);

                if (isCancel || ++index >= refers.Count) {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    isbusying = false;
                    AssetDatabase.Refresh();
                    $"替换完成".Log(GameData.Log.Success);

                    // 正常替换成功就是空的，，偷懒了
                    referView.FreshViewSources(default);
                }
            };
        }

        /// <summary>
        /// 更新引用视图
        /// </summary>
        /// <param name="list"></param>
        /// <param name="element"></param>
        private void OnUpdateReferView(VisualElement element) {
            if (isbusying)
                return;

            var list = sourceView.Sources;
            var res = element.name;

            var asset = res.GetRelativeAssetsPath().LoadAssetAtPath<Object>();
            // 防止重复疯狂迭代
            if (asset == preview.asset)
                return;

            // 更新预览
            preview.UpdatePreview(asset);

            // 序列化模式，可以手动unity设置，否则无法找到确实使用了资源的文件
            EditorSettings.serializationMode = SerializationMode.ForceText;
            string guid = AssetDatabase.AssetPathToGUID(res.GetRelativeAssetsPath());

            List<string> referFiles = new();
            int startIndex = 0;
            isbusying = true;
            EditorApplication.update = () => {
                var file = list.ElementAt(startIndex);
                bool isCancel = EditorUtility.DisplayCancelableProgressBar("Find Match Resources", file, (float)startIndex / list.Count);

                if (Regex.IsMatch(File.ReadAllText(file), guid))
                    referFiles.Add(file);

                if (isCancel || ++startIndex >= list.Count) {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    referView.FreshViewSources(referFiles.ToArray());
                    isbusying = false;
                }
            };
        }

        /// <summary>
        /// 模糊查询
        /// </summary>
        /// <param name="evt"></param>
        private void OnSearchRes(ChangeEvent<string> evt) {
            sourceView.UpdateView(evt.newValue?.ToLower());
        }

        /// <summary>
        /// 刷新资源
        /// 重新找找一遍资源塞到左侧列表
        /// </summary>
        private void RefreshResources() {
            sourceView.FreshViewSources(GetResources());
            referView.FreshViewSources(default);
            preview.UpdatePreview(null);
            replaceField.value = null;
        }

        /// <summary>
        /// 获得所有去除后缀的资源
        /// </summary>
        /// <returns></returns>
        private string[] GetResources() {
            return Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories).Where(s => !withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
        }
    }

    #region Scroll View Element => VisualElement
    /// <summary>
    /// 资源视图
    /// </summary>
    public class SourceViewElement : VisualElement {
        // ScrollView
        private ScrollView view;
        // 标签
        private Label label;
        // 资源列表
        private List<string> sources = new();
        public IReadOnlyCollection<string> Sources => sources;

        // 元素点击事件
        private Action<VisualElement> forcedEvent;

        public SourceViewElement(VisualElement parent, string[] sources = default, Action<VisualElement> forcedEvent = default) : base() {
            parent.Add(this);
            this.forcedEvent = forcedEvent;
            DrawView(parent);
            FreshViewSources(sources);
        }

        /// <summary>
        /// 更新元素
        /// </summary>
        /// <param name="sources"></param>
        public virtual void FreshViewSources(string[] sources) {
            if (sources == default) {
                this.sources = new();
            } else {
                this.sources = new List<string>(sources);
            }
            UpdateView(default);
        }

        /// <summary>
        /// 绘制大体面板
        /// </summary>
        /// <param name="parent"></param>
        protected virtual void DrawView(VisualElement parent) {
            var box = new Box();
            parent.Add(box);
            box.Add(label = new Label());
            box.Add(view = new ScrollView());
        }

        /// <summary>
        /// 更新面板
        /// </summary>
        /// <param name="match"></param>
        public void UpdateView(string match) {
            var result = new List<string>(sources);
            if (!string.IsNullOrEmpty(match)) {
                result = result.Where(res => Regex.IsMatch(res.Split("\\").Last().ToLower(), match)).ToList();
                label.text = $"<b>Search {match} Found in Project</b> : {result.Count}".Colorful(System.Drawing.Color.Orange);
            } else {
                label.text = $"<b>Found in Project</b> : {result.Count}".Colorful(System.Drawing.Color.Orange);
            }
            
            view.Clear();
            foreach (var res in result)
                view.Add(DrawElement(res));
        }

        /// <summary>
        /// 绘制元素
        /// </summary>
        protected virtual VisualElement DrawElement(string res) {
            var element = new VisualElement() {
                focusable = true,
                name = res,
            };
            element.AddToClassList("element");

            var asset = new ObjectField() { value = res.GetRelativeAssetsPath().LoadAssetAtPath<Object>(), objectType = typeof(UnityEngine.Object) };
            asset.SetEnabled(false);
            asset.AddToClassList("focusable");
            element.Add(asset);

            element.RegisterCallback<FocusEvent>(OnFocused);

            return element;
        }

        /// <summary>
        /// 焦点事件
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void OnFocused(FocusEvent evt) =>
            forcedEvent?.Invoke(evt.currentTarget as VisualElement);
    }
    #endregion

    #region Preview Element => IMGUICointainer
    /// <summary>
    /// 预览窗口
    /// </summary>
    public class PreviewElementView : VisualElement {
        public enum PreviewType {
            Inspector,
            Preview,
        }

        // 资产
        public Object asset { get; private set; }
        // 滑条
        private Vector2 view;

        public PreviewType previewType;
        private string key = "PreviewElementViewType";

        private Box box;
        private Label label;
        private Image image;
        private IMGUIContainer container;

        public PreviewElementView(VisualElement parent) : base() {
            parent.Add(this);
            previewType = key.EditorGetEnum(PreviewType.Inspector);

            parent.Add(box = new Box());
            box.Add(label = new Label());
            image = new Image();
            container = new IMGUIContainer();

            label.text = $"Preview Type: {previewType}";
        }

        /// <summary>
        /// 更新预览
        /// </summary>
        /// <param name="asset"></param>
        public void UpdatePreview(Object asset) {
            if (this.asset == asset)
                return;
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
            key.EditorSave(type);
            ShowPreview();
        }

        /// <summary>
        /// 实现面板
        /// </summary>
        private void ShowPreview() {
            label.text = $"Preview Type: {previewType}({asset?.name})";
            if (asset == null)
                return;

            switch (previewType) {
                case PreviewType.Inspector:
                    var assetEditor = UnityEditor.Editor.CreateEditor(asset);
                    if (box.Contains(image))
                        box.Remove(image);
                    if (!box.Contains(container))
                        box.Add(container);
                    container.onGUIHandler = () => {
                        view = EditorGUILayout.BeginScrollView(view);
                        assetEditor.OnInspectorGUI();
                        EditorGUILayout.EndScrollView();
                    };
                break;
                case PreviewType.Preview:
                    if (box.Contains(container))
                        box.Remove(container);
                    if (!box.Contains(image))
                        box.Add(image);
                    WaitToSetImage();
                break;
            }
        }

        /// <summary>
        /// 等待设置图片
        /// </summary>
        /// <returns></returns>
        private async void WaitToSetImage() {
            // 好像不会立刻生成图片，有一点延迟
            // 但又不是所有Object都有预览，所以只延迟一帧
            await Task.Yield();
            image.image = AssetPreview.GetAssetPreview(asset);
        }

        /// <summary>
        /// 获得下拉菜单状态
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public DropdownMenuAction.Status GetStatus(PreviewType targetType)
            => targetType == previewType ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal;
    }
    #endregion
}
