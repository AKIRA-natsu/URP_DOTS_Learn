using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.Networking;

namespace AKIRA.Editor {
    [CustomPropertyDrawer(typeof(ModuleConfig))]
    public class ModuleConfigDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            // 绘制默认的面板
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.indentLevel = 0;

            // 绘制每个属性
            SerializedProperty moduleNameProp = property.FindPropertyRelative("moduleName");
            SerializedProperty gitPathProp = property.FindPropertyRelative("gitPath");
            SerializedProperty isLoadedProp = property.FindPropertyRelative("isLoaded");
            SerializedProperty pathsProp = property.FindPropertyRelative("paths");

            float lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            position.y += lineHeight;
            position.x -= position.width / 2;
            position.width *= 1.4f;

            // 绘制模块名称字段
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, lineHeight), moduleNameProp);
            position.y += lineHeight;

            EditorGUI.BeginChangeCheck();
            // 绘制Git路径字段
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, lineHeight), gitPathProp);
            position.y += lineHeight;
            if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(gitPathProp.stringValue)) {
                moduleNameProp.stringValue = gitPathProp.stringValue.Split('_').Last();
            }

            GUI.enabled = false;
            // 绘制是否已加载字段
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, lineHeight), isLoadedProp);
            position.y += lineHeight;
            GUI.enabled = true;

            // 绘制路径数组
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, lineHeight), pathsProp, true);
            
            if (!string.IsNullOrEmpty(gitPathProp.stringValue) && GUILayout.Button("Get Paths"))
                ConnectPath(gitPathProp.stringValue);
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            SerializedProperty pathsProp = property.FindPropertyRelative("paths");
            int arraySize = pathsProp.arraySize;
            float lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            return lineHeight * 4 + EditorGUI.GetPropertyHeight(pathsProp, true);
        }

        

        /// <summary>
        /// 
        /// </summary>
        public void ConnectPath(string path) {
            // 创建UnityWebRequest对象，并设置URL
            UnityWebRequest request = UnityWebRequest.Get(path);

            // 发送网络请求
            request.SendWebRequest();

            // 等待请求完成
            while (!request.isDone) {
                // 可以在这里添加一些加载中的提示
            }

            // 检查是否有错误
            if (request.result == UnityWebRequest.Result.ConnectionError) {
                Debug.LogError("Failed to connect to git path: " + request.error);
            }
            else {
                // 请求成功，获取响应的内容
                string response = request.downloadHandler.text;
                // 在这里解析响应的内容，根据需要处理资源
                Debug.Log("Response: " + response);
            }
        }
    }
}