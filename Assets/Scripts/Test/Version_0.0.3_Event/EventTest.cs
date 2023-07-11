using UnityEngine;
using AKIRA.Manager;

public class EventTest : MonoBehaviour {
    public string event1;
    public string event2;
    public string event3;

    [ContextMenu("Test Event")]
    private void Start() {
        EventSystem.Instance.AddEventListener(event1, EventOneLog);
        EventSystem.Instance.AddEventListener(event2, EventTwoLog);
        EventSystem.Instance.AddEventListener(event3, EventThreeLog);

        EventSystem.Instance.TriggerEvent(event1);
    }

    private void EventOneLog(object data) {
        // 不移除右键触发TestEvent无限添加事件了。
        EventSystem.Instance.RemoveEventListener(event1, EventOneLog);
        EventSystem.Instance.TriggerEvent(event3);
        EventSystem.Instance.TriggerEvent(event2);
        "this is event1，应该比Event2提前打日志".Log(); // 3
    }

    private void EventTwoLog(object data) {
        EventSystem.Instance.RemoveEventListener(event2, EventTwoLog);
        "this is event2, 被Event1触发，错误顺序：在Event1 log前就打了此日志".Log(); // 2
    }

    private void EventThreeLog(object data) {
        EventSystem.Instance.RemoveEventListener(event3, EventThreeLog);
        "this is event3, 被Event1触发，错误顺序：在Event1 log前就打了此日志".Log(); // 1
    }
}