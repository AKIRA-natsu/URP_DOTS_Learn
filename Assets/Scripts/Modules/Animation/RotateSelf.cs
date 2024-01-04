using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 旋转朝向轴
/// </summary>
public enum RotateTowards {
    X,
    Y,
    Z,
}

/// <summary>
/// 旋转效果
/// </summary>
public class RotateSelf : SelfAnim {
    [CNName("旋转朝向")]
    [SerializeField]
    private RotateTowards towards;
    [CNName("旋转速度 ")]
    public float rotateSpeed;

    // 方向
    private Vector3 rotate;
    

    private void OnValidate() {
        DecideTowards();
    }

    protected override void OnEnable() {
        base.OnEnable();
        DecideTowards();
    }

    /// <summary>
    /// 决定旋转朝向
    /// </summary>
    private void DecideTowards() {
        switch (towards) {
            case RotateTowards.X:
                rotate = Vector3.right;
                break;
            case RotateTowards.Y:
                rotate = Vector3.down;
                break;
            case RotateTowards.Z:
                rotate = Vector3.forward;
                break;
        }
    }

    /// <summary>
    /// 旋转
    /// </summary>
    public override void GameUpdate() {
        this.transform.Rotate(rotate * rotateSpeed, Space.Self);
    }
}
