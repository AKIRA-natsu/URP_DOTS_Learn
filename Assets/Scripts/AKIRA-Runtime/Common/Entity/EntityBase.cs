using AKIRA;
using AKIRA.Attribute;
using UnityEngine;

/// <summary>
/// 实体对象
/// </summary>
public abstract class EntityBase : MonoBehaviour, IUpdateCallback {
    // 更新组
    [SerializeField]
    [HideInInspector]
    [SelectionPop(typeof(GameData.Group))]
    private string updateGroup = GameData.Group.Default;
    
    // 更新方式
    [SerializeField]
    [HideInInspector]
    private UpdateMode updateMode = UpdateMode.Update;

    protected virtual void OnEnable() {
        // 如果重载自动更新
        if (IsOverride())
            this.Regist(updateGroup, updateMode);
    }
    
    protected virtual void OnDisable() {
        // 如果重载自动更移除
        if (IsOverride())
            this.Remove(updateGroup, updateMode);
    }

    public virtual void OnUpdate() { }
    public virtual void OnUpdateResume() { }
    public virtual void OnUpdateStop() { }

    /// <summary>
    /// 是否重写 OnUpdate
    /// </summary>
    /// <returns></returns>
    internal bool IsOverride() {
        return !(GetType().GetMethod("OnUpdate").DeclaringType == typeof(EntityBase));
    }
}
