using System;

namespace AKIRA.UIFramework {
    /// <summary>
    /// UI 基类
    /// </summary>
    public abstract class UIBase : IDisposable {

        public UIBase() {}

        /// <summary>
        /// 唤醒
        /// </summary>
        public abstract void Awake(object obj);
        /// <summary>
        /// 进入
        /// </summary>
        protected virtual void OnEnter() {}
        /// <summary>
        /// 恢复
        /// </summary>
        public virtual void OnResume() {}
        /// <summary>
        /// 暂停
        /// </summary>
        public virtual void OnPause() {}
        /// <summary>
        /// 退出
        /// </summary>
        protected virtual void OnExit() {}

        /// <summary>
        /// 销毁
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// 默认反射唤醒方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        public abstract void Invoke(string name, params object[] args);
    }
}