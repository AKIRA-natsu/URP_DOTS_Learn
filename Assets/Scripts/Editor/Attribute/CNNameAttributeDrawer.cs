using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CNNameAttribute))]
public class CNNameAttributeDrawer: PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var cn = attribute as CNNameAttribute;

        if (cn.readOnly)
            GUI.enabled = false;
        
        EditorGUI.PropertyField(position, property, new GUIContent(cn.name));
        GUI.enabled = true;
    }
}