using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AKIRA.UIFramework {
    public class GuidePanelProp : UIComponent {
        [UIControl("Mask")]
        protected GuideMask Mask;
        [UIControl("Rigid")]
        protected BoxCollider2D Rigid;
        [UIControl("Dialog")]
        protected RectTransform Dialog;
        [UIControl("Dialog")]
        protected CanvasGroup DialogGroup;
        [UIControl("Dialog/Bg")]
        protected RectTransform DialogBg;
        [UIControl("Dialog/DialogContext")]
        protected TextMeshProUGUI DialogContext;
    }
}