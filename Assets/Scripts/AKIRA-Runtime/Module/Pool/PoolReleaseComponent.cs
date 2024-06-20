using System;
using UnityEngine;

namespace AKIRA.Manager {
    /// <summary>
    /// Pool 自动释放组件
    /// </summary>
    internal class PoolReleaseComponent : MonoBehaviour {
        private bool inited = false;                // check inited

        [ReadOnly]
        [SerializeField]
        private string key;                         // pool key

        private DateTime time;                      // last used real time
        #if UNITY_EDITOR
        [ReadOnly]
        [SerializeField]
        private string lastUsedTime;                // last used time
        #endif

        private const int ReleaseMinute = 30;       // release minute

        public void Init(string key) {
            this.key = key;
            UpdateTime();
            inited = true;
        }

        public void UpdateTime() {
            time = DateTime.Now;
            #if UNITY_EDITOR
            lastUsedTime = time.ToString(@"HH:mm:ss");
            #endif
        }

        private void Update() {
            if (!inited)
                return;

            var cur = DateTime.Now;
            if ((cur - time).TotalMinutes > ReleaseMinute) {
                UpdateTime();
                $"Pool => Try Auto Release {this.gameObject}".Log(GameData.Log.Editor);
                ObjectPool.Instance.Release(key);
            }
        }
    }
}