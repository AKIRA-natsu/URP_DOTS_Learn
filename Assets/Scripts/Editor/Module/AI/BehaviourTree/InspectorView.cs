using System;
using UnityEngine.UIElements;

namespace AKIRA.Editor.AI {
    public class InspectorView : VisualElement {

        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> {}
        
        private UnityEditor.Editor editor;

        public InspectorView() {

        }

        internal void UpdateSelection(NodeView view) {
            Clear();

            UnityEngine.Object.DestroyImmediate(editor);
            
            editor = UnityEditor.Editor.CreateEditor(view.node);
            IMGUIContainer container = new IMGUIContainer(() => {
                if (editor.target) {
                    editor.OnInspectorGUI();
                }
            });
            Add(container);
        }
    }
}