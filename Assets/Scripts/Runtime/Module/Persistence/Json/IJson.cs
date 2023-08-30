/// <summary>
/// Json
/// </summary>
public interface IJson {
    /// <summary>
    /// 路径
    /// </summary>
    /// <value></value>
    string Path { get; }
    /// <summary>
    /// 数据
    /// </summary>
    /// <value></value>
    object Data { get; set; }
}