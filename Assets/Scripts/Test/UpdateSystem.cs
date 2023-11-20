using System;
using System.Collections.Generic;
using AKIRA;
using AKIRA.Manager;
using UnityEngine;

/// <summary>
/// 更新模式
/// </summary>
public enum UpdateMode {
    /// <summary>
    ///  
    /// </summary>
    Update,
    /// <summary>
    /// 
    /// </summary>
    FixedUpdate,
    /// <summary>
    /// 
    /// </summary>
    LateUpdate,
}

/// <summary>
/// 更新驱动管理
/// </summary>
public class UpdateSystem : MonoBehaviour {
    // 更新组列表
    [SerializeField]
    private List<IUpdate> updates = new();

    private static UpdateSystem instance;
    public static UpdateSystem Instance => instance;
    public static UpdateSystem GetOrCreateDefaultSystem() {
        if (instance == null) {
            instance = new GameObject("[UpdateSystem]").AddComponent<UpdateSystem>();
        }
        return instance;
    }

    /// <summary>
    /// <para>注册更新 <paramref name="update" /></para>
    /// <para>只注册无参类型，有参类型根据实际情况父物体遍历子节点更新</para>
    /// </summary>
    /// <param name="update"></param>
    /// <param name="key">组键值</param>
    /// <param name="mode">更新类型</param>
    public void Regist(IUpdate update, UpdateMode mode = UpdateMode.Update) {
        if (updates.Contains(update))
            return;
        updates.Add(update);
    }

    /// <summary>
    /// 移除更新
    /// </summary>
    /// <param name="update"></param>
    /// <param name="key">组键值</param>
    /// <param name="mode">更新类型</param>
    public void Remove(IUpdate update, UpdateMode mode = UpdateMode.Update) {
        if (!updates.Contains(update))
            return;
        updates.Remove(update);
    }

    private void Update() {
        for (int i = 0; i < updates.Count; i++)
            updates[i].GameUpdate();
    }
}

/// <summary>
/// 更新扩展
/// </summary>
public static class UpdateExtend {
    /// <summary>
    /// <para>注册更新</para>
    /// <para>等同于 UpdateSystem.Instance.Regist</para>
    /// </summary>
    /// <param name="update"></param>
    /// <param name="key">组键值</param>
    /// <param name="mode"></param>
    public static void Regist(this IUpdate update, string key = "Default", UpdateMode mode = UpdateMode.Update) {
        UpdateSystem.GetOrCreateDefaultSystem().Regist(update, mode);
    }

    /// <summary>
    /// <para>移除更新</para>
    /// <para>等同于 UpdateSystem.Instance.Remove</para>
    /// </summary>
    /// <param name="update"></param>
    /// <param name="key">组键值</param>
    /// <param name="mode"></param>
    public static void Remove(this IUpdate update, string key = "Default", UpdateMode mode = UpdateMode.Update) {
        UpdateSystem.GetOrCreateDefaultSystem().Remove(update, mode);
    }
}