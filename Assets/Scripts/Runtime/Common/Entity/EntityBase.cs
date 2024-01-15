using System.Threading.Tasks;
using AKIRA;
using AKIRA.Attribute;
using UnityEngine;

/// <summary>
/// 实体对象
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class EntityBase : MonoBehaviour, IUpdateCallback {
    // 更新组
    [SerializeField]
    [SelectionPop(typeof(GameData.Group))]
    private string updateGroup = GameData.Group.Default;
    
    // 更新方式
    [SerializeField]
    private UpdateMode updateMode = UpdateMode.Update;

    protected virtual async void OnEnable() {
        if (IsOverride()) {
            // 如果是对象池的Entity，wake比onenable晚，可能需要等待初始化
            if (GetType().IsSubclassOf(typeof(PoolEntityBase)))
                await Task.Yield();
            this.Regist(updateGroup, updateMode);
        }
    }
    
    protected virtual void OnDisable() {
        if (IsOverride())
            this.Remove(updateGroup, updateMode);
    }

    /// <summary>
    /// 是否重写GameUpdate，自动去添加/移除更新
    /// </summary>
    /// <returns></returns>
    private bool IsOverride() {
        return !(GetType().GetMethod("GameUpdate").DeclaringType == typeof(EntityBase));
    }

    public virtual void GameUpdate() { }
    public virtual void OnUpdateResume() { }
    public virtual void OnUpdateStop() { }
}

/// <summary>
/// 引用池实体对象
/// </summary>
public abstract class PoolEntityBase : EntityBase, IPool {
    // 比OnEnable晚
    public abstract void Wake(object data = null);
    // 比OnDisable早
    public abstract void Recycle(object data = null);
}