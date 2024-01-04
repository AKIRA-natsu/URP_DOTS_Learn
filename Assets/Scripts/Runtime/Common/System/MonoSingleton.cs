using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace AKIRA.Manager {
    /// <summary>
    /// Mono 单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DisallowMultipleComponent]
    public class MonoSingleton<T> : MonoBehaviour, ISystem where T : MonoSingleton<T> {
        private static T instance;
        public static T Instance => instance;

        protected List<IController> controllers = new();


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

        public async virtual Task Initialize() {
            await Task.Yield();
        }

        /// <summary>
        /// 生成Controller实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected async Task<U> CreateController<U>(string dll = GameData.DLL.Default) where U : IController {
            IController controller = default;
            if (typeof(U).IsSubclassOf(typeof(Component))) {
                var component = new GameObject($"[{typeof(U).Name}]").AddComponent(typeof(U));
                component.SetParent(this);
                controller = component as IController;
            } else {
                controller = typeof(U).CreateInstance<U>(dll);
            }
            await controller.Initialize();
            controllers.Add(controller);
            return (U)controller;
        }

        /// <summary>
        /// 获得Controller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public U GetController<U>() where U : IController {
            return (U)controllers.SingleOrDefault(controller => controller is U);
        }
    }
}
