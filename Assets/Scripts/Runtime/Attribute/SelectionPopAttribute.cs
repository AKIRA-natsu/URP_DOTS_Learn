using System.Reflection;
using System;
using UnityEngine;

namespace AKIRA.Attribute {
    /// <summary>
    /// <para>string 选择面板</para>
    /// <para>仅用于保存常量String的类</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class SelectionPopAttribute : PropertyAttribute {
        /// <summary>
        /// 类型
        /// </summary>
        /// <value></value>
        public Type type { get; protected set; }

        /// <summary>
        /// 类型中获得常量请注意！
        /// </summary>
        /// <param name="type"></param>
        public SelectionPopAttribute(Type type)
            => this.type = type;

        /// <summary>
        /// 类型中获得常量请注意！
        /// DLL为Unity默认DLL
        /// </summary>
        /// <param name="type"></param>
        public SelectionPopAttribute(string type, string dllName = GameData.DLL.Default)
            => this.type = Assembly.Load(dllName).GetType(type);
    }
}