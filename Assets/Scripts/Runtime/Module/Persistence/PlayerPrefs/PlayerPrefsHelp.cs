using UnityEngine;

/// <summary>
/// PlayerPrefs 本地存储
/// </summary>
public static class PlayerPrefsHelp {
    /// <summary>
    /// PlayerPrefs save
    /// </summary>
    /// <param name="key"></param>
    /// <param name="data"></param>
    public static void Save(this string key, int data) {
        PlayerPrefs.SetInt(key, data);
    }

    /// <summary>
    /// PlayerPrefs save
    /// </summary>
    /// <param name="key"></param>
    /// <param name="data"></param>
    public static void Save(this string key, string data) {
        PlayerPrefs.SetString(key, data);
    }

    /// <summary>
    /// PlayerPrefs save
    /// </summary>
    /// <param name="key"></param>
    /// <param name="data"></param>
    public static void Save(this string key, float data) {
        PlayerPrefs.SetFloat(key, data);
    }

    /// <summary>
    /// default return 1
    /// </summary>
    /// <param name="key"></param>
    /// <param name="default">默认返回</param>
    /// <returns></returns>
    public static int GetInt(this string key, int @default = 0) {
        return PlayerPrefs.GetInt(key, @default);
    }

    /// <summary>
    /// default return null
    /// </summary>
    /// <param name="key"></param>
    /// <param name="default">默认返回</param>
    /// <returns></returns>
    public static string GetString(this string key, string @default = "") {
        return PlayerPrefs.GetString(key, @default);
    }

    /// <summary>
    /// default return 0
    /// </summary>
    /// <param name="key"></param>
    /// <param name="default">默认返回</param>
    /// <returns></returns>
    public static float GetFloat(this string key, float @default = 0) {
        return PlayerPrefs.GetFloat(key, @default);
    }

    /// <summary>
    /// 是否存在保存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool Exist(this string key) {
        return PlayerPrefs.HasKey(key);
    }

    /// <summary>
    /// 手动保存
    /// </summary>
    public static void Save() {
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="key"></param>
    public static void Delete(this string key) {
        PlayerPrefs.DeleteKey(key);
    }

    /// <summary>
    /// 删除全部
    /// </summary>
    public static void Delete() {
        PlayerPrefs.DeleteAll();
    }
}