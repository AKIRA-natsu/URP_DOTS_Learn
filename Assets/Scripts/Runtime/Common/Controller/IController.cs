using System.Threading.Tasks;

namespace AKIRA {
    /// <summary>
    /// Controller Base
    /// </summary>
    public interface IController {
        /// <summary>
        /// 初始化
        /// </summary>
        Task Initialize();
    }
}