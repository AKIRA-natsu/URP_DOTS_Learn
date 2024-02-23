namespace AKIRA.Behaviour.GM {
    /// <summary>
    /// 调试窗口接口
    /// </summary>
    public interface IConsoleUI {
        /// <summary>
        /// UI 显示与隐藏
        /// </summary>
        /// <value></value>
        bool Active { get; set; }

        /// <summary>
        /// 重新绘制页面
        /// </summary>
        /// <param name="values"></param>
        void Repaint(params object[] values);
    }

    /// <summary>
    /// GM 命令行
    /// </summary>
    public interface IGMCommand {
        /// <summary>
        /// 命令行名称，大小写都行
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 运行处理逻辑
        /// </summary>
        /// <param name="value">参数</param>
        string Excute(params string[] values);
    }
}