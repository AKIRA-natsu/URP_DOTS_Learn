using System;
using AKIRA.UIFramework;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 遮罩
/// </summary>
public class GuideMask : MonoBehaviour, IUpdate {
    /// <summary>
    /// 图片
    /// </summary>
    private Image maskImg;

    /// <summary>
    /// 区域范围缓存
    /// </summary>
    private Vector3[] corners = new Vector3[4];
    /// <summary>
    /// 高亮区域半径
    /// </summary>
    private float startRadius;
    /// <summary>
    /// 最终显示高亮半径
    /// </summary>
    private float endRadius;
    /// <summary>
    /// 遮罩材质球
    /// </summary>
    private Material maskMat;
    /// <summary>
    /// 遮罩材质球圆心
    /// </summary>
    private static int CenterKey = Shader.PropertyToID("_Center");
    /// <summary>
    /// 社招材质球滑动条
    /// </summary>
    private static int SliderKey = Shader.PropertyToID("_Slider");

    [CNName("光亮半径差值")]
    public float radiusOffset = 30f;

    /// <summary>
    /// 收缩速度
    /// </summary>
    private float shrinkVelocity = 0f;
    /// <summary>
    /// 收缩时间
    /// </summary>
    private float shrinkTime = 0f;

    private Action onShrinkEnd;

    private void Awake() {
        maskImg = this.GetComponent<Image>();
        maskMat = maskImg.material;
    }

    /// <summary>
    /// 更新遮罩
    /// </summary>
    /// <param name="info"></param>
    public void RefreshMask(in GuideInfo info) {
        // 获得高亮区域的四个顶点坐标
        info.arrowTarget.GetComponent<RectTransform>().GetWorldCorners(corners);
        // 计算最终高亮显示区域的半径
        endRadius = Vector2.Distance(corners[0].WorldToCanvas(), corners[2].WorldToCanvas()) / 2f + radiusOffset;
        // 计算高亮显示区域圆心
        float x = corners[0].x + ((corners[3].x - corners[0].x) / 2f);
        float y = corners[0].y + ((corners[1].y - corners[0].y) / 2f);
        Vector2 center = new Vector3(x, y, 0).WorldToCanvas();
        // 设置遮罩材质球的圆心变量
        Vector4 centerMat = new Vector4(center.x, center.y, 0, 0);
        maskMat.SetVector(CenterKey, centerMat);
        // 计算当前高亮显示区域的半径
        // 获取画布区域的四个顶点
        UI.Rect.GetWorldCorners(corners);
        // 画布顶点距离高亮区域最远的距离作为当前高亮区域半径的初始值
        startRadius = 0f;
        foreach (var corner in corners)
            startRadius = Mathf.Max(startRadius, Vector3.Distance(center, corner.WorldToCanvas()));
        maskMat.SetFloat(SliderKey, startRadius);
    }

    /// <summary>
    /// 激活
    /// </summary>
    /// <param name="active"></param>
    public void Active(bool active) {
        maskImg.enabled = active;
    }

    /// <summary>
    /// 开始遮罩动画
    /// </summary>
    /// <param name="time">时间</param>
    /// <param name="callback">回调</param>
    public void DoMaskAnim(float time, Action callback = null) {
        this.shrinkTime = time;
        this.onShrinkEnd = callback;
        this.Regist();
    }

    public void GameUpdate() {
        var value = Mathf.SmoothDamp(startRadius, endRadius, ref shrinkVelocity, shrinkTime);
        if (Mathf.Abs(value - endRadius) > 0.001f) {
            startRadius = value;
            maskMat.SetFloat(SliderKey, startRadius);
        } else {
            this.Remove();
            onShrinkEnd?.Invoke();
        }
    }
}