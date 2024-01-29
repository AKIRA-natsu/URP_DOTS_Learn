using System;
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

        // main view setting
        private UITreeSetting setting;

        public UITreeSubView() : base() {
            style.width = 220f;
            // style.flexGrow = .3f;
            style.flexShrink = 1f;

            // read setting
            setting = UITreeSetting.ReadSetting();

            // setting element
            var settingContainer = new VisualElement();
            Add(settingContainer);
            settingContainer.Add(new Label("<b><size=15 />View Setting</b>".Colorful(System.Drawing.Color.LightSkyBlue)));
            var popField = new PopupField<UITreeSetting.UINodeViewStyle>("Node Style",
                                new() { UITreeSetting.UINodeViewStyle.UI_Only, UITreeSetting.UINodeViewStyle.UI_With_Prop },
                                setting.viewStyle,
                                OnSettingViewStyleChanged);
            popField.labelElement.style.minWidth = 70f;
            settingContainer.Add(popField);
            settingContainer.Add(new Button(setting.SaveSetting) { text = "Build" });

            // space
            Add(new VisualElement() { style = { height = 20f } });

            // other element
            Add(titleLable = new Label());
            Add(objectField = new ObjectField() { objectType = typeof(Transform) });
            Add(rectGUI = new IMGUIContainer() { style = { flexShrink = 1f } } );

            SetTitle();
            rectGUI.onGUIHandler = OnRectTransformHandler;
        }

        private string OnSettingViewStyleChanged(UITreeSetting.UINodeViewStyle style)
        {
            setting.viewStyle = style;
            return Enum.GetName(typeof(UITreeSetting.UINodeViewStyle), style);
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
            var text = Application.isPlaying ? "Inspector For Hierarchy" : "Inspector For Prefab Origin";
            titleLable.text = $"<b><size=15 />{text}</b>".Colorful(System.Drawing.Color.Orange);
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
            objectField.value = view.Node.self_Trans;

            // create editor
            rectEditor = UnityEditor.Editor.CreateEditor(objectField.value);
        }
    }
}