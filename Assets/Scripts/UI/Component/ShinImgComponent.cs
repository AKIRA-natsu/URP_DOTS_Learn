using UnityEngine;

namespace AKIRA.UIFramework {
    /// <summary>
    /// 开启/关闭在页面调用Show/Hide
    /// </summary>
    public class ShinImgComponent : ShinImgComponentProp, IUpdate {
        /// <summary>
        /// 闪光连续次数
        /// </summary>
        public int continuousCount = 2;
        /// <summary>
        /// 闪光次数间隔时间
        /// </summary>
        public float interval = 2;
        /// <summary>
        /// 闪光速度
        /// </summary>
        public float speed = 4f;
        // 初始位置
        private readonly Rect Origin = new Rect(1, 0, 1, 1);

        // 变化值
        private float value = 1;
        // 当前次数
        private int curCount;
        // 上一次间隔时间
        private float lastIntervalTime;

        protected override void OnEnter() {
            this.Regist(GameData.Group.UI);
            ShinImg.uvRect = Origin;
        }

        protected override void OnExit() {
            this.Remove(GameData.Group.UI);
        }

        public void OnUpdate() {
            if (curCount == continuousCount) {
                if (Time.time - lastIntervalTime <= interval)
                    return;
                
                curCount = 0;
            }

            if (value <= -1) {
                value = 1;
                ShinImg.uvRect = Origin;
                if (++curCount == continuousCount)
                    lastIntervalTime = Time.time;
            } else {
                value = Mathf.Lerp(value, -1.1f, Time.deltaTime * speed);
                ShinImg.uvRect = new Rect(value, 0, 1, 1);
            }
        }

    }
}