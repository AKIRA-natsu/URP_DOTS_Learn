using UnityEngine;

namespace AKIRA.Manager {
    /// <summary>
    /// Mono 单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DisallowMultipleComponent]
    public class MonoSingleton<T> : MonoBehaviour, ISystem where T : MonoSingleton<T> {
        private static T instance;
        // 舍弃自动生成Manager防止报错的方式
        // public static T Instance {
        //     get {
        //         if (instance == null) {
        //             // 直接 return 初始化前被调用导致会报错
        //             GameObject manager = new GameObject($"[{typeof(T).Name}]").DontDestory();
        //             instance = manager.AddComponent(typeof(T)) as T;
        //         }
        //         return instance;
        //     }
        // }

        public static T Instance => instance;

        /// <summary>
        /// 获得或创建默认Instance
        /// </summary>
        /// <returns></returns>
        public static T GetOrCreateDefaultInstance() {
            if (instance == null) {
                GameObject manager = new GameObject($"[{typeof(T).Name}]").DontDestory();
                instance = manager.AddComponent(typeof(T)) as T;
            }
            return instance;
        }

        protected virtual void Awake() {
            if (instance == null)
                instance = (T)this;
            else
                Destroy(gameObject);
        }

        protected virtual void OnDestroy() {
            if (instance == this)
                instance = null;
        }
    }
}
