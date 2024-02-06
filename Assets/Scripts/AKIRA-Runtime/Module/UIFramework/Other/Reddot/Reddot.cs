using System.Collections.Generic;
using System.Linq;

namespace AKIRA.UIFramework {
    /// <summary>
    /// 红点作为 WinNode 的一部分
    /// </summary>
    public partial class WinNode {
        public List<ReddotComponent> Reddots { get; private set; } = new();

        /// <summary>
        /// 获得红点
        /// </summary>
        /// <returns></returns>
        private List<ReddotComponent> GetReddots() {
            if (self_Trans == null || IsTopNode())
                return Reddots; 
            var values = self_Trans.GetComponentsInChildren<ReddotComponent>()?.Where(value => value.Linker.Equals(Name));
            if (values == null)
                return Reddots;
            Reddots.AddRange(values);
            return Reddots;
        }
    }
}