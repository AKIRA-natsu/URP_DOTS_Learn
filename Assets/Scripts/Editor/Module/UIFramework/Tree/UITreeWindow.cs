using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.EditorCoroutines.Editor;
using System.Collections;
using AKIRA.UIFramework;

namespace AKIRA.Editor {
    /// <summary>
    /// UI 树
    /// </summary>
    public class UITreeWindow : EditorWindow {
        // main tree view
        private UITreeView treeView;
        // sub tree view
        private UITreeSubView subView;

        [MenuItem("Tools/AKIRA.Framework/Module/UI/[Editor] UI Tree", priority = -10)]
        private static void ShowWindow() {
            var window = GetWindow<UITreeWindow>();
            window.titleContent = new GUIContent("UITreeWindow");
            window.minSize = new Vector2(1080, 560);
            window.Show();
        }

        public void CreateGUI() {
            // root
            VisualElement root = rootVisualElement;

            // // menu
            // var toolBar = new Toolbar();
            // toolBar.style.height = 20f;
            // toolBar.style.backgroundColor = new Color(0.1568628f, 0.1568628f, 0.1568628f);
            // root.Add(toolBar);

            // // bar items
            // toolBar.Add(new ToolbarButton(Build) { name = "build", text = "Build", tooltip = "build a ui tree view" });

            // container
            var container = new VisualElement() { name = "container" };
            container.style.flexDirection = FlexDirection.RowReverse;
            container.style.flexGrow = 1f;
            container.style.flexShrink = 1f;
            root.Add(container);

            // sub view
            container.Add(subView = new UITreeSubView() { name = "sub" });
            
            // main view
            container.Add(treeView = new UITreeView() { name = "main", onNodeSelected = subView.UpdateView });
        }

        private void OnEnable() {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
        
        private void OnDisable() {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            switch (change) {
                case PlayModeStateChange.EnteredEditMode:
                break;
                case PlayModeStateChange.ExitingEditMode:
                break;
                case PlayModeStateChange.EnteredPlayMode:
                    EditorCoroutineUtility.StartCoroutineOwnerless(ReBuild());
                break;
                case PlayModeStateChange.ExitingPlayMode:
                    Build();
                break;
            }
        }

        // wait uimanager build tree
        private IEnumerator ReBuild() {
            yield return new WaitUntil(() => !Application.isPlaying || UITree.IsFinishBuild);
            if (!Application.isPlaying)
                yield break;
            Build();
        }

        // build tree
        private void Build() {
            treeView.Build();
            subView.UpdateView(null);
        }
    }
}