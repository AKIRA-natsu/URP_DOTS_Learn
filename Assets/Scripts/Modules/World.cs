using UnityEngine;

namespace AKIRA.Manager {
    /// <summary>
    /// 游戏启动器
    /// </summary>
    public class World : AbstractWorld, IAttachData {
        protected override string OnCompletedEventName => Consts.Event.OnInitSystemCompleted;
        public override bool DontDestory => false;

        protected override void Awake() {
            base.Awake();
            Application.targetFrameRate = 60;
        }

        public override void RegistModules() {}
    }
}