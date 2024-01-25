using System;
using UnityEngine;

namespace AKIRA.Attribute {
    /// <summary>
    /// <para>string 路径面板</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class SelectionPathAttribute : PropertyAttribute {
        /// <summary>
        /// 面板变量替换名称
        /// </summary>
        /// <value></value>
        public string Name { get; private set; }

        /// <summary>
        /// 路径选择特性
        /// </summary>
        public SelectionPathAttribute() { }

        /// <summary>
        /// 路径选择特性
        /// </summary>
        /// <param name="name">面板变量替换名称</param>
        public SelectionPathAttribute(string name) => Name = name;
    }
}