using UnityEngine;

/// <summary>
/// 移动效果
/// </summary>
public class MoveSelf : SelfAnim {
    
    [CNName("移动朝向")]
    [SerializeField]
    private RotateTowards towards;
    [CNName("移动速度 ")]
    public float moveSpeed;
    [CNName("移动半径")]
    public float moveRadius;

    // 方向
    private Vector3 direction;
    // 原来坐标
    private Vector3 localPosition;

    private void Awake() {
        localPosition = this.transform.localPosition;
    }

    private void OnValidate() {
        DecideDirection();
    }

    protected override void OnEnable() {
        base.OnEnable();
        DecideDirection();
        // 隐藏期间改动位置
        localPosition = this.transform.localPosition;
    }

    protected override void OnDisable() {
        base.OnDisable();
        // 回到最初位置
        this.transform.localPosition = localPosition;
    }

    private void DecideDirection() {
        switch (towards) {
            case RotateTowards.X:
                direction = Vector3.right;
                break;
            case RotateTowards.Y:
                direction = Vector3.up;
                break;
            case RotateTowards.Z:
                direction = Vector3.forward;
                break;
        }
    }

    public override void GameUpdate() {
        this.transform.localPosition = localPosition + direction * moveRadius * Mathf.Sin(moveSpeed * Time.time);
    }
}