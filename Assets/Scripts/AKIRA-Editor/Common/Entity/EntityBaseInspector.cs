using UnityEditor;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace AKIRA.Editor {
    /// <summary>
    /// <para>draw by ui tool kit</para>
    /// <para>参考: https://qiita.com/su10/items/9d83b6b5c6ee43b08e4d</para>
    /// </summary>
    [CustomEditor(typeof(EntityBase), true, isFallback = true)]
    public class EntityBaseInspector : global::UnityEditor.Editor {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            var entity = target as EntityBase;
            var method = typeof(EntityBase).GetMethod("IsOverride", BindingFlags.NonPublic | BindingFlags.Instance);
            var res = (bool)method.Invoke(entity, null);

            // 如果没有重写GameUpdate就不绘制更新需要的两个参数
            if (res) {
                var updateElement = new VisualElement();
                container.Add(updateElement);

                updateElement.Add(new Label() { text = "<b><size=12>Update Inspector</size></b>" });
                updateElement.Add(new PropertyField(serializedObject.FindProperty("updateGroup")));
                updateElement.Add(new PropertyField(serializedObject.FindProperty("updateMode")));

                var space = new VisualElement();
                space.style.height = 10f;
                updateElement.Add(space);
            }

            InspectorElement.FillDefaultInspector(container, serializedObject, this);

            return container;
        }
    }
}