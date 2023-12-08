using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AKIRA.Attribute.Editor {
    [CustomPropertyDrawer(typeof(SelectionPopAttribute))]
    public class SelectionPopAttributeDrawer : PropertyDrawer {
        private bool extendY = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType == SerializedPropertyType.String) {
                var selection = attribute as SelectionPopAttribute;
                var type = selection.type;

                var strValue = property.stringValue;
                var values = type.GetFields();
                var index = 0;
                List<GUIContent> names = new List<GUIContent>();
                for (int i = 0; i < values.Length; i++) {
                    var name = values[i].GetRawConstantValue().ToString();
                    if (String.IsNullOrEmpty(name))
                        continue;
                    if (strValue.Equals(name))
                        index = i;
                    names.Add(new GUIContent(name));
                }

                extendY = names.Count == 0;
                if (extendY) {
                    Rect strPosition = new Rect(position.x, position.y, position.width, position.height / 2);
                    property.stringValue = EditorGUI.TextField(strPosition, label, property.stringValue);
                    Rect boxPosition = new Rect(position.x, position.y + strPosition.height + 3f, position.width, position.height / 2 - 3f);
                    EditorGUI.HelpBox(boxPosition, $"No const value in {type}", MessageType.Info);
                } else {
                    index = EditorGUI.Popup(position, label, index, names.ToArray());
                    property.stringValue = names[index].text;
                }
            } else {
                EditorGUI.HelpBox(position, $"{typeof(SelectionPopAttribute)} is only support for {typeof(string)}", MessageType.Warning);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return base.GetPropertyHeight(property, label) * (extendY ? 2.2f : 1);
        }
}
}