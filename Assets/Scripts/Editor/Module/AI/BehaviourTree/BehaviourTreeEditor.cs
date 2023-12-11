using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;
using AKIRA.Behaviour.AI;
using System.Linq;
using System;

namespace AKIRA.Editor.AI {
    public class BehaviourTreeEditor : EditorWindow {
        private BehaviourTreeView treeView;
        private InspectorView inspectorView;
        private IMGUIContainer blackboardView;

        private SerializedObject treeObject;
        private SerializedProperty blackboardProperty;

        [MenuItem("Tools/AKIRA.Framework/Module/AI/BehaviourTreeEditor")]
        public static void OpenWindow()
        {
            BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviourTreeEditor");
            wnd.minSize = new Vector2(1080, 560);
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line) {
            if (Selection.activeObject is BehaviourTree) {
                OpenWindow();
                return true;
            }
            return false;
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            var name = typeof(BehaviourTreeEditor).Name;
            var location = name.GetScriptLocation().Replace(Application.dataPath, "Assets");

            // Import UXML
            // var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/AI_BehaviourTree/Editor/BehaviourTreeEditor.uxml");
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(location.Replace($"{name}.cs", $"UIBuilder/{name}.uxml"));
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            // var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/AI_BehaviourTree/Editor/BehaviourTreeEditor.uss");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(location.Replace($"{name}.cs", $"UIBuilder/{name}.uss"));
            root.styleSheets.Add(styleSheet);

            treeView = root.Q<BehaviourTreeView>();
            inspectorView = root.Q<InspectorView>();
            blackboardView = root.Q<IMGUIContainer>();
            blackboardView.onGUIHandler = () => {
                treeObject.Update();
                EditorGUILayout.PropertyField(blackboardProperty);
                treeObject.ApplyModifiedProperties();
            };

            treeView.onNodeSelected = OnNodeSelectionChanged;
            OnSelectionChange();
        }

        private void OnEnable() {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable() {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange change) {
            switch (change) {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        private void OnSelectionChange() {
            BehaviourTree tree = Selection.activeObject as BehaviourTree;
            if (!tree) {
                if (Selection.activeGameObject) {
                    AIAgent agent = Selection.activeGameObject.GetComponent<AIAgent>();
                    if (agent && agent.tree) {
                        tree = agent.tree;
                    }
                }
            }

            if (Application.isPlaying) {
                if (tree) {
                    treeView.PopulateView(tree);
                }
            } else {
                if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID())) {
                    treeView.PopulateView(tree);
                }
            }

            if (tree != null) {
                treeObject = new SerializedObject(tree);
                blackboardProperty = treeObject.FindProperty("blackboard");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        private void OnNodeSelectionChanged(NodeView view) {
            inspectorView.UpdateSelection(view);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnInspectorUpdate() {
            treeView?.UpdateNodeStates();
        }
    }
}