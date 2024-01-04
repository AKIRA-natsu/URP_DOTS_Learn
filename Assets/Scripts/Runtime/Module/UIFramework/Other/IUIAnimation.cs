using System;
using UnityEngine;

namespace AKIRA.UIFramework {
    /// <summary>
    /// <para>UI开启关闭动画</para>
    /// <para>如果存在多个，只会走第一个</para>
    /// </summary>
    public interface IUIAnimation {
        /// <summary>
        /// 初始化，因为开启关闭交给动画控制，group必须需要在动画中调整
        /// </summary>
        void OnInit(CanvasGroup group);

        /// <summary>
        /// 动画显示
        /// </summary>
        /// <param name="onShowStart"></param>
        /// <param name="onShowEnd"></param>
        void OnShow(Action onShowStart, Action onShowEnd);

        /// <summary>
        /// 动画隐藏
        /// </summary>
        /// <param name="onHideStart"></param>
        /// <param name="onHideEnd"></param>
        void OnHide(Action onHideStart, Action onHideEnd);
    }
}