using UnityEngine;

public class TestClick : MonoBehaviour, IClick {
    public void OnClick() {
        "测试点击".Log();
    }
    
}