using System.Collections.Generic;
using UnityEngine;
using AKIRA.Manager;
using System.Linq;

namespace AKIRA.UIFramework {
    /// <summary>
    /// <para>UI 基类</para>
    /// <para>拿取获得的基类</para>
    /// </summary>
    public abstract partial class UIComponent : UIBase {
        public GameObject gameObject { get; protected set; }
        public Transform transform { get; protected set; }
        // 优化UI页面的显示与隐藏
        protected CanvasGroup group;

        // 可适配组件列表
        public List<RectTransform> MatchableList { get; private set; } = new List<RectTransform>();
        
        internal bool active = true;
        /// <summary>
        /// 是否激活，set调用Show/Hide方法
        /// </summary>
        public bool Active { 
            get => active;
            set {
                if (value) {
                    Show();
                } else {
                    Hide();
                }
            }
        }

        public override void Awake(object obj) {
            WinType type = (WinType)obj;
            // 初始化创建
            this.gameObject = AssetSystem.Instance.LoadObject<GameObject>(UIDataManager.Instance.GetUIData(this).path)
                                                .Instantiate()
                                                .SetParent(UI.GetParent(type), true);
            this.transform = gameObject.transform;
            BindFields();

            group = this.gameObject.GetOrAddComponent<CanvasGroup>();
            InitAnimation();

            // 默认隐藏
            ActiveCanvasGroup(false);
            active = false;
        }

        /// <summary>
        /// 绑定私有字段
        /// </summary>
        protected void BindFields() {
            var fields = this.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            foreach (var field in fields) {
                var uIControls = field.GetCustomAttributes(typeof(UIControlAttribute), false);
                if (uIControls.Length == 0) continue;
                var uIControl = uIControls[0] as UIControlAttribute;

                if (field.FieldType.IsSubclassOf(typeof(UIComponentProp))) {
                    var componentProp = field.FieldType.CreateInstance<UIComponentProp>();
                    componentProp.Awake(this.transform.Find(uIControl.Path));
                    field.SetValue(this, componentProp);
                } else {
                    field.SetValue(this, this.transform.Find(uIControl.Path).GetComponent(field.FieldType));
                }

                if (uIControl.Matchable)
                    MatchableList.Add(this.transform.Find(uIControl.Path).GetComponent<RectTransform>());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="active"></param>
        private void ActiveCanvasGroup(bool active) {
            group.alpha = active ? 1f : 0f;
            group.blocksRaycasts = active;
            group.interactable = active;
        }

        /// <summary>
        /// <para>显示</para>
        /// <para>如果有IUIAnimation，group失效，显示交给IUIAnimatino控制</para>
        /// </summary>
        public virtual void Show(params object[] args) {
            if (active) return;
            if (animation == null) {
                ActiveCanvasGroup(true);
            } else {
                animation.OnShow(OnShowStart, OnShowEnd);
            }
            active = true;
            this.OnEnter();
        }

        /// <summary>
        /// <para>隐藏</para>
        /// <para>如果有IUIAnimation，group失效，隐藏交给IUIAnimatino控制</para>
        /// </summary>
        public virtual void Hide() {
            if (!active) return;
            if (animation == null) {
                ActiveCanvasGroup(false);
            } else {
                animation.OnHide(OnHideStart, OnHideEnd);
            }
            active = false;
            this.OnExit();
        }

        public override void Dispose() {
            this.gameObject.Destory();
        }

        public override void Invoke(string name, params object[] args) {
            // 防止同名称重写
            var methods = this.GetType().GetMethods().Where(method => method.Name.Equals(name));
            // 找参数一致的方法
            var method = methods.SingleOrDefault(method => method.GetParameters().Length == args.Length);
            if (method == null)
                return;
            method.Invoke(this, args);
        }
    }

    public partial class UIComponent {
        private IUIAnimation animation;

        /// <summary>
        /// 初始化UI动画
        /// </summary>
        protected void InitAnimation() {
            animation = this.transform.GetComponent<IUIAnimation>();
            animation?.OnInit(group);
        }

        /// <summary>
        /// 如果有IUIAnimation，OnShow函数的OnShowStart事件会走此函数
        /// </summary>
        protected virtual void OnShowStart() {}
        /// <summary>
        /// 如果有IUIAnimation，OnShow函数的OnShowEnd事件会走此函数
        /// </summary>
        protected virtual void OnShowEnd() {}
        /// <summary>
        /// 如果有IUIAnimation，OnHide函数的OnHideStart事件会走此函数
        /// </summary>
        protected virtual void OnHideStart() {}
        /// <summary>
        /// 如果有IUIAnimation，OnHide函数的OnHideEnd事件会走此函数
        /// </summary>
        protected virtual void OnHideEnd() {}
    }
}