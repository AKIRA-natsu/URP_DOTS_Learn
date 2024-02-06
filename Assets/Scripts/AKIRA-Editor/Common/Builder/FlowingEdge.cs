using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AKIRA.Editor {
    /// <summary>
    /// <para>流动连线</para>
    /// <para>来源：https://blog.csdn.net/qq_21397217/article/details/126490788</para>
    /// </summary>
    public class FlowingEdge : Edge {
        private bool isEnableFlow;
        public bool EnableFlow {
            get => isEnableFlow;
            set {
                if (isEnableFlow == value)
                    return;
                
                isEnableFlow = value;
                if (value) {
                    Add(flowImg);
                } else {
                    Remove(flowImg);
                }
            }
        }

        private float flowSize = 6f;
        public float FlowSize {
            get => flowSize;
            set {
                flowSize = value;
                flowImg.style.width = new Length(value, LengthUnit.Pixel);
                flowImg.style.height = new Length(value, LengthUnit.Pixel);
            }
        }

        public float FlowSpeed { get; set; } = 150f;

        private readonly Image flowImg;

        public FlowingEdge() {
            flowImg = new Image {
                name = "flow-image",
                style = {
                    width = new Length(flowSize, LengthUnit.Pixel),
                    height = new Length(flowSize, LengthUnit.Pixel),
                    borderTopLeftRadius = new Length(flowSize / 2, LengthUnit.Pixel),
                    borderTopRightRadius = new Length(flowSize / 2, LengthUnit.Pixel),
                    borderBottomLeftRadius = new Length(flowSize / 2, LengthUnit.Pixel),
                    borderBottomRightRadius = new Length(flowSize / 2, LengthUnit.Pixel),
                }
            };

            edgeControl.RegisterCallback<GeometryChangedEvent>(OnEdgeControlGeometryChanged);

            EnableFlow = true;
        }

        public override bool UpdateEdgeControl()
        {
            if (!base.UpdateEdgeControl())
                return false;
            
            UpdateFlow();
            return true;
        }

        #region Flow
        private float totalEdgeLength;
        private float passedEdgeLength;
        private int flowPhaseIndex;
        private double flowPhaseStartTime;
        private double flowPhaseDuration;
        private float currentPhaseLength;

        private void UpdateFlow() {
            if (!isEnableFlow)
                return;
            
            // position
            var posProgress = (EditorApplication.timeSinceStartup - flowPhaseStartTime) / flowPhaseDuration;
            var flowStartPoint = edgeControl.controlPoints[flowPhaseIndex];
            var flowEndPoint = edgeControl.controlPoints[flowPhaseIndex + 1];
            var flowPos = Vector2.Lerp(flowStartPoint, flowEndPoint, (float)posProgress);
            flowImg.transform.position = flowPos - Vector2.one * flowSize / 2;

            // color
            var colorProgress = (passedEdgeLength + currentPhaseLength * posProgress) / totalEdgeLength;
            var startColor = edgeControl.outputColor;
            var endColor = edgeControl.inputColor;
            var flowColor = Color.Lerp(startColor, endColor, (float)colorProgress);
            flowImg.style.backgroundColor = flowColor;

            // enter next phase
            if (posProgress >= 0.99999f) {
                passedEdgeLength += currentPhaseLength;
                flowPhaseIndex++;

                // restart
                if (flowPhaseIndex >= edgeControl.controlPoints.Length - 1) {
                    flowPhaseIndex = 0;
                    passedEdgeLength = 0;
                }

                flowPhaseStartTime = EditorApplication.timeSinceStartup;
                currentPhaseLength = Vector2.Distance(edgeControl.controlPoints[flowPhaseIndex], edgeControl.controlPoints[flowPhaseIndex + 1]);
                flowPhaseDuration = currentPhaseLength / FlowSpeed;
            }
        }

        private void OnEdgeControlGeometryChanged(GeometryChangedEvent evt)
        {
            // restart
            flowPhaseIndex = 0;
            passedEdgeLength = 0;
            flowPhaseStartTime = EditorApplication.timeSinceStartup;
            currentPhaseLength = Vector2.Distance(edgeControl.controlPoints[flowPhaseIndex], edgeControl.controlPoints[flowPhaseIndex + 1]);
            flowPhaseDuration = currentPhaseLength / FlowSpeed;

            // calculate edge path length
            totalEdgeLength = 0;
            for (int i = 0; i < edgeControl.controlPoints.Length - 1; i++) {
                var p = edgeControl.controlPoints[i];
                var pNext = edgeControl.controlPoints[i + 1];
                var phaseLength = Vector2.Distance(p, pNext);
                totalEdgeLength += phaseLength;
            }
        }
        #endregion
    }
}