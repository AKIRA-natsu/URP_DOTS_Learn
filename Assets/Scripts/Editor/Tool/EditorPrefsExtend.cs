using System;
using UnityEditor;

internal static class EditorPrefsExtend {
    public static void EditorSave(this string key, int value) {
        EditorPrefs.SetInt(key, value);
    }

    public static void EditorSave(this string key, string value) {
        EditorPrefs.SetString(key, value);
    }

    public static void EditorSave(this string key, bool value) {
        EditorPrefs.SetBool(key, value);
    }

    public static void EditorSave(this string key, float value) {
        EditorPrefs.SetFloat(key, value);
    }

    public static void EditorSave(this string key, Enum value) {
        key.EditorSave(value.ToString());
    }

    public static int EditorGetInt(this string key, int @default = default) {
        return EditorPrefs.GetInt(key, @default);
    }

    public static string EditorGetString(this string key, string @default = default) {
        return EditorPrefs.GetString(key, @default);
    }

    public static bool EditorGetBool(this string key, bool @default = default) {
        return EditorPrefs.GetBool(key, @default);
    }

    public static float EditorGetFloat(this string key, float @default = default) {
        return EditorPrefs.GetFloat(key, @default);
    }

    public static T EditorGetEnum<T>(this string key, T @default = default) where T : Enum {
        var value = key.EditorGetString(@default?.ToString());
        return Enum.Parse<T>(value);
    }

    public static bool EditorExist(this string key) {
        return EditorPrefs.HasKey(key);
    }

    public static void EditorDelete(this string key) {
        EditorPrefs.DeleteKey(key);
    }

    [Obsolete("慎用")]
    public static void EditorDelete() {
        EditorPrefs.DeleteAll();
    }
}