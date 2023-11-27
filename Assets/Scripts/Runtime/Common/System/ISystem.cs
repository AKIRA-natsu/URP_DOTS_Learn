using System.Threading.Tasks;

namespace AKIRA.Manager {
    /// <summary>
    /// System Base
    /// </summary>
    public interface IController {
        /// <summary>
        /// 初始化
        /// </summary>
        Task Initialize();
    }

    /// <summary>
    /// Controller Base
    /// </summary>
    public interface ISystem : IController { }
}
