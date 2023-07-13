using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.Networking;
using System.Threading.Tasks;
using AKIRA.Editor.Git;

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
            var path = gitPathProp.stringValue;
            if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(path)) {
                moduleNameProp.stringValue = path.Split('_').Last();
            }

            GUI.enabled = false;
            // 绘制是否已加载字段
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, lineHeight), isLoadedProp);
            position.y += lineHeight;
            GUI.enabled = true;

            // 绘制路径数组
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, lineHeight), pathsProp, true);
            
            // 获得路径数组，git url + cmd 获取
            if (!string.IsNullOrEmpty(path) && GUILayout.Button("Get Paths"))
                GetGitFileList(moduleNameProp.stringValue, path);
            
            if (isLoadedProp.boolValue) {
                if (GUILayout.Button("Delete Module")) {
                    "卸载模块".Log(GameData.Log.Editor);
                }
            } else {
                if (GUILayout.Button("Load Module")) {
                    // for (int i = 0; i < pathsProp.arraySize; i++)
                    //     DownloadModule(pathsProp.GetArrayElementAtIndex(i).stringValue);
                }
            }
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            SerializedProperty pathsProp = property.FindPropertyRelative("paths");
            int arraySize = pathsProp.arraySize;
            float lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            return lineHeight * 4 + EditorGUI.GetPropertyHeight(pathsProp, true);
        }

        /// <summary>
        /// 获得url路径
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="downloadUrl"></param>
        /// <returns></returns>
        public async void GetGitFileList(string moduleName, string downloadUrl) {
            string html = await GetRequest(downloadUrl);

            // 找到Assets
            var assetsLine = html.Split('\n').SingleOrDefault(line => line.Contains(downloadUrl.Replace(ModuleDownloadConfig.GitHubURL, "")) && line.Contains("Assets"));
            var splitStartIndex = assetsLine.IndexOf("href=\"") + 6;
            var splitLastIndex = assetsLine.IndexOf('\"', splitStartIndex);
            var assetsUrl = $"{ModuleDownloadConfig.GitHubURL}{assetsLine.Substring(splitStartIndex, splitLastIndex - splitStartIndex)}";
            $"Assets URL => {assetsUrl}".Log(GameData.Log.Editor);

            GitObject gitObject = JsonUtility.FromJson<GitObject>(await GetRequest(assetsUrl));

            // 遍歷獲得子節點
            gitObject = await GetChildren(gitObject, assetsUrl.Replace("/Assets", ""));

            ModuleDownloadWindow.ShowWindow(moduleName, gitObject);
        }

        /// <summary>
        /// 獲得子節點
        /// </summary>
        /// <param name="git"></param>
        /// <param name="assetsPath"></param>
        /// <returns></returns>
        private async Task<GitObject> GetChildren(GitObject git, string assetsPath) {
            var items = git.payload.tree.items;
            foreach (var item in items) {
                if (item.ContentType == TreeItem.ItemType.File)
                    continue;
                var path = Path.Combine(assetsPath, item.path);
                GitObject child = JsonUtility.FromJson<GitObject>(await GetRequest(path));
                git.children.Add(item.path, child);
                $"{item.path} 添加子節點 {path}".Log();
                child = await GetChildren(child, assetsPath);
            }
            return git;
        }

        /// <summary>
        /// 获得地址数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private async Task<string> GetRequest(string path) {
            UnityWebRequest webRequest = UnityWebRequest.Get(path);
            var asyncOperation = webRequest.SendWebRequest();

            while (!asyncOperation.isDone) {
                await Task.Yield();
            }

            if (webRequest.result != UnityWebRequest.Result.Success) {
                $"Request {path} Error: {webRequest.error}".Error();
                return default;
            }

            return webRequest.downloadHandler.text;
        }
    }
}