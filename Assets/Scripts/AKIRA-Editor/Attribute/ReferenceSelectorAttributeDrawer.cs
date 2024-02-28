using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;
using System;
using AKIRA.Editor;

namespace AKIRA.Attribute.Editor {
    /// <summary>
    /// 更强的序列化参考：https://github.com/mackysoft/Unity-SerializeReferenceExtensions
    /// </summary>
    [CustomPropertyDrawer(typeof(ReferenceSelectorAttribute))]
    public class ReferenceSelectorAttributeDrawer : PropertyDrawer {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement() {
                name = property.displayName,
                style = {
                    // flexDirection = FlexDirection.Row,
                    flexGrow = 1f,
                    flexShrink = 1f,
                }
            };

            // get dll and types
            var fullNames = property.managedReferenceFieldTypename.Split(" ");
            var dllName = fullNames[0];
            var types = fullNames[1].GetConfigTypeByInterface(dllName);

            if (types == default || types.Length == 0) {
                // add help box
                container.Add(new HelpBox($"{fullNames[1]} found no types", HelpBoxMessageType.Warning) { style = { flexGrow = 1f, flexShrink = 1f, } });
            } else {
                // try set default value
                // if (property.managedReferenceValue == null) {
                //     property.managedReferenceValue = types.First().CreateInstance();
                //     property.serializedObject.ApplyModifiedProperties();
                // }

                // parse to string values
                // add first value "Null"
                var values = new string[types.Length + 1];
                values[0] = "";
                types.Foreach((i, type) => values[i + 1] = type.Name);

                // container.Add(new Label(property.displayName) { style = { paddingTop = 2f, width = 100f } });
                // container.Add(new PopupField<string>(values.ToList(), property.managedReferenceValue?.GetType().Name ?? values[0], value => {
                //     // set reference value
                //     if (value.Equals("NULL")) {
                //         property.managedReferenceValue = null;
                //     } else {
                //         property.managedReferenceValue = types[Array.IndexOf(values, value) - 1].CreateInstance(dllName);
                //     }
                //     property.serializedObject.ApplyModifiedProperties();
                //     return value;
                // }) { style = { flexGrow = 1f, flexShrink = 1f, } });
            
                var dropFied = new DropdownField(property.displayName) { value = property.managedReferenceValue?.GetType().Name ?? "Null", style = { flexGrow = 1f, flexShrink = 1f, } };
                container.Add(dropFied);
                dropFied.RegisterCallback<ClickEvent>(@event => {
                    var field = new AdvanceDropField(fullNames[1], values, item => {
                        if (string.IsNullOrEmpty(item.fullName))
                            property.managedReferenceValue = null;
                        else
                            property.managedReferenceValue = item.fullName.GetConfigTypeByAssembley(dllName).CreateInstance(dllName);
                        property.serializedObject.ApplyModifiedProperties();
                        dropFied.value = property.managedReferenceValue?.GetType().Name ?? "Null";
                    });
                    field.Show(dropFied.worldBound);
                });
            }

            return container;
        }
    }
}