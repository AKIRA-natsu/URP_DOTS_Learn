using UnityEngine;

namespace AKIRA.Manager {
    /// <summary>
    /// 调试窗口接口
    /// </summary>
    public interface IConsoleUI {
        /// <summary>
        /// 初始化，提供所有命令行
        /// </summary>
        /// <param name="commands"></param>
        void Init(Command[] commands);

        /// <summary>
        /// 重新绘制页面
        /// </summary>
        /// <param name="values"></param>
        void Repaint(params object[] values);
    }
}