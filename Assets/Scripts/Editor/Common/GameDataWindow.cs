using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace AKIRA.Editor {
    public class GameDataWindow : EditorWindow {
        // 类名
        private string[] classNames;
        // 选择的键值
        private int index;
        private Vector2 view;
        // 字段名称
        private string addFieldStr;
        // 添加值
        private string addValueStr;
        // 添加注释
        private string addSummaryStr;
        // 脚本路径
        private string path;

        [MenuItem("Tools/AKIRA.Framework/Common/GameDataWindow")]
        private static void ShowWindow() {
            var window = GetWindow<GameDataWindow>();
            window.titleContent = new GUIContent("GameDataWindow");
            window.minSize = new Vector2(560, 300);
            window.Show();
        }

        private void OnEnable() {
            path = typeof(GameData).Name.GetScriptLocation();
            var types = typeof(GameData).GetNestedTypes();
            classNames = new string[types.Length];
            for (int i = 0; i < types.Length; i++)
                classNames[i] = types[i].Name;
        }

        private void OnGUI() {
            EditorGUILayout.BeginVertical("framebox");
            index = EditorGUILayout.Popup("Class Name", index, classNames);
            if (GUILayout.Button("Edit Script Directly")) {
                System.Diagnostics.Process.Start(path);
            }
            EditorGUILayout.EndVertical();

            view = EditorGUILayout.BeginScrollView(view);
            EditorGUILayout.BeginVertical("framebox");
            DrawDataProperty();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 绘制内部元素
        /// </summary>
        private void DrawDataProperty() {
            var type = typeof(GameData).GetNestedTypes()[index];
            var values = type.GetFields();
            for (int i = 0; i < values.Length; i++) {
                var fieldName = values[i].Name;
                var name = values[i].GetRawConstantValue().ToString();
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField($"Field: {fieldName}   Value: {name}");
                if (GUILayout.Button("Edit"))
                    EditorValue(fieldName);
                GUI.color = Color.red;
                if (GUILayout.Button("Delete"))
                    RemoveDataValue(fieldName);
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("box");
            addFieldStr = EditorGUILayout.TextField("Field", addFieldStr);
            addValueStr = EditorGUILayout.TextField("Value", addValueStr);
            addSummaryStr = EditorGUILayout.TextField("Summary", addSummaryStr);
            if (!string.IsNullOrEmpty(addFieldStr) && !string.IsNullOrEmpty(addValueStr) && GUILayout.Button("Add")) {
                AddNewDataValue();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 添加新的值
        /// </summary>
        private void AddNewDataValue() {
            var lines = File.ReadAllLines(path).ToList();

            var fileClassIndex = lines.FindIndex(line => line.Contains($"public class {classNames[index]}"));
            // 寻找第一个 } 结尾
            var lastIndex = lines.FindIndex(fileClassIndex, lines.Count - fileClassIndex, line => line.Contains("}"));
            var findIndex = lines.FindIndex(fileClassIndex, lastIndex - fileClassIndex, line => line.Contains($"public const string {addFieldStr}"));
            if (findIndex != -1) {
                lines[findIndex - 2] = @$"            /// {addSummaryStr}";
                lines[findIndex] = @$"            public const string {addFieldStr} = ""{addValueStr}"";";
                $"GameData.{classNames[index]} 已经存在 {addFieldStr}，更新行 {findIndex}".Log(GameData.Log.Editor);
            } else {
                string content = 
@$"            /// <summary>
            /// {addSummaryStr}
            /// </summary>
            public const string {addFieldStr} = ""{addValueStr}"";";
                lines.Insert(lastIndex, content);

                $@"GameData脚本更新，插入行 {lastIndex + 1}: {addFieldStr} - {addValueStr}".Log(GameData.Log.Editor);

            }

            File.WriteAllLines(path, lines);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="name"></param>
        private void RemoveDataValue(string name) {
            var lines = File.ReadAllLines(path).ToList();
            var fileClassIndex = lines.FindIndex(5, lines.Count - 6, line => line.Contains($"public const string {name}"));
            lines.RemoveRange(fileClassIndex - 3, 4);
            File.WriteAllLines(path, lines);

            $"GameData脚本更新，删除行 {fileClassIndex - 3} - {fileClassIndex}：{name}".Log(GameData.Log.Editor);

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="name"></param>
        private void EditorValue(string name) {
            var lines = File.ReadAllLines(path).ToList();
            var fileClassIndex = lines.FindIndex(5, lines.Count - 6, line => line.Contains($"public const string {name}"));
            addFieldStr = name;
            addValueStr = lines[fileClassIndex].Split("=")[1].Trim(' ', ';', '"');
            addSummaryStr = lines[fileClassIndex - 2].Replace("///", "").Trim();
        }
    }
}