using UnityEngine;
using AKIRA.Manager;
using System;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
#endif

namespace AKIRA.UIFramework {
    [Win(WinEnum.Guide, "Assets/Res/MainBundle/Prefabs/UI/Guide.prefab", WinType.Interlude)]
    public class GuidePanel : GuidePanelProp, IUpdate {
        public override void Awake(object obj) {
            base.Awake(obj);
            // 初始化隐藏
            Hide();
            Mask.Active(false);
            
            EventSystem.Instance.AddEventListener(GameData.Event.OnInitSystemCompleted, _ => {
                EventSystem.Instance.AddEventListener(GameData.Event.OnGuidenceCompleted, _ => Hide());
                GuideSystem.Instance.RegistOnGuideUIResumeAction(() => Show());
                GuideSystem.Instance.RegistOnGuideUIPauseAction(Hide);
            });

#if UNITY_ANDROID || UNITY_IOS
        EnhancedTouchSupport.Enable();
#endif
        }

        /// <summary>
        /// 接收指引
        /// </summary>
        /// <param name="info"></param>
        public async void ReceiveGuideInfo(GuideInfo info) {
            if (info.isShowBg) {
                Mask.RefreshMask(info);
                Mask.Active(true);
                Mask.DoMaskAnim(0.3f);
                Rigid.transform.position = info.arrowTarget.transform.position;
                Rigid.size = info.arrowTarget.GetComponent<RectTransform>().sizeDelta;
            } else {
                Mask.Active(false);
                Rigid.transform.localPosition = Vector3.zero;
                Rigid.size = OutScreenMark.ScreenRightTop;
            }

            if (info.dialogDirection == GuideDialogDirection.None || String.IsNullOrWhiteSpace(info.dialog)) {
                DialogGroup.alpha = 0f;
            } else {
                UpdateDialogPosition(info.dialogDirection);
                DialogContext.text = info.dialog;
                if (DialogGroup.alpha <= 0.99f) {
                    while (DialogGroup.alpha <= 0.99f) {
                        await UniTask.DelayFrame(0);
                        DialogGroup.alpha = Mathf.Lerp(DialogGroup.alpha, 1f, Time.deltaTime * 5f);
                    }
                    DialogGroup.alpha = 1f;
                }
            }

            Rigid.enabled = true;
            this.Regist(GameData.Group.UI);
        }

        /// <summary>
        /// 更新Dialog位置
        /// </summary>
        /// <param name="direction"></param>
        private void UpdateDialogPosition(GuideDialogDirection direction) {
            Vector2 position = default;
            var size = DialogBg.sizeDelta;
            var x = Mathf.Abs(Screen.width / 2 - OutScreenMark.WidthOffset - size.x);
            var y = Mathf.Abs(Screen.height / 2 - OutScreenMark.HeightOffset - size.y);
            switch (direction) {
                case GuideDialogDirection.Up:
                position = new Vector2(0, y);
                break;
                case GuideDialogDirection.Down:
                position = new Vector2(0, -y);
                break;
                case GuideDialogDirection.Left:
                position = new Vector2(-x, 0);
                break;
                case GuideDialogDirection.Right:
                position = new Vector2(x, 0);
                break;
                case GuideDialogDirection.Center:
                break;
            }
            Dialog.anchoredPosition = position;
        }

        /// <summary>
        /// 结束教程
        /// </summary>
        private void EndGuide() {
            this.Remove(GameData.Group.UI);
            Rigid.enabled = false;
            Mask.Active(false);
            GuideSystem.Instance.NextGuide(0.3f);
        }

        public void GameUpdate() {
            if (GuideSystem.Instance.CurrentIGuide == null) {
#if UNITY_EDITOR
                if (Mouse.current.leftButton.wasPressedThisFrame)
#else
                if (Touch.activeTouches.Count > 0)
#endif
                {
                    // 判断是否按在目标上
#if UNITY_EDITOR
                    var hit = Physics2D.Raycast(UI.UICamera.transform.position, Mouse.current.position.ReadValue().ScreenToUGUI());
#else
                    var hit = Physics2D.Raycast(UI.UICamera.transform.position, Touch.activeTouches[0].screenPosition.ScreenToUGUI());
#endif
                    if (hit.collider != null && hit.collider.Equals(Rigid))
                        EndGuide();
                }
            } else {
                if (GuideSystem.Instance.CurrentIGuide.FinishCondition())
                    EndGuide();
            }

        }
    }
}