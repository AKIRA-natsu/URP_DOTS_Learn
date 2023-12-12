using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AKIRA.Editor.Git;

namespace AKIRA.Editor {
    public class ModuleDownloadWindow : EditorWindow {
        #region static show params
        private static GitObject GitObject;
        private static string ModuleName;
        private static Dictionary<string, bool> FoldMap = new Dictionary<string, bool>();
        private static Dictionary<string, bool> ContainMap = new Dictionary<string, bool>();

        private static ModuleDownloadConfig config;
        public static ModuleDownloadConfig Config {
            get {
                if (config == null)
                    config = GameConfig.Instance.GetConfig<ModuleDownloadConfig>();
                return config;
            }
        }
        #endregion

        #region self params
        private Vector2 view;
        #endregion

        public static void ShowWindow(string moduleName, GitObject git) {
            "獲得GitObject，打開Git Path選擇窗口".Log();
            ModuleName = moduleName;
            GitObject = git;

            // 文件夾收縮的bool值分佈
            FoldMap.Clear();
            ContainMap.Clear();
            InitWindow(GitObject);
            ShowWindow();
        }

        /// <summary>
        /// 初始化窗口
        /// </summary>
        /// <param name="git"></param>
        private static void InitWindow(GitObject git) {
            var items = git.payload.tree.items;
            foreach (var item in items) {
                ContainMap.Add(item.path, Config.IsPathInConfig(ModuleName, item.path));
                if (item.ContentType == TreeItem.ItemType.Directory) {
                    FoldMap.Add(item.path, false);
                } else {
                    continue;
                }
            }
            foreach (var child in git.children.Values)
                InitWindow(child);
        }

        // [MenuItem("Tools/AKIRA.Framework/Common/ModuleDownloadWindow")]
        private static void ShowWindow() {
            var window = GetWindow<ModuleDownloadWindow>();
            window.titleContent = new GUIContent("ModuleDownloadWindow");
            window.Show();
        }

        private void OnGUI() {
            if (string.IsNullOrEmpty(ModuleName))
                return;
            
            try {
                view = EditorGUILayout.BeginScrollView(view);
                EditorGUILayout.BeginVertical("framebox");

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField($"Module Assets: {ModuleName}");
                EditorGUILayout.HelpBox("選擇要下載的目錄", MessageType.Info);
                EditorGUILayout.EndVertical();
                
                DrawGitElement(GitObject);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            } catch (System.NullReferenceException e) {
                $"發生錯誤，關閉窗口：{e}".Error();
                Close();
            }
        }

        /// <summary>
        /// 繪製元素
        /// </summary>
        /// <param name="git"></param>
        private void DrawGitElement(GitObject git) {
            EditorGUILayout.BeginVertical("box");
            var items = git.payload.tree.items;
            foreach (var item in items) {
                // 跳過meta文件
                var path = item.path;
                if (path.Contains(".meta"))
                    continue;

                EditorGUILayout.BeginHorizontal("box");

                EditorGUI.BeginChangeCheck();
                ContainMap[path] = EditorGUILayout.Toggle(ContainMap[path], GUILayout.Width(20f));
                // 檢查鍵入path數組
                if (EditorGUI.EndChangeCheck()) {
                    if (ContainMap[path]) {
                        AddPath(item, item.ContentType == TreeItem.ItemType.Directory ? git.children[path] : null);
                    } else {
                        RemovePath(item, item.ContentType == TreeItem.ItemType.Directory ? git.children[path] : null);
                    }
                }
                    
                if (item.ContentType == TreeItem.ItemType.Directory) {
                    FoldMap[path] = EditorGUILayout.Foldout(FoldMap[path], item.name);
                    if (FoldMap[path])
                        DrawGitElement(git.children[path]);
                } else {
                    EditorGUILayout.LabelField(item.name);
                }

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 添加路徑
        /// </summary>
        /// <param name="item"></param>
        /// <param name="child"></param>
        private void AddPath(TreeItem item, GitObject child) {
            Config.AddPath(ModuleName, item.path);
            ContainMap[item.path] = true;
            if (item.ContentType == TreeItem.ItemType.Directory) {
                // 額外添加子節點
                var items = child.payload.tree.items;
                foreach (var i in items) {
                    if (i.path.Contains(".meta") || ContainMap[i.path])
                        continue;
                    AddPath(i, i.ContentType == TreeItem.ItemType.Directory ? child.children[i.path] : null);
                }
            }
        }

        /// <summary>
        /// 添加路徑
        /// </summary>
        /// <param name="item"></param>
        /// <param name="child"></param>
        private void RemovePath(TreeItem item, GitObject child) {
            Config.RemovePath(ModuleName, item.path);
            ContainMap[item.path] = false;
            if (item.ContentType == TreeItem.ItemType.Directory) {
                // 額外添加子節點
                var items = child.payload.tree.items;
                foreach (var i in items) {
                    if (i.path.Contains(".meta") || !ContainMap[i.path])
                        continue;
                    RemovePath(i, i.ContentType == TreeItem.ItemType.Directory ? child.children[i.path] : null);
                }
            }
        }
    }
}
