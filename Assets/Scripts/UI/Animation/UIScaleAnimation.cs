using System;
using AKIRA.UIFramework;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// UI页面Scale动效
/// </summary>
public class UIScaleAnimation : MonoBehaviour, IUIAnimation {
    private CanvasGroup group;
    [SerializeField]
    private RectTransform target;
    
    [SerializeField]
    private Ease showEase = Ease.OutBack;
    [SerializeField]
    private Ease hideEase = Ease.InBack;
    
    [SerializeField]
    [Min(0f)]
    private float scaleDuration = .3f;

    private Tween tween;

    public void OnInit(CanvasGroup group) {
        this.group = group;
        if (target == null)
            target = this.GetComponent<RectTransform>();
        target.localScale = Vector3.zero;
    }

    public void OnHide(Action onHideStart, Action onHideEnd) {
        tween?.Kill(true);
        tween = target.DOScale(0f, scaleDuration).SetEase(hideEase).OnStart(() => onHideStart.Invoke()).OnComplete(() => {
            onHideEnd.Invoke();
            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;
        });
    }

    public void OnShow(Action onShowStart, Action onShowEnd) {
        group.alpha = 1f;
        tween?.Kill(true);
        tween = target.DOScale(1f, scaleDuration).SetEase(showEase).OnStart(() => onShowStart.Invoke()).OnComplete(() => {
            onShowEnd.Invoke();
            group.blocksRaycasts = true;
            group.interactable = true;
        });
    }
}