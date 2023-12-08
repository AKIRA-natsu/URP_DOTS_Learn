using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AKIRA;

/// <summary>
/// 反射相关的拓展
/// </summary>
public static class ReflectionHelp {
    // 程序集
    private static Dictionary<string, Assembly> assemblies = new();

    /// <summary>
    /// 获得程序集
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static Assembly GetAssembly(string name) {
        if (assemblies.ContainsKey(name)) {
            return assemblies[name];
        } else {
            var asm = Assembly.Load(name);
            assemblies[name] = asm;
            return asm;
        }
    }

    /// <summary>
    /// 不同程序集查找类型
    /// </summary>
    /// <param name="className"></param>
    /// <returns></returns>
    public static Type GetConfigTypeByAssembley(this string className, string dllName = GameData.DLL.Default) {
        var types = GetAssembly(dllName).GetTypes();
        foreach (var type in types) {
            if (type.Name.Equals(className)) {
                return type;
            }
        }
        return null;
    }

    /// <summary>
    /// 生成实例 object
    /// </summary>
    /// <param name="type"></param>
    /// <param name="dllName"></param>
    /// <returns></returns>
    public static object CreateInstance(this Type type, string dllName = GameData.DLL.Default) {
        return GetAssembly(dllName).CreateInstance(type.FullName);
    }

    /// <summary>
    /// 生成实例 T
    /// </summary>
    /// <param name="type"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T CreateInstance<T>(this Type type, string dllName = GameData.DLL.Default) {
        return (T)GetAssembly(dllName).CreateInstance(type.FullName);
    }

    /// <summary>
    /// 从整个程序集类中获得含有 <paramref name="T"/> 的类集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Type[] Handle<T>(string dllName = GameData.DLL.Default) {
        var types = GetAssembly(dllName).GetExportedTypes();
        return types.Where(type => {
            var attributes = Attribute.GetCustomAttributes(type, false);
            foreach (var attribute in attributes) {
                if (attribute is T)
                    return true;
            }
            return false;
        }).ToArray();
    }

    /// <summary>
    /// 反射 fieldinfo转换类型
    /// </summary>
    /// <param name="convertibleValue"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object ConvertTo(this object convertibleValue, Type type) {
        // 非空会包DNULL啥的报错
        if (string.IsNullOrEmpty(convertibleValue?.ToString()))
            return default;

        if (!type.IsGenericType) {
            if (type.IsEnum) {
                // Enum无法转过去,convertibleValue.ToString()值为Enum.ToString()
                return Enum.Parse(type, convertibleValue.ToString());
            } else {
                return Convert.ChangeType(convertibleValue, type);
            }
        } else {
            Type genericTypeDefinition = type.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(Nullable<>)) {
                return Convert.ChangeType(convertibleValue, Nullable.GetUnderlyingType(type));
            }
        }
        throw new InvalidCastException(string.Format("Invalid cast from type \"{0}\" to type \"{1}\".", convertibleValue.GetType().FullName, type.FullName));
    }
}