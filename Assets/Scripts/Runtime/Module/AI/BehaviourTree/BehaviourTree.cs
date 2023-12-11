using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AKIRA.Behaviour.AI {
    [CreateAssetMenu(fileName = "BehaviourTree", menuName = "AKIRA.Framework/Module/AIBehaviourTree", order = 0)]
    public class BehaviourTree : ScriptableObject {
        // root
        public Node rootNode;
        // state
        public Node.State treeState = Node.State.Running;
        // contains nodes
        public List<Node> nodes = new List<Node>();
        // 
        public Blackboard blackboard = new Blackboard();

        public Node.State Update() {
            if (rootNode.state == Node.State.Running) {
                treeState = rootNode.Update();
            }

            return treeState;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Create Node
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Node CreateNode(Type type, Vector2 position) {
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();
            node.position = position;
            node.OnCreate();

            Undo.RecordObject(this, "Behaviour Tree(Create node)");
            nodes.Add(node);

            if (!Application.isPlaying) {
                AssetDatabase.AddObjectToAsset(node, this);
            }
            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree(Create node)");

            AssetDatabase.SaveAssets();

            return node;
        }

        /// <summary>
        /// Delete Node
        /// </summary>
        /// <param name="node"></param>
        public void DeleteNode(Node node) {
            Undo.RecordObject(this, "Behaviour Tree(Delete node)");
            nodes.Remove(node);
            
            // AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void AddChild(Node parent, Node child) {
            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator) {
                Undo.RecordObject(decorator, "Behaviour Tree(Add Child)");
                decorator.child = child;
                EditorUtility.SetDirty(decorator);
            }

            RootNode rootNode = parent as RootNode;
            if (rootNode) {
                Undo.RecordObject(rootNode, "Behaviour Tree(Add Child)");
                rootNode.child = child;
                EditorUtility.SetDirty(rootNode);
            }

            CompositeNode composite = parent as CompositeNode;
            if (composite) {
                Undo.RecordObject(composite, "Behaviour Tree(Add Child)");
                composite.children.Add(child);
                EditorUtility.SetDirty(composite);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void RemoveChild(Node parent, Node child) {
            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator) {
                Undo.RecordObject(decorator, "Behaviour Tree(Remove Child)");
                decorator.child = null;
                EditorUtility.SetDirty(decorator);
            }

            RootNode rootNode = parent as RootNode;
            if (rootNode) {
                Undo.RecordObject(rootNode, "Behaviour Tree(Remove Child)");
                rootNode.child = null;
                EditorUtility.SetDirty(rootNode);
            }

            CompositeNode composite = parent as CompositeNode;
            if (composite) {
                Undo.RecordObject(composite, "Behaviour Tree(Remove Child)");
                composite.children.Remove(child);
                EditorUtility.SetDirty(composite);
            }
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public List<Node> GetChildren(Node parent) {
            List<Node> children = new List<Node>();
            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator && decorator.child != null) {
                children.Add(decorator.child);
            }

            RootNode rootNode = parent as RootNode;
            if (rootNode && rootNode.child != null) {
                children.Add(rootNode.child);
            }

            CompositeNode composite = parent as CompositeNode;
            if (composite) {
                return composite.children;
            }

            return children;
        }

        public void Traverse(Node node, System.Action<Node> visiter) {
            if (node) {
                visiter.Invoke(node);
                var children = GetChildren(node);
                children.ForEach(node => Traverse(node, visiter));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BehaviourTree Clone() {
            BehaviourTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            tree.nodes = new List<Node>();
            Traverse(tree.rootNode, tree.nodes.Add);
            return tree;
        }

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="ai"></param>
        public void Bind(AIAgent ai) {
            Traverse(rootNode, node => {
                node.ai = ai;
                node.blackboard = this.blackboard;
            });
        }
    }
}