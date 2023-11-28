using System;

namespace AKIRA.Attribute {
    /// <summary>
    /// <para>系统自动生成标签</para>
    /// <para>实例化已经在UIManager, EventSystem, AssetSystem, ObjectPool, UpdateSystem之后</para>
    /// <para>具体看GameManager</para>
    /// <para>默认顺序 0</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class SystemLauncherAttribute : System.Attribute {
        /// <summary>
        /// 重要性
        /// </summary>
        /// <value></value>
        public int significance { get; protected set; }

        /// <summary>
        /// 系统自动生成标签
        /// </summary>
        /// <param name="type">重要性顺序，数字越小越提前实例化</param>
        public SystemLauncherAttribute(int significance)
            => this.significance = significance;

        /// <summary>
        /// 系统自动生成标签，默认顺序0
        /// </summary>
        public SystemLauncherAttribute()
            => this.significance = 0;
    }
}