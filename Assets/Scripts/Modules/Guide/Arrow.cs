using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 指引2D箭头
/// </summary>
public class Arrow : MonoBehaviour {
    // 箭头
    private Transform render;
    // 跟随目标
    public Transform follow;
    // 指向目标
    public Transform target;

    // 围绕半径
    public float radius = 1f;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="follow"></param>
    public void Init(Transform follow) {
        this.follow = follow;
        render = this.transform.GetChild(0);
        render.localScale = Vector3.zero;
    }

    /// <summary>
    /// 设置目标箭头
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target) {
        this.target = target;
        render.DOScale(1f, 0.5f).SetEase(Ease.InBack);
        // 有延迟，手动更新一次
        UpdateArrow();
    }

    /// <summary>
    /// 清除箭头
    /// </summary>
    public void ClearTarget() {
        target = null;
        render.DOScale(0f, 0.5f).SetEase(Ease.OutBack);
    }

    /// <summary>
    /// 更新箭头
    /// </summary>
    public void UpdateArrow() {
        this.transform.position = follow.position;
        // 调整转向
        this.transform.LookAt(target);
        // 更新箭头位置
        render.transform.position = this.transform.position + this.transform.forward * radius;
    }

    /// <summary>
    /// 更新箭头
    /// </summary>
    /// <param name="position"></param>
    public void UpdateArrow(Vector3 position) {
        this.transform.position = follow.position;
        // 调整转向
        this.transform.LookAt(position);
        // 更新箭头位置
        render.transform.position = this.transform.position + this.transform.forward * radius;
    }
}
