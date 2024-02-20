using System;
using UnityEngine;

namespace AKIRA.Attribute {
    /// <summary>
    /// <para>引用选择</para>
    /// <para>针对接口类型</para>
    /// <para>对象需要有无参构造函数</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ReferenceSelectorAttribute : PropertyAttribute {
        /// <summary>
        /// <para>引用选择</para>
        /// <para>针对接口类型</para>
        /// <para>对象需要有无参构造函数</para>
        /// </summary>
        public ReferenceSelectorAttribute() { }
    }
}