using System;
using System.Collections.Generic;
using System.Drawing;
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
        // output prop
        public Port OutputProp { get; private set; }

        public UINodeView(WinNode node, Action<UINodeView> onNodeSelected = null) : base() {
            this.title = node.Name;
            this.Node = node;
            this.onNodeSelected = onNodeSelected;

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

            if (!node.IsTopNode()) {
                OutputProp = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
                OutputProp.portName = "props";
                OutputProp.portColor = Color.DeepSkyBlue.ToUnityColor();
            }

            // elements
            if (!node.IsTopNode()) {
                PopupField<UITreeView.UINodeViewStyle> popupField = new(
                                new List<UITreeView.UINodeViewStyle>() { UITreeView.UINodeViewStyle.UI_Only, UITreeView.UINodeViewStyle.UI_With_Prop },
                                UITreeView.UINodeViewStyle.UI_Only,
                                OnSelectedValueChanged);
                mainContainer.Add(popupField);
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

        public override Port InstantiatePort(Orientation orientation, Direction direction, Port.Capacity capacity, Type type)
        {
            // enable FlowingEdge for ui node
            return Port.Create<FlowingEdge>(orientation, direction, capacity, type);
        }

        private string OnSelectedValueChanged(UITreeView.UINodeViewStyle style) {
            OnViewStyleChanged(style);
            return Enum.GetName(typeof(UITreeView.UINodeViewStyle), style).Replace("_", " ");
        }

        public void OnViewStyleChanged(UITreeView.UINodeViewStyle style) {
            if (style == UITreeView.UINodeViewStyle.UI_Only) {
                if (outputContainer.Contains(OutputProp))
                    outputContainer.Remove(OutputProp);
            } else {
                if (!outputContainer.Contains(OutputProp))
                    outputContainer.Add(OutputProp);
            }
        }
    }
}