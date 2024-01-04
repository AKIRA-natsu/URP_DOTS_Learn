using AKIRA;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// <para>给Button使用的未点击时缩放动效</para>
/// <para>punchTime需要统一！！</para>
/// </summary>
[RequireComponent(typeof(Button))]
public class UnClickPunchSelf : PunchSelf, IPointerDownHandler, IPointerUpHandler {
    // 自身按钮
    private Button selfButton;
    // 重写静态PunchWard
    private static PunchWard ward = PunchWard.Forward;
    // 重写静态时间time
    private static float time = 0f;
    // 实例计数
    private static int useInstanceCount = 0;

    protected override void Awake() {
        base.Awake();
        selfButton = this.GetComponent<Button>();
    }

    protected override void OnEnable() {
        base.OnEnable();
        useInstanceCount++;
    }

    protected override void OnDisable() {
        base.OnDisable();
        useInstanceCount--;
    }

    public override void GameUpdate() {
        // 维持静态的time增量与Time.deltaTime一致
        var deltaTime = Time.deltaTime / useInstanceCount;

        if (ward == PunchWard.Forward) {
            time += deltaTime;
            if (time >= punchTime)
                ward = PunchWard.Backward;
        } else {
            time -= deltaTime;
            if (time <= 0)
                ward = PunchWard.Forward;
        }

        if (!selfButton.interactable)
            return;

        this.transform.localScale = punchCurve.Evaluate(time / punchTime) * punchOffset + originScale;
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (auto)
            this.Remove(GameData.Group.Animation);
        this.transform.localScale = originScale;
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (auto)
            this.Regist(GameData.Group.Animation);
    }
}