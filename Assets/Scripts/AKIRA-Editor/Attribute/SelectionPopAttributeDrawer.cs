using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AKIRA.Editor;

namespace AKIRA.Attribute.Editor {
    [CustomPropertyDrawer(typeof(SelectionPopAttribute))]
    public class SelectionPopAttributeDrawer : PropertyDrawer {
        private bool extendY = false;

        private static Dictionary<Type, List<string>> caches = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType == SerializedPropertyType.String) {
                var selection = attribute as SelectionPopAttribute;
                var type = selection.type;
                var propertyValue = property.stringValue;

                // check cache
                List<string> names = default;
                if (!caches.ContainsKey(type)) {
                    var values = type.GetFields();
                    names = new() { "" };
                    for (int i = 0; i < values.Length; i++) {
                        var name = values[i].GetRawConstantValue().ToString();
                        if (string.IsNullOrEmpty(name))
                            continue;
                        names.Add(name);
                    }
                    caches[type] = names;
                } else {
                    names = caches[type];
                }

                extendY = names.Count == 0;
                if (extendY) {
                    Rect strPosition = new(position.x, position.y, position.width, position.height / 2);
                    property.stringValue = EditorGUI.TextField(strPosition, label, propertyValue);
                    Rect boxPosition = new(position.x, position.y + strPosition.height + 3f, position.width, position.height / 2 - 3f);
                    EditorGUI.HelpBox(boxPosition, $"No const value in {type}", MessageType.Info);
                } else {
                    var prefixLabel = EditorGUI.PrefixLabel(position, label);
                    if (EditorGUI.DropdownButton(prefixLabel, new(string.IsNullOrEmpty(propertyValue) ? "Null" : propertyValue), FocusType.Passive)) {
                        var field = new AdvanceDropField(type.FullName, names, item => {
                            property.serializedObject.Update();
                            property.stringValue = item.fullName;
                            property.serializedObject.ApplyModifiedProperties();
                        });
                        field.Show(position);
                    }
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