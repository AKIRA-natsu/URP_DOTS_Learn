using UnityEngine;

public class TestDrag : MonoBehaviour, IDrag {
    public void OnDragDown() {
        "测试拖拽按下".Log();
    }

    public void OnDrag() {
        "测试拖拽ing".Log();
    }

    public void OnDragUp() {
        "测试拖拽抬起".Log();
    }
}