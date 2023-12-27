using System;
using AKIRA.UIFramework;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// UI页面Fade动效
/// </summary>
public class UIFadeAnimation : MonoBehaviour, IUIAnimation {
    private CanvasGroup group;
    
    [SerializeField]
    private Ease showEase = Ease.Linear;
    [SerializeField]
    private Ease hideEase = Ease.Linear;
    
    [SerializeField]
    [Min(0f)]
    private float fadeDuration = .3f;

    private Tween tween;

    public void OnInit(CanvasGroup group) {
        this.group = group;
    }

    public void OnHide(Action onHideStart, Action onHideEnd) {
        tween?.Kill(true);
        tween = group.DOFade(0f, fadeDuration).SetEase(hideEase).OnStart(() => onHideStart.Invoke()).OnComplete(() => {
            onHideEnd.Invoke();
            group.blocksRaycasts = false;
            group.interactable = false;
        });
    }

    public void OnShow(Action onShowStart, Action onShowEnd) {
        tween?.Kill(true);
        tween = group.DOFade(1f, fadeDuration).SetEase(showEase).OnStart(() => onShowStart.Invoke()).OnComplete(() => {
            onShowEnd.Invoke();
            group.blocksRaycasts = true;
            group.interactable = true;
        });
    }
}