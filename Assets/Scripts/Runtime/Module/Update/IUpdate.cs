#region IUpdate
/// <summary>
/// 更新接口
/// </summary>
public interface IUpdate {
    /// <summary>
    /// 更新
    /// </summary>
    void GameUpdate();
}

/// <summary>
/// 更新接口
/// </summary>
/// <typeparam name="T">参数</typeparam>
public interface IUpdate<T> {
    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="value">参数</param>
    void GameUpdate(T value);
}

/// <summary>
/// 更新接口
/// </summary>
/// <typeparam name="T1">参数1</typeparam>
/// <typeparam name="T2">参数2</typeparam>
public interface IUpdate<T1, T2> {
    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="value1">参数1</param>
    /// <param name="value2">参数2</param>
    void GameUpdate(T1 value1, T2 value2);
}

/// <summary>
/// 更新接口
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
public interface IUpdate<T1, T2, T3> {
    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <param name="value3"></param>
    void GameUpdate(T1 value1, T2 value2, T3 value3);
}

/// <summary>
/// 更新接口
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
public interface IUpdate<T1, T2, T3, T4> {
    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <param name="value3"></param>
    /// <param name="value4"></param>
    void GameUpdate(T1 value1, T2 value2, T3 value3, T4 value4);
}

/// <summary>
/// 更新接口
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
public interface IUpdate<T1, T2, T3, T4, T5> {
    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <param name="value3"></param>
    /// <param name="value4"></param>
    /// <param name="value5"></param>
    void GameUpdate(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5);
}
#endregion

#region IUpdateCallback
/// <summary>
/// <para>更新接口</para>
/// <para>继承IUpdate，带恢复/暂停更新方法</para>
/// </summary>
public interface IUpdateCallback : IUpdate {
    /// <summary>
    /// 更新暂停
    /// </summary>
    void OnUpdateStop();
    /// <summary>
    /// 更新恢复
    /// </summary>
    void OnUpdateResume();
}

/// <summary>
/// <para>更新接口</para>
/// <para>继承IUpdate，带恢复/暂停更新方法</para>
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IUpdateCallback<T> : IUpdate<T> {
    /// <summary>
    /// 更新暂停
    /// </summary>
    void OnUpdateStop();
    /// <summary>
    /// 更新恢复
    /// </summary>
    void OnUpdateResume();
}

// /// <summary>
// /// <para>更新接口</para>
// /// <para>继承IUpdate，带恢复/暂停更新方法</para>
// /// </summary>
// /// <typeparam name="T1"></typeparam>
// /// <typeparam name="T2"></typeparam>
// public interface IUpdateCallback<T1, T2> : IUpdate<T1, T2> {}

// /// <summary>
// /// <para>更新接口</para>
// /// <para>继承IUpdate，带恢复/暂停更新方法</para>
// /// </summary>
// /// <typeparam name="T1"></typeparam>
// /// <typeparam name="T2"></typeparam>
// /// <typeparam name="T3"></typeparam>
// public interface IUpdateCallback<T1, T2, T3> : IUpdate<T1, T2, T3> {}

// /// <summary>
// /// <para>更新接口</para>
// /// <para>继承IUpdate，带恢复/暂停更新方法</para>
// /// </summary>
// /// <typeparam name="T1"></typeparam>
// /// <typeparam name="T2"></typeparam>
// /// <typeparam name="T3"></typeparam>
// /// <typeparam name="T4"></typeparam>
// public interface IUpdateCallback<T1, T2, T3, T4> : IUpdate<T1, T2, T3, T4> {}

// /// <summary>
// /// <para>更新接口</para>
// /// <para>继承IUpdate，带恢复/暂停更新方法</para>
// /// </summary>
// /// <typeparam name="T1"></typeparam>
// /// <typeparam name="T2"></typeparam>
// /// <typeparam name="T3"></typeparam>
// /// <typeparam name="T4"></typeparam>
// /// <typeparam name="T5"></typeparam>
// public interface IUpdateCallback<T1, T2, T3, T4, T5> : IUpdate<T1, T2, T3, T4, T5> {}
#endregion