using System;
using System.Collections.Generic;
using UnityEngine;

namespace AKIRA.UIFramework {
    /// <summary>
    /// <para>适用UI组件 基类</para>
    /// </summary>
    public abstract class UIComponentProp : UIComponent {
        public override void Awake(object obj) {
            this.gameObject = (GameObject)obj;
            this.transform = this.gameObject.transform;
            BindFields();
            group = this.gameObject.AddComponent<CanvasGroup>();
        }
    }
}