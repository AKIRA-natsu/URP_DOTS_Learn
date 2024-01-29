using UnityEngine;

namespace AKIRA.UIFramework {
    /// <summary>
    /// 红点组件
    /// </summary>
    public class ReddotComponent : MonoBehaviour {
        // 链接UI对象
        [field: SerializeField]
        public string Linker { get; private set; }

        // 红点标签
        [field: SerializeField]
        public string ReddotTag { get; private set; }

        public bool Active {
            get => this.gameObject.activeSelf;
            set => this.gameObject.SetActive(value);
        }

        /// <summary>
        /// 设置控制此红点的UI点
        /// </summary>
        public void SetLinker() => SetLinker(this.transform);
        private void SetLinker(Transform trans) {
            if (trans == null)
                return;
            var type = $"{trans.name}Panel".GetConfigTypeByAssembley();
            if (type == null) {
                type = $"{trans.name}Component".GetConfigTypeByAssembley();
                if (type == null) {
                    SetLinker(trans.parent);
                } else {
                    Linker = type.Name;
                }
            } else {
                Linker = type.Name;
            }
        }
    }
}