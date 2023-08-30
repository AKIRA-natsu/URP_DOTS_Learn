namespace AKIRA.UIFramework {
    /// <summary>
    /// UI 基类
    /// </summary>
    public abstract class UIBase {

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

        public abstract void Invoke(string name, params object[] args);
    }
}