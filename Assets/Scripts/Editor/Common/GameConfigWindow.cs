using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AKIRA.Editor {
    /// <summary>
    /// 游戏配置窗口
    /// </summary>
    [CustomEditor(typeof(GameConfig))]
    public class GameConfigWindow : UnityEditor.Editor {
        private SerializedProperty editorProperty;
        private SerializedProperty runtimeProperty;

        // 是否展开
        private bool[] extends = new bool[2] { true, true };
        private int[] selectionIndexs = new int[2];
        // 面板
        private UnityEditor.Editor editor;

        // 获得ScriptObject类
        private List<Type> selections;
        private List<string> selectionNames;

        [MenuItem("Tools/AKIRA.Framework/Common/Select GameConfig")]
        private static void SelectConfig() {
            var paths = Directory.GetFiles(Path.Combine(Application.dataPath, "Resources"), "*.asset", SearchOption.AllDirectories);
            foreach (var path in paths) {
                var target = path.GetRelativeAssetsPath().LoadAssetAtPath<UnityEngine.Object>();
                if (target != null && target is GameConfig) {
                    Selection.activeObject = target;
                    break;
                }
            }
        }

        private void OnEnable() {
            editorProperty = serializedObject.FindProperty("editorConfigs");
            runtimeProperty = serializedObject.FindProperty("runtimeConfigs");
            editor = null;

            selections = new();
            var targetType = typeof(ScriptableObject);
            var editorType1 = typeof(EditorWindow);
            var editorType2 = typeof(UnityEditor.Editor);
            var dllFields = typeof(GameData.DLL).GetFields();
            foreach (var field in dllFields) {
                var value = field.GetRawConstantValue().ToString();
                // 剔除用宏括起来的 EditorWindow 和 Editor 类型
                var types = Assembly.Load(value).GetTypes()
                    .Where(type => type.IsSubclassOf(targetType) && !type.IsSubclassOf(editorType1) && !type.IsSubclassOf(editorType2));
                selections.AddRange(types);
            }
            // 剔除自身
            selections.Remove(target.GetType());

            selectionNames = new() { "--" };
            foreach (var selection in selections)
                selectionNames.Add(selection.Name);
        }

        private void OnDisable() {
            editor = null;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            DrawList(0, editorProperty, target => MoveToOtherProperty(runtimeProperty, target));
            DrawList(1, runtimeProperty, target => MoveToOtherProperty(editorProperty, target));
            serializedObject.ApplyModifiedProperties();
            
            if (editor == null)
                return;
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("framebox");
            editor.OnInspectorGUI();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 列表绘制
        /// </summary>
        /// <param name="index"></param>
        /// <param name="property"></param>
        /// <param name="onMoveToOtherProperty"></param>
        private void DrawList(int index, SerializedProperty property, Action<UnityEngine.Object> onMoveToOtherProperty) {
            EditorGUILayout.BeginVertical("framebox");
            EditorGUILayout.BeginHorizontal("box");
            var name = property.name;
            name = name.Replace(name.First().ToString(), name.First().ToString().ToUpper());
            // EditorGUILayout.LabelField($"{name}({property.arraySize})");
            extends[index] = EditorGUILayout.Foldout(extends[index], $"{name}({property.arraySize})");

            selectionIndexs[index] = EditorGUILayout.Popup(selectionIndexs[index], selectionNames.ToArray(), GUILayout.Width(100f));
            if (GUILayout.Button("+", GUILayout.Width(30f)) && selectionIndexs[index] != 0)
                CreateConfig(property, selections[selectionIndexs[index] - 1]);

            EditorGUILayout.EndHorizontal();

            if (extends[index]) {
                for (int i = 0; i < property.arraySize; i++) {
                    EditorGUILayout.BeginHorizontal();
                    var child = property.GetArrayElementAtIndex(i);
                    child.objectReferenceValue = EditorGUILayout.ObjectField(child.objectReferenceValue, typeof(ScriptableObject), false);
                    // 删除
                    if (GUILayout.Button("-", GUILayout.Width(30f)))
                        DeleteConfig(property, i--);
                    // 查看
                    if (editor != null && editor.target.GetType() == child.objectReferenceValue.GetType())
                        GUI.color = System.Drawing.Color.GreenYellow.ToUnityColor();
                    if (GUILayout.Button("~", GUILayout.Width(30f)))
                        editor = CreateEditor(child.objectReferenceValue);
                    GUI.color = Color.white;
                    // 移动
                    if (GUILayout.Button("->", GUILayout.Width(30f))) {
                        onMoveToOtherProperty.Invoke(child.objectReferenceValue);
                        property.DeleteArrayElementAtIndex(i--);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 创建配置
        /// </summary>
        /// <param name="property"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private ScriptableObject CreateConfig(SerializedProperty property, Type type) {
            for (int i = 0; i < editorProperty.arraySize; i++) {
                if (editorProperty.GetArrayElementAtIndex(i).objectReferenceValue.GetType().Equals(type)) {
                    $"已经在EditorConfig中加入了Config {type}".Log(GameData.Log.Warn);
                    return default;
                }
            }
            for (int i = 0; i < runtimeProperty.arraySize; i++) {
                if (runtimeProperty.GetArrayElementAtIndex(i).objectReferenceValue.GetType().Equals(type)) {
                    $"已经在RuntimeConfig加入了Config {type}".Log(GameData.Log.Warn);
                    return default;
                }
            }

            property.InsertArrayElementAtIndex(property.arraySize);

            ScriptableObject config = ScriptableObject.CreateInstance(type);
            config.name = type.Name;

            if (!Application.isPlaying)
                AssetDatabase.AddObjectToAsset(config, target);
            Undo.RegisterCreatedObjectUndo(config, "Behaviour Tree(Create node)");

            AssetDatabase.SaveAssets();
            property.GetArrayElementAtIndex(property.arraySize - 1).objectReferenceValue = config;
            return config;
        }

        /// <summary>
        /// 删除配置
        /// </summary>
        /// <param name="property"></param>
        /// <param name="index"></param>
        private void DeleteConfig(SerializedProperty property, int index) {
            var config = property.GetArrayElementAtIndex(index).objectReferenceValue;
            property.DeleteArrayElementAtIndex(index);
            if (editor != null && editor.target.GetType() == config.GetType())
                editor = null;

            AssetDatabase.RemoveObjectFromAsset(config);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 移动到另外一个Property
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="target"></param>
        private void MoveToOtherProperty(SerializedProperty property, UnityEngine.Object target) {
            property.InsertArrayElementAtIndex(property.arraySize);
            property.GetArrayElementAtIndex(property.arraySize - 1).objectReferenceValue = target;
        }
    }
}