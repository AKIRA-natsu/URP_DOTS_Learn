using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// <para>双击打开日志文件所在位置</para>
/// <para>来源：http://www.cppblog.com/heath/archive/2016/06/21/213777.html</para>
/// </summary>
public class LogEditor {
    [UnityEditor.Callbacks.OnOpenAsset(0)]
    private static bool OnOpenAsset(int instanceID, int line) {
        string stackTrace = GetStackTrace();
        // 过滤标签
        if (!string.IsNullOrEmpty(stackTrace) && stackTrace.Contains("日志")) {
            Match matches = Regex.Match(stackTrace, @"\(at (.+)\)", RegexOptions.IgnoreCase);
            // 获得当前点击的对象文件路径
            string pathline = AssetDatabase.GetAssetPath(instanceID);
            if (pathline.Contains("LogSystem.cs")) {
                while (matches.Success) {
                    pathline = matches.Groups[1].Value;

                    // 过滤进入LogSystem的行
                    if (!pathline.Contains("LogSystem.cs")) {
                        int splitIndex = pathline.LastIndexOf(":");
                        string path = pathline[..splitIndex];
                        line = Convert.ToInt32(pathline[(splitIndex + 1)..]);
                        string fullPath = Application.dataPath[..Application.dataPath.LastIndexOf("Assets")];
                        fullPath = fullPath + path;
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath.Replace('/', '\\'), line);
                        break;
                    }
                    matches = matches.NextMatch();
                }
            } else {
                // 直接打开对应文件和对应列
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(pathline.Replace('/', '\\'), line);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获得日志输出信息
    /// </summary>
    /// <returns></returns>
    private static string GetStackTrace() {
        var consoleWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
        var fieldInfo = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
        var consoleWindowInstance = fieldInfo.GetValue(null);
        if (consoleWindowInstance == null)
            return default;
        if ((object)EditorWindow.focusedWindow != consoleWindowInstance)
            return default;
        // 
        var listViewStateType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ListViewState");
        fieldInfo = consoleWindowType.GetField("m_ListView", BindingFlags.Instance | BindingFlags.NonPublic);
        var listView = fieldInfo.GetValue(consoleWindowInstance);
        // 
        fieldInfo = listViewStateType.GetField("row", BindingFlags.Instance | BindingFlags.Public);
        var row = (int)fieldInfo.GetValue(listView);
        // 
        fieldInfo = consoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
        string activeText = fieldInfo.GetValue(consoleWindowInstance).ToString();
        return activeText;
    }
}