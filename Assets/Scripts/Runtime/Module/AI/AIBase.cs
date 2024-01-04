using AKIRA.Behaviour.Aniamtion;
using UnityEngine;

namespace AKIRA.Behaviour.AI {
    /// <summary>
    /// 基类
    /// </summary>
    [SelectionBase]
    public abstract class AIBase : MonoBehaviour, IUpdateCallback {
        // parameters
        private AnimationController ianima;
        /// <summary>
        /// 动画
        /// </summary>
        /// <value></value>
        public AnimationController Animation {
            get {
                if (ianima == null)
                    ianima = this.GetComponentInChildren<AnimationController>();
                return ianima;
            }
        }

        /// <summary>
        /// 更新模式
        /// </summary>
        [SerializeField]
        protected UpdateMode mode = UpdateMode.Update;

        // abstract functions
        public abstract void GameUpdate();
        public abstract void OnUpdateStop();
        public abstract void OnUpdateResume();
    }
}