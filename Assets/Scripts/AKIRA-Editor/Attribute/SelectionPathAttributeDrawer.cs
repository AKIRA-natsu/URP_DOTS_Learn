using UnityEngine;
using UnityEditor;

namespace AKIRA.Attribute.Editor {
    [CustomPropertyDrawer(typeof(SelectionPathAttribute))]
    public class SelectionPathAttributeDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType == SerializedPropertyType.String) {
                var name = (attribute as SelectionPathAttribute).Name;
                if (string.IsNullOrEmpty(name))
                    name = property.name;
                var newName = name.Substring(0, 1).ToUpper() + name.Substring(1);

                // rect settings
                var lablePosition = position;
                lablePosition.width = 100f;
                var valuePosition = position;
                valuePosition.x = 100f;
                valuePosition.width = position.width - 100f - 60f - 3f;
                var btnPosition = position;
                btnPosition.width = 60f;
                btnPosition.x = position.width - btnPosition.width;

                // property drawer
                EditorGUI.LabelField(lablePosition, newName);
                var value = property.stringValue;
                EditorGUI.TextField(valuePosition, value);
                if (GUI.Button(btnPosition, "Choose")) {
                    // 来源: https://forum.unity.com/threads/editorutility-openfilepanel-causes-error-log-for-endlayoutgroup.1389873/
                    EditorApplication.delayCall += () => {
                        value = EditorUtility.OpenFolderPanel($"Choose {newName} Path", string.IsNullOrEmpty(value) ? Application.dataPath : value, "");
                        if (string.IsNullOrEmpty(value))
                            return;
                        if (value.Contains(Application.dataPath))
                            value = value.GetRelativeAssetsPath();
                        property.stringValue = value;
                        property.serializedObject.ApplyModifiedProperties();
                        property.serializedObject.SetIsDifferentCacheDirty();
                        AssetDatabase.SaveAssets();
                    };
                }
            } else {
                EditorGUI.HelpBox(position, $"{typeof(SelectionPathAttribute)} is only support for {typeof(string)}", MessageType.Warning);
            }
        }
    }
}