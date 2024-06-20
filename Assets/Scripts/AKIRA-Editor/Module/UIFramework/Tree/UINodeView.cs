using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using AKIRA.UIFramework;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AKIRA.Editor {
    /// <summary>
    /// 节点视图
    /// </summary>
    public class UINodeView : Node {
        // window data
        public WinNode Node { get; private set; }
        // 选择事件
        private Action<UINodeView> onNodeSelected;

        // input
        public Port Input { get; private set; }
        // output
        public Port Output { get; private set; }

        public UINodeView(WinNode node, UITreeSetting.UINodeViewStyle viewStyle, Action<UINodeView> onNodeSelected = null) : base() {
            this.title = node.Name;
            this.Node = node;
            this.onNodeSelected = onNodeSelected;

            // add color line to differentiate node
            mainContainer.Insert(1, new VisualElement() {
                style = {
                    height = 3f,
                    backgroundColor = GetBackgroundColor().ToUnityColor(),
                }
            });

            // port
            if (!node.IsRootNode()) {
                Input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                Input.portName = "parent";
                Input.portColor = Color.Plum.ToUnityColor();
                inputContainer.Add(Input);
            }

            if (!node.IsLastNode()) {
                Output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
                Output.portName = "children";
                Output.portColor = Color.SkyBlue.ToUnityColor();
                outputContainer.Add(Output);
            }

            // elements
            if (!node.IsTopNode() && !node.IsLastNode()) {
                PopupField<UITreeSetting.UINodeViewStyle> popupField = new(
                                new List<UITreeSetting.UINodeViewStyle>() { UITreeSetting.UINodeViewStyle.UI_Only, UITreeSetting.UINodeViewStyle.UI_With_Prop },
                                viewStyle,
                                OnSelectedValueChanged);
                mainContainer.Add(popupField);
            }

            // get prefab value in editor
            if (node.self_Trans == null && node.IsWindowNode()) {
                var attribute = Node.self.GetType().GetCustomAttribute(typeof(WinAttribute)) as WinAttribute;
                var path = attribute.Data.path;
                node.self_Trans = path.LoadAssetAtPath<UnityEngine.GameObject>().transform;

                typeof(WinNode).GetMethod("GetReddots", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(node, null);
            }
        }

        public override void OnSelected() {
            base.OnSelected();
            onNodeSelected?.Invoke(this);
        }

        public override void OnUnselected() {
            base.OnUnselected();
            onNodeSelected?.Invoke(null);
        }

        private Color GetBackgroundColor() {
            if (Node.IsTopNode())
                return Color.Coral;

            if (Node.IsPropNode())
                return Color.LightPink;

            if (Node.IsWindowNode())
                return Color.LightSkyBlue;

            return Color.White;
        }

        public override Port InstantiatePort(Orientation orientation, Direction direction, Port.Capacity capacity, Type type)
        {
            // enable FlowingEdge for ui node
            return Port.Create<FlowingEdge>(orientation, direction, capacity, type);
        }

        private string OnSelectedValueChanged(UITreeSetting.UINodeViewStyle style) {
            OnViewStyleChanged(style);
            return Enum.GetName(typeof(UITreeSetting.UINodeViewStyle), style).Replace("_", " ");
        }

        public void OnViewStyleChanged(UITreeSetting.UINodeViewStyle style) {
            if (style == UITreeSetting.UINodeViewStyle.UI_Only) {
                
            } else {

            }
        }
    }
}