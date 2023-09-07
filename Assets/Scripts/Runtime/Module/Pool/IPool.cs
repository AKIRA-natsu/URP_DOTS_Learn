/// <summary>
/// 对象池接口
/// </summary>
public interface IPool {
    /// <summary>
    /// 唤醒
    /// </summary>
    void Wake(object data = null);
    
    /// <summary>
    /// 回收
    /// </summary>
    void Recycle(object data = null);
}