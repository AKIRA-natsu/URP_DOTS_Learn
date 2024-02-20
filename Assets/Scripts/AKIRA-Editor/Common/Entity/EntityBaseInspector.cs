using UnityEditor;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

/// <summary>
/// <para>draw by ui tool kit</para>
/// <para>参考: https://qiita.com/su10/items/9d83b6b5c6ee43b08e4d</para>
/// </summary>
[CustomEditor(typeof(EntityBase), true, isFallback = true)]
public class EntityBaseInspector : Editor {
    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();

        // make inspector element upper
        InspectorElement.FillDefaultInspector(container, serializedObject, this);

        var entity = target as EntityBase;
        var method = typeof(EntityBase).GetMethod("IsOverride", BindingFlags.NonPublic | BindingFlags.Instance);
        var res = (bool)method.Invoke(entity, null);

        // 如果没有重写GameUpdate就不绘制更新需要的两个参数
        if (res) {
            var updateElement = new VisualElement() { name = "UpdateElement" } ;
            container.Add(updateElement);

            // space
            updateElement.Add(new VisualElement() { style = { height = 10f } });

            updateElement.Add(new Label() { text = "<b><size=12>Update Inspector</size></b>".Colorful(System.Drawing.Color.LightSkyBlue) });
            updateElement.Add(new PropertyField(serializedObject.FindProperty("updateGroup")));
            updateElement.Add(new PropertyField(serializedObject.FindProperty("updateMode")));
        }


        return container;
    }
}