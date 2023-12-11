using System;
using System.Collections.Generic;
using UnityEngine;
using AKIRA.Manager;

namespace AKIRA.UIFramework {
    /// <summary>
    /// <para>UI 基类</para>
    /// <para>拿取获得的基类</para>
    /// </summary>
    public abstract class UIComponent : UIBase {
        public GameObject gameObject { get; protected set; }
        public Transform transform { get; protected set; }
        // 优化UI页面的显示与隐藏
        protected CanvasGroup group;

        // 可适配组件列表
        public List<RectTransform> MatchableList { get; private set; } = new List<RectTransform>();
        
        private bool active = true;
        /// <summary>
        /// 是否激活，set调用Show/Hide方法
        /// </summary>
        public bool Active { 
            get => active;
            set {
                if (active == value)
                    return;
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
                                                .SetParent(GetParent(type), true);
            this.transform = gameObject.transform;
            BindFields();

            group = this.gameObject.AddComponent<CanvasGroup>();
            Hide();
        }

        /// <summary>
        /// 获得UI Game Object父节点
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private GameObject GetParent(WinType type) {
            switch (type) {
                case WinType.Normal:
                    return UI.View;
                case WinType.Interlude:
                    return UI.Top;
                case WinType.Notify:
                    return UI.Background;
                default:
                    return UI.View;
            }
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
        /// 显示
        /// </summary>
        public virtual void Show() {
            group.alpha = 1f;
            group.blocksRaycasts = true;
            group.interactable = true;
            active = true;
            this.OnEnter();
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public virtual void Hide() {
            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;
            active = false;
            this.OnExit();
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public virtual void Destory() {
            this.gameObject.Destory();
        }

        /// <summary>
        /// 获得Props数组
        /// </summary>
        /// <param name="parent"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T[] GetComponentProps<T>(RectTransform parent) where T : UIComponentProp, new() {
            var count = parent.childCount;
            List<T> result = new();
            var typeName = typeof(T).Name;
            for (int i = 0; i < count; i++) {
                var child = parent.GetChild(i);
                if (!typeName.Contains(child.name))
                    continue;
                var component = new T();
                component.Awake(child);
                result.Add(component);
            }
            return result.ToArray();
        }

        public override void Invoke(string name, params object[] args) { }
    }
}