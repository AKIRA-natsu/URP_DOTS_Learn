using AKIRA;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI 点击缩放效果 不受Auto Update影响
/// </summary>
public class ClickPunchSelf : PunchSelf, IPointerDownHandler, IPointerUpHandler {
    protected override void OnEnable() {
        punchOffset = originScale * (punchValue - 1f);
    }

    protected override void OnDisable() { }

    private void OnValidate() {
        punchOffset = originScale * (punchValue - 1f);
    }

    public override void OnUpdate() {
        if (ward == PunchWard.Forward) {
            if (time >= punchTime)
                return;
            time += Time.deltaTime;
        } else {
            if (time <= 0) {
                this.Remove(GameData.Group.Animation);
                return;
            }
            time -= Time.deltaTime;
        }

        this.transform.localScale = punchCurve.Evaluate(time / punchTime) * punchOffset + originScale;
    }

    public void OnPointerDown(PointerEventData eventData) {
        ward = PunchWard.Forward;
        this.Regist(GameData.Group.Animation);
    }

    public void OnPointerUp(PointerEventData eventData) {
        ward = PunchWard.Backward;
    }
}