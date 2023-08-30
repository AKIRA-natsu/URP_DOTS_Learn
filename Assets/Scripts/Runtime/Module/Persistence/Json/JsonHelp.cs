using System.Text;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using AKIRA;

/// <summary>
/// <para>Json存储</para>
/// <para>Application.streamingAssetsPath + "/JsonName.json"</para>
/// </summary>
public static class JsonHelp {
    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="target"></param>
    public static void Save(this IJson target) {
        var path = CombinePath(target.Path);
        if (!File.Exists(path)) {
            // IOException: Sharing violation on path 报错
            // https://blog.csdn.net/qq_42351033/article/details/88372506
            File.Create(path).Dispose();
        }

        // JsonConvert.SerializeObject 会换行。。
        string json = JsonConvert.SerializeObject(target.Data, Formatting.Indented);
        // string json = JsonUtility.ToJson(target.Data);
        File.WriteAllText(path, json, Encoding.UTF8);
        $"Json: {target} save".Log(GameData.Log.Success);
    }

    /// <summary>
    /// 读取
    /// </summary>
    /// <param name="target"></param>
    /// <typeparam name="T">类/结构体 类型</typeparam>
    /// <returns></returns>
    public static T Read<T>(this IJson target) {
        var path = CombinePath(target.Path);
        if (!File.Exists(path)) {
            $"Json {path} not found".Colorful(Color.cyan).Error();
            return default;
        }

        string json = File.ReadAllText(path, Encoding.UTF8);
        return JsonUtility.FromJson<T>(json);
    }

    /// <summary>
    /// 读取并覆盖
    /// </summary>
    /// <param name="target"></param>
    /// <typeparam name="T"></typeparam>
    public static void ReadOverWrite(this IJson target) {
        var path = CombinePath(target.Path);
        if (!File.Exists(path)) {
            $"Json {path} not found".Colorful(Color.magenta).Error();
            return;
        }

        string json = File.ReadAllText(path, Encoding.UTF8);
        JsonUtility.FromJsonOverwrite(json, target.Data);
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="target"></param>
    public static void Delete(this IJson target) {
        var path = CombinePath(target.Path);
        if (!File.Exists(path))
            return;
        
        File.Delete(path);
    }

    /// <summary>
    /// 获得路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static string CombinePath(string path) {
        #if UNITY_EDITOR
        return Path.Combine(Application.streamingAssetsPath, path);
        #else
        return Path.Combine(Application.persistentDataPath, path);
        #endif
    }
}