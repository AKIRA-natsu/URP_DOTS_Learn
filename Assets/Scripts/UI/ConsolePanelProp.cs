using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AKIRA.UIFramework {
    public class ConsolePanelProp : UIComponent {
        [UIControl("ScrollView/Viewport/Content/ConsoleTxt")]
        protected TextMeshProUGUI ConsoleTxt;
        [UIControl("ScrollView/Viewport/Content")]
        protected RectTransform Content;
        [UIControl("ScrollView/Viewport/Content")]
        protected ContentSizeFitter ContentFitter;
        [UIControl("ScrollView/Viewport")]
        protected RectTransform Viewport;
        [UIControl("ScrollView/Scrollbar Vertical")]
        protected Scrollbar ScrollbarVertical;
        [UIControl("ScrollView")]
        protected ScrollRect ScrollView;
        [UIControl("ConsoleInput/Text Area/Text")]
        protected TextMeshProUGUI Text;
        [UIControl("ConsoleInput/Text Area")]
        protected TextMeshProUGUI TextArea;
        [UIControl("ConsoleInput")]
        protected TMP_InputField ConsoleInput;
    }
}