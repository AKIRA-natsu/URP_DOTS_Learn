using System;
using System.Text;
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
        private string[] extensions = new string[] {
            ".cs", ".meta", ".asset", ".prefab"
        };

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

            // 检查项目是否包含文件夹
            if (pathsProp.arraySize != 0 && GUILayout.Button("Recheck Load"))
                isLoadedProp.boolValue = CheckModuleLoad(pathsProp);
            
            if (isLoadedProp.boolValue) {
                if (GUILayout.Button("Delete Module"))
                    isLoadedProp.boolValue = DeleteModule(pathsProp);
            } else {
                if (pathsProp.arraySize != 0 && GUILayout.Button("Load Module"))
                    DownloadModule(pathsProp, isLoadedProp);
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

        /// <summary>
        /// 检查是否已经加载过
        /// </summary>
        private bool CheckModuleLoad(SerializedProperty pathProp) {
            var count = pathProp.arraySize;
            var existCount = 0;
            for (int i = 0; i < count; i++) {
                var productPath = GetProductPath(pathProp.GetArrayElementAtIndex(i).stringValue);

                if (File.Exists(productPath) || Directory.Exists(productPath))
                    existCount++;
                else
                    $"File Not Found：{productPath}".Log(GameData.Log.Warn);
            }
            
            $"Check Finish".Log(GameData.Log.Success);
            return existCount == count;
        }

        /// <summary>
        /// 下载模块
        /// </summary>
        /// <param name="pathProp"></param>
        /// <returns></returns>
        private async void DownloadModule(SerializedProperty pathProp, SerializedProperty property) {
            try {
                var count = pathProp.arraySize;
                for (int i = 0; i < count; i++) {
                    var gitPath = pathProp.GetArrayElementAtIndex(i).stringValue;

                    if (!CheckEffectivePath(gitPath))
                        continue;

                    var productPath = GetProductPath(gitPath);
                    if (File.Exists(productPath))
                        continue;
                    
                    $"Download File：{gitPath}".Log();
                    var data = JsonUtility.FromJson<GitObject>(await GetRequest(gitPath));

                    var lastIndex = productPath.LastIndexOf('/') + 1;
                    var name = productPath.Remove(0, lastIndex);

                    string folderPath = Path.GetDirectoryName(productPath); // 获取文件夹路径

                    // 检查文件夹是否存在，如果不存在则创建
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string text = await GetRequest(data.payload.blob.displayUrl);
                    byte[] bytes = Encoding.UTF8.GetBytes(text);
                    using (var stream = File.Open(productPath, FileMode.OpenOrCreate)) {
                        stream.Write(bytes);
                        await stream.DisposeAsync();
                    }
                }
                property.boolValue = true;
                AssetDatabase.Refresh();
            } catch (Exception e) {
                $"Download Error: {e}".Error();
            }
        }

        /// <summary>
        /// 判断有效后缀
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool CheckEffectivePath(string path) {
            foreach (var extension in extensions)
                if (path.Contains(extension))
                    return true;
                
            return false;
        }

        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="pathProp"></param>
        /// <returns></returns>
        private bool DeleteModule(SerializedProperty pathProp) {
            var count = pathProp.arraySize;
            for (int i = 0; i < count; i++) {
                var gitPath = pathProp.GetArrayElementAtIndex(i).stringValue;
                var productPath = GetProductPath(gitPath);
                if (!File.Exists(productPath))
                    continue;
                
                $"Delete File: {productPath}".Log();
                File.Delete(productPath);
            }
            AssetDatabase.Refresh();
            return false;
        }

        /// <summary>
        /// 获得文件在项目路径
        /// </summary>
        /// <param name="gitPath"></param>
        /// <returns></returns>
        private string GetProductPath(string gitPath) {
            return Path.Combine(Application.dataPath, gitPath.Split("Assets/")[1]);
        }

    }
}