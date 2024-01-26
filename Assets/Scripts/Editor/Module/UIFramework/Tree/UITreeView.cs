using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System;
using AKIRA.UIFramework;
using System.Linq;
using System.Reflection;

namespace AKIRA.Editor {
    /// <summary>
    /// UI 树视图
    /// </summary>
    public class UITreeView : GraphView {
        // callback
        public Action<UINodeView> onNodeSelected;

        // node view style
        public enum UINodeViewStyle {
            UI_Only,
            UI_With_Prop,
        }

        public new class UxmlFactory : UxmlFactory<UITreeView, UxmlTraits> {}

        public UITreeView() {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            // style
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("GridTreeEditor".GetFileLocation("uss"));
            styleSheets.Add(styleSheet);

            style.flexGrow = 1f;
            style.flexShrink = 1f;
        }

        public void Build() {
            // clear nodes and edges
            DeleteElements(nodes);
            DeleteElements(edges);

            // reset position and scale
            UpdateViewTransform(Vector3.zero, Vector3.one);

            // check ui tree build
            if (!UITree.IsFinishBuild) {
                List<UIComponent> uis = new();
                var wins = ReflectionHelp.Handle<WinAttribute>();
                Array.Sort(wins, (a, b) => a.GetCustomAttribute<WinAttribute>().Data.self - b.GetCustomAttribute<WinAttribute>().Data.self);
                foreach (var win in wins)
                    uis.Add(win.CreateInstance<UIComponent>());

                UI.Initialize(GameData.Asset.UIManager.LoadAssetAtPath<GameObject>());
                UITree.Build(uis);
                "UITree Build In Editor....".Log(GameData.Log.Editor);
            }

            // build
            Build(UITree.Root);
            BuildConnection();
        }

        // recursion for build nodes
        private void Build(WinNode node) {
            var view = new UINodeView(node, onNodeSelected);
            view.SetPosition(CalculatePosition(view.GetPosition(), node));
            AddElement(view);

            if (node.IsLastNode())
                return;

            foreach (var child in node.children)
                Build(child);
        }

        // make connections for each node
        private void BuildConnection() {
            // foreach node check parent
            foreach (UINodeView view in nodes.Cast<UINodeView>()) {
                var parentNodeView = FindParentNodeView(view);
                if (parentNodeView == null)
                    continue;
                var edge = new FlowingEdge() { input = view.Input, output = parentNodeView.Output };
                edge.input.Connect(edge);
                edge.output.Connect(edge);
                Add(edge);
            }
        }

        // find parent node in node list
        private UINodeView FindParentNodeView(UINodeView view) {
            if (view.Node.IsRootNode())
                return default;
            var views = nodes.Cast<UINodeView>();
            return views.Single(v => v.Node == view.Node.parent);
        }

        // calculate position for view
        private Rect CalculatePosition(Rect position, WinNode node) {
            position.x = node.GetDepth() * 200;
            position.y = UITree.GetSortingInTree(node) * 100;
            return position;
        }
    }
}