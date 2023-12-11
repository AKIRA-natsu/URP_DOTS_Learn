/// <summary>
/// 攻击分组
/// </summary>
public enum AttackGroup {
    /// <summary>
    /// 无
    /// </summary>
    None,
    /// <summary>
    /// 友方，组员
    /// </summary>
    Partner,
    /// <summary>
    /// 敌方
    /// </summary>
    Enemy,
    /// <summary>
    /// 路人
    /// </summary>
    NPC,
    /// <summary>
    /// 第三方
    /// </summary>
    Third,
    /// <summary>
    /// 特殊
    /// </summary>
    Special,
    /// <summary>
    /// 其他
    /// </summary>
    Other,
}

/// <summary>
/// 攻击属性数据
/// </summary>
public abstract class AttackPropertyData {}

public interface IAttack {
    /// <summary>
    /// 所属攻击组
    /// </summary>
    /// <value></value>
    AttackGroup Group { get; }
    /// <summary>
    /// 攻击属性数据
    /// </summary>
    /// <value></value>
    AttackPropertyData Data { get; }
}

/// <summary>
/// 可攻击者
/// </summary>
public interface IAttackable : IAttack {
    /// <summary>
    /// 寻找攻击对象
    /// </summary>
    /// <param name="targetGroup"></param>
    /// <returns></returns>
    IAttackReciver FindReciver(AttackGroup targetGroup);

    /// <summary>
    /// 发送攻击信号
    /// </summary>
    /// <returns></returns>
    bool SendAttackSingal();
}

/// <summary>
/// 攻击对象
/// </summary>
public interface IAttackReciver : IAttack {
    /// <summary>
    /// 接受攻击信号
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    bool ReciveAttackSingal(IAttackable sender);
}