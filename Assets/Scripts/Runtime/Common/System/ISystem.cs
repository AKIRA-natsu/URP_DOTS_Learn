namespace AKIRA.Manager {
    /// <summary>
    /// System Base
    /// </summary>
    public interface ISystem {
        /// <summary>
        /// 初始化
        /// </summary>
        void Initialize();
    }

    /// <summary>
    /// Controller Base
    /// </summary>
    public interface IController : ISystem { }
}
