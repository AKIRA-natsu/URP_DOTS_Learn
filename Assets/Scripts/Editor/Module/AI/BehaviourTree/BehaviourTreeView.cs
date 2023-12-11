using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using AKIRA.Behaviour.AI;
using Node = AKIRA.Behaviour.AI.Node;

namespace AKIRA.Editor.AI {
    public class BehaviourTreeView : GraphView {

        private BehaviourTree tree;
        public Action<NodeView> onNodeSelected;

        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> {}

        public BehaviourTreeView() {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var name = typeof(BehaviourTreeEditor).Name;
            var location = name.GetScriptLocation().Replace(Application.dataPath, "Assets");

            // var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/AI_BehaviourTree/Editor/BehaviourTreeEditor.uss");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(location.Replace($"{name}.cs", $"UIBuilder/{name}.uss"));
            styleSheets.Add(styleSheet);

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnUndoRedo() {
            PopulateView(tree);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private NodeView FindNodeView(Node node) {
            return GetNodeByGuid(node.guid) as NodeView;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tree"></param>
        internal void PopulateView(BehaviourTree tree) {
            this.tree = tree;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            if (tree.rootNode == null) {
                tree.rootNode = tree.CreateNode(typeof(RootNode), Vector2.zero);
                EditorUtility.SetDirty(tree);
                AssetDatabase.SaveAssets();
            }

            // create nodes
            tree.nodes.ForEach(CreateNodeView);

            // create edges
            tree.nodes.ForEach(node => {
                var children = tree.GetChildren(node);
                NodeView parentView = FindNodeView(node);
                children.ForEach(child => {
                    NodeView childView = FindNodeView(child);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                });
            });
        }

        public override List<Port> GetCompatiblePorts(Port port, NodeAdapter adapter) {
            return ports.ToList().Where(endPort =>
                endPort.direction != port.direction && endPort.node != port.node).ToList();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange) {
            if (graphViewChange.elementsToRemove != null) {
                graphViewChange.elementsToRemove.ForEach(element => {
                    NodeView nodeView = element as NodeView;
                    if (nodeView != null) {
                        tree.DeleteNode(nodeView.node);
                    }

                    Edge edge = element as Edge;
                    if (edge != null) {
                        NodeView parentView = edge.output.node as NodeView;
                        NodeView childView = edge.input.node as NodeView;
                        tree.RemoveChild(parentView.node, childView.node);
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null) {
                graphViewChange.edgesToCreate.ForEach(edge => {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    tree.AddChild(parentView.node, childView.node);
                });
            }

            if (graphViewChange.movedElements != null) {
                nodes.ForEach(node => {
                    NodeView view = node as NodeView;
                    view.SortChildren();
                });
            }

            return graphViewChange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) {
            // base.BuildContextualMenu(evt);
            // 直接获得lockMousePosition return zero
            // 来源：https://forum.unity.com/threads/how-to-get-mouse-click-position-when-got-contextualmenupopulateevent.614032/
            Vector2 graphViewMousePosition = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);

            {
                var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
                foreach (var type in types) {
                    evt.menu.AppendAction($"[{type.BaseType.Name}]/{type.Name}", (a) => CreateNode(type, graphViewMousePosition));
                }
            }

            {
                var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
                foreach (var type in types) {
                    evt.menu.AppendAction($"[{type.BaseType.Name}]/{type.Name}", (a) => CreateNode(type, graphViewMousePosition));
                }
            }

            {
                var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
                foreach (var type in types) {
                    evt.menu.AppendAction($"[{type.BaseType.Name}]/{type.Name}", (a) => CreateNode(type, graphViewMousePosition));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        private void CreateNode(Type type, Vector2 position) {
            Node node = tree.CreateNode(type, position);
            CreateNodeView(node);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private void CreateNodeView(Node node) {
            NodeView nodeView = new NodeView(node);
            nodeView.onNodeSelected = onNodeSelected;
            AddElement(nodeView);
        }

        public void UpdateNodeStates() {
            nodes.ForEach(node => {
                NodeView view = node as NodeView;
                view.UpdateState();
            });
        }
    }
}
