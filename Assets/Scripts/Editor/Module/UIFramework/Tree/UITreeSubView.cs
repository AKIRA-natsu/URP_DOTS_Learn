using System;
using System.Reflection;
using AKIRA.UIFramework;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AKIRA.Editor {
    /// <summary>
    /// 子视图
    /// </summary>
    public class UITreeSubView : VisualElement {
        // sub view element
        private Label titleLable;               // title
        private ObjectField objectField;        // object field
        private IMGUIContainer rectGUI;         // recttransform gui container
        private UnityEditor.Editor rectEditor;  // recttrasnform editor inspector

        public UITreeSubView() : base() {
            style.width = 220f;
            // style.flexGrow = .3f;
            style.flexShrink = 1f;

            Add(titleLable = new Label());
            Add(objectField = new ObjectField() { objectType = typeof(Transform) });
            Add(rectGUI = new IMGUIContainer() { style = { flexShrink = 1f } } );

            SetTitle();
            rectGUI.onGUIHandler = OnRectTransformHandler;
        }

        // clear element values
        private void ClearElementValues() {
            SetTitle();
            objectField.value = null;
            rectEditor = null;
        }

        /// <summary>
        /// 设置标题
        /// </summary>
        private void SetTitle() {
            titleLable.text = Application.isPlaying ? "Inspector For Hierarchy" : "Inspector For Prefab Origin";
        }

        // recttransform inspector
        private void OnRectTransformHandler() {
            if (rectEditor == null)
                return;
            
            EditorGUILayout.BeginVertical("box");
            rectEditor.OnInspectorGUI();
            EditorGUILayout.EndVertical();
        }

        // select node and update view
        public void UpdateView(UINodeView view) {
            // editor下，非运行self_trans是空的
            if (view == null) {
                ClearElementValues();
                return;
            }

            // title
            SetTitle();

            // object
            var objectValue = view.Node.self_Trans;
            if (objectValue == null) {
                // 改成查找的方式
                var attribute = view.Node.self.GetType().GetCustomAttribute(typeof(WinAttribute)) as WinAttribute;
                var path = attribute.Data.path;
                objectValue = path.LoadAssetAtPath<GameObject>().transform;
            }
            objectField.value = objectValue;

            // create editor
            rectEditor = UnityEditor.Editor.CreateEditor(objectValue);
        }
    }
}