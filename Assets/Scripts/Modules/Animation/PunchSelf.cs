using System.Collections;
using System.Collections.Generic;
using AKIRA.Manager;
using UnityEngine;

/// <summary>
/// 缩放效果
/// </summary>
public class PunchSelf : SelfAnim {
    [CNName("缩放程度（等比）")]
    public float punchValue = 1f;
    [CNName("缩放时间")]
    public float punchTime = 1f;
    [CNName("缩放动效")]
    [SerializeField]
    protected AnimationCurve punchCurve;

    // 当前动画时间
    protected float time = 0f;
    // 动画朝向
    protected PunchWard ward = PunchWard.Forward;
    // 原来的缩放大小
    protected Vector3 originScale;
    // 缩放目标和当前的差值
    protected Vector3 punchOffset;

    protected virtual void Awake() {
        this.originScale = this.transform.localScale;
    }

    protected override void OnEnable() {
        base.OnEnable();
        punchOffset = originScale * (punchValue - 1f);
    }

    private void OnValidate() {
        punchOffset = originScale * (punchValue - 1f);
    }

    public override void GameUpdate() {
        if (ward == PunchWard.Forward) {
            time += Time.deltaTime;
            if (time > punchTime)
                ward = PunchWard.Backward;
        } else {
            time -= Time.deltaTime;
            if (time < 0)
                ward = PunchWard.Forward;
        }

        this.transform.localScale = punchCurve.Evaluate(time / punchTime) * punchOffset + originScale;
    }
}
