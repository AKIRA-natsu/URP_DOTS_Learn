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
        if (!File.Exists(target.Path)) {
            // IOException: Sharing violation on path 报错
            // https://blog.csdn.net/qq_42351033/article/details/88372506
            File.Create(target.Path).Dispose();
        }

        // JsonConvert.SerializeObject 会换行。。
        string json = JsonConvert.SerializeObject(target.Data, Formatting.Indented);
        // string json = JsonUtility.ToJson(target.Data);
        File.WriteAllText(target.Path, json, Encoding.UTF8);
        $"Json: {target} save".Log(GameData.Log.Success);
    }

    /// <summary>
    /// 读取
    /// </summary>
    /// <param name="target"></param>
    /// <typeparam name="T">类/结构体 类型</typeparam>
    /// <returns></returns>
    public static T Read<T>(this IJson target) {
        if (!File.Exists(target.Path)) {
            $"Json {target.Path} not found".Colorful(Color.cyan).Error();
            return default;
        }

        string json = File.ReadAllText(target.Path, Encoding.UTF8);
        return JsonUtility.FromJson<T>(json);
    }

    /// <summary>
    /// 读取并覆盖
    /// </summary>
    /// <param name="target"></param>
    /// <typeparam name="T"></typeparam>
    public static void ReadOverWrite(this IJson target) {
        if (!File.Exists(target.Path)) {
            $"Json {target.Path} not found".Colorful(Color.magenta).Error();
            return;
        }

        string json = File.ReadAllText(target.Path, Encoding.UTF8);
        JsonUtility.FromJsonOverwrite(json, target.Data);
    }
}